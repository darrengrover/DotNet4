using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Newtonsoft.Json;

namespace TagReaderShared
{
    /// <summary>
    /// Shared IPC wrapper used by both:
    ///  - Service (publisher): call CreatePublisher()
    ///  - Monitor (consumer):  call OpenConsumerWithRetry()
    /// </summary>
    public sealed class SharedReaderStatus : IDisposable
    {
        // Single, global name so Session 0 (service) and the desktop session can share it
        private const string BASE_NAME = "SoftwareClever.TagReaderStatus.v2";
        private const string GLOBAL_MMFNAME = @"Global\" + BASE_NAME;
        private const string LOCAL_DEV_NAME = @"Local\" + BASE_NAME + ".dev";

        // Headroom for JSON payload
        private const int MMF_SIZE = 32768;

        private MemoryMappedFile _mmf;
        private readonly object _lock = new object();

        // Local cache for publisher writes
        private Dictionary<int, ReaderStatusDto> _statusCache = new Dictionary<int, ReaderStatusDto>();

        private volatile bool _isHealthy;
        public bool IsHealthy { get { return _isHealthy; } }

        private SharedReaderStatus(MemoryMappedFile mmf)
        {
            if (mmf == null) throw new ArgumentNullException(nameof(mmf));
            _mmf = mmf;
            _isHealthy = true;
        }

        public static SharedReaderStatus CreatePublisher()
        {
            var isService = !Environment.UserInteractive;

            if (isService)
            {
                // Service: Global with ACLs
                var mmf = TryCreatePublisher(GLOBAL_MMFNAME, security: BuildSecurity());
                if (mmf != null) return new SharedReaderStatus(mmf);

                // If some old/global object is blocking us, change the version string and rebuild, or reboot to clear.
                throw new UnauthorizedAccessException("Failed to create Global mapping; a stale object may exist with a restrictive DACL.");
            }
            else
            {
                // Console debug: Local DEV with default security (no ACL object passed)
                var mmf = TryCreatePublisher(LOCAL_DEV_NAME, security: null);
                if (mmf != null) return new SharedReaderStatus(mmf);

                throw new UnauthorizedAccessException("Failed to create Local DEV mapping; a stale object may exist with a restrictive DACL.");
            }
        }
        private static MemoryMappedFile TryCreatePublisher(string name, MemoryMappedFileSecurity security)
        {
            try
            {
                if (security == null)
                {
                    try { return MemoryMappedFile.CreateNew(name, MMF_SIZE); }
                    catch (IOException) { return MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.ReadWrite); }
                }
                else
                {
                    try
                    {
                        return MemoryMappedFile.CreateNew(
                            name, MMF_SIZE, MemoryMappedFileAccess.ReadWrite,
                            MemoryMappedFileOptions.None, security, HandleInheritability.None);
                    }
                    catch (IOException)
                    {
                        return MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.ReadWrite);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine("CreatePublisher access denied for '" + name + "': " + ex.Message);
                return null;
            }
        }
        private static MemoryMappedFileSecurity BuildSecurity()
        {
            var sec = new MemoryMappedFileSecurity();

            sec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(
                new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                MemoryMappedFileRights.FullControl, AccessControlType.Allow));

            sec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(
                new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                MemoryMappedFileRights.FullControl, AccessControlType.Allow));

            // Desktop users
            sec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(
                new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                MemoryMappedFileRights.ReadWrite, AccessControlType.Allow));

            return sec;
        }

        public static SharedReaderStatus OpenConsumerWithRetry(int retries, int delayMsBetweenRetries)
        {
            int attempts = Math.Max(retries, 1);
            int delay = Math.Max(delayMsBetweenRetries, 0);

            for (int i = 0; i < attempts; i++)
            {
                var mmf = TryOpen(GLOBAL_MMFNAME);
                if (mmf != null) return new SharedReaderStatus(mmf);

                mmf = TryOpen(LOCAL_DEV_NAME);
                if (mmf != null) return new SharedReaderStatus(mmf);

                System.Threading.Thread.Sleep(delay);
            }

            throw new InvalidOperationException("Shared memory not found in Global or Local (dev).");
        }
        private static MemoryMappedFile TryOpen(string name)
        {
            try { return MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.ReadWrite); }
            catch (FileNotFoundException) { return null; }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine("OpenExisting access denied for '" + name + "': " + ex.Message);
                return null;
            }
        }
        public void Dispose()
        {
            _isHealthy = false;
            try { if (_mmf != null) _mmf.Dispose(); } catch { }
        }
        private SharedReaderStatus sharedStatus;
        private readonly object _ipcLock = new object();

        private void EnsureSharedStatus()
        {
            // Fast path
            if (sharedStatus != null && sharedStatus.IsHealthy) return;

            lock (_ipcLock)
            {
                // Double-check after taking the lock
                if (sharedStatus != null && sharedStatus.IsHealthy) return;

                // Dispose any broken instance
                try { sharedStatus?.Dispose(); } catch { /* ignore */ }
                sharedStatus = null;

                // Create (service/publisher side)
                sharedStatus = CreatePublisher();
            }
        }
        // -------------------------------------------------
        // SERVICE-SIDE WRITE OPERATIONS
        // -------------------------------------------------

        public void UpdateSingleReader(ReaderStatusDto dto)
        {
            EnsureSharedStatus();
            if (dto == null) return;            
            lock (_lock)
            {
                _statusCache[dto.LocationId] = dto;
                WriteToMemory();
            }
        }

        public void RemoveReader(int locationId)
        {
            EnsureSharedStatus();
            lock (_lock)
            {
                if (_statusCache.Remove(locationId))
                {
                    WriteToMemory();
                }
            }
        }

        public void UpdateAllReaders(IEnumerable<ReaderStatusDto> items)
        {
            EnsureSharedStatus();
            if (items == null) return;         
            lock (_lock)
            {
                _statusCache.Clear();
                foreach (var x in items)
                {
                    _statusCache[x.LocationId] = x;
                }
                WriteToMemory();
            }
        }

        // Commit order: payload first, then length as the final "publish"
        private void WriteToMemory()
        {
            GuardIpc(delegate
            {
                string json = JsonConvert.SerializeObject(_statusCache.Values);
                byte[] payload = Encoding.UTF8.GetBytes(json);
                int required = sizeof(int) + payload.Length;

                if (required > MMF_SIZE)
                {
                    throw new InvalidOperationException("Shared payload too large (" + required + " bytes) for MMF_SIZE=" + MMF_SIZE);
                }

                using (var accessor = _mmf.CreateViewAccessor(0, required, MemoryMappedFileAccess.Write))
                {
                    // 1) Write payload at offset 4
                    accessor.WriteArray(sizeof(int), payload, 0, payload.Length);
                    // 2) Publish length as the final commit step
                    accessor.Write(0, payload.Length);
                }
            });
        }

        // -------------------------------------------------
        // MONITOR-SIDE READ OPERATIONS
        // -------------------------------------------------

        public List<ReaderStatusDto> ReadAll()
        {
            return GuardIpc<List<ReaderStatusDto>>(delegate
            {
                using (var accessor = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
                {
                    int length = accessor.ReadInt32(0);
                    if (length <= 0 || length > MMF_SIZE - sizeof(int))
                    {
                        return new List<ReaderStatusDto>();
                    }

                    var buffer = new byte[length];
                    accessor.ReadArray(sizeof(int), buffer, 0, length);

                    string json = Encoding.UTF8.GetString(buffer);
                    var list = JsonConvert.DeserializeObject<List<ReaderStatusDto>>(json);
                    if (list == null) list = new List<ReaderStatusDto>();
                    return list;
                }
            });
        }

        // -------------------------------------------------
        // Resilience helpers
        // -------------------------------------------------

        private T GuardIpc<T>(Func<T> body)
        {
            try
            {
                return body();
            }
            catch (ObjectDisposedException)
            {
                _isHealthy = false; throw;
            }
            catch (IOException)
            {
                _isHealthy = false; throw;
            }
            catch (UnauthorizedAccessException)
            {
                _isHealthy = false; throw;
            }
        }

        private void GuardIpc(Action body)
        {
            GuardIpc<object>(delegate
            {
                body();
                return null;
            });
        }
    }
}