using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

class Program
{
    static bool _verbose = false;
    static bool _running = true;

    static async Task Main(string[] args)
    {
        int port = 5500;
        if (args.Length > 0 && int.TryParse(args[0], out var p))
        {
            port = p;
        }

        Console.WriteLine($"Card reader emulator starting on port {port}");
        Console.WriteLine("Commands:");
        Console.WriteLine("  [hex card ID] - Queue card read (e.g., '7C339600' or '0x7C339600')");
        Console.WriteLine("  'verbose'     - Toggle debug output");
        Console.WriteLine("  'quit'        - Exit emulator");
        Console.WriteLine();

        var cards = new ConcurrentQueue<string>();

        // Console input handler
        _ = Task.Run(() =>
        {
            while (_running)
            {
                try
                {
                    var line = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    line = line.Trim();

                    if (line.Equals("verbose", StringComparison.OrdinalIgnoreCase))
                    {
                        _verbose = !_verbose;
                        Console.WriteLine($">>> Verbose logging: {(_verbose ? "ON" : "OFF")}");
                        continue;
                    }

                    if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine(">>> Shutting down...");
                        _running = false;
                        break;
                    }

                    if (line.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        line = line.Substring(2);

                    cards.Enqueue(line.ToUpperInvariant());
                    Console.WriteLine($">>> Queued card: {line}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">>> Input handler error: {ex.Message}");
                }
            }
        });

        // Main server loop with reconnection
        while (_running)
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                Console.WriteLine($">>> Listening on port {port}...");

                while (_running)
                {
                    // Wait for client with cancellation support
                    var acceptTask = listener.AcceptTcpClientAsync();
                    var delayTask = Task.Delay(500); // Check _running flag every 500ms
                    var completedTask = await Task.WhenAny(acceptTask, delayTask);

                    if (completedTask == delayTask)
                    {
                        // Timeout - check if we should continue
                        continue;
                    }

                    using var client = await acceptTask;
                    Console.WriteLine(">>> Client CONNECTED");

                    try
                    {
                        await HandleClient(client, cards);
                    }
                    catch (IOException ioEx) when (ioEx.InnerException is SocketException)
                    {
                        Console.WriteLine(">>> Client DISCONNECTED (connection closed by remote host)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($">>> Client error: {ex.GetType().Name}: {ex.Message}");
                    }

                    Console.WriteLine(">>> Ready for next connection...");
                }
            }
            catch (SocketException sockEx)
            {
                Console.WriteLine($">>> Socket error: {sockEx.Message}");
                Console.WriteLine(">>> Retrying in 2 seconds...");
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Unexpected error: {ex.GetType().Name}: {ex.Message}");
                await Task.Delay(2000);
            }
            finally
            {
                listener?.Stop();
            }
        }

        Console.WriteLine(">>> Emulator stopped");
    }

    static async Task HandleClient(TcpClient client, ConcurrentQueue<string> cards)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.ASCII);
        using var writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };

        while (client.Connected && _running)
        {
            string cmd;
            try
            {
                cmd = await reader.ReadLineAsync();
                if (cmd == null)
                    break; // Connection closed gracefully
            }
            catch (IOException)
            {
                // Connection lost while reading
                throw;
            }

            if (_verbose)
                Console.WriteLine($"RX: {cmd}");

            try
            {
                if (cmd.StartsWith("rfid:qid.id", StringComparison.OrdinalIgnoreCase))
                {
                    if (!cards.TryDequeue(out var card))
                    {
                        await writer.WriteLineAsync("{0x0000,0,0x00,0;0x00}");
                    }
                    else
                    {
                        await writer.WriteLineAsync("{0x0000,0,0x00,0;0x" + card + "}");
                        Console.WriteLine($">>> SENT card: {card}");
                    }
                }
                else if (cmd.StartsWith("rfid:dev.luid?", StringComparison.OrdinalIgnoreCase))
                {
                    await writer.WriteLineAsync("rfid:dev.luid=1");
                    if (_verbose)
                        Console.WriteLine("TX: rfid:dev.luid=1");
                }
                else if (cmd.StartsWith("rfid:out.led=", StringComparison.OrdinalIgnoreCase))
                {
                    // LED command
                    var ledValue = cmd.Substring("rfid:out.led=".Length);
                    if (_verbose)
                        Console.WriteLine($">>> LED set to: {ledValue}");
                    await writer.WriteLineAsync("OK");
                }
                else if (cmd.StartsWith("rfid:cmd.", StringComparison.OrdinalIgnoreCase))
                {
                    // Config commands (echo, prompt, etc.)
                    if (_verbose)
                        Console.WriteLine($">>> Config: {cmd}");
                    await writer.WriteLineAsync("OK");
                }
                else
                {
                    await writer.WriteLineAsync("OK");
                    if (_verbose)
                        Console.WriteLine("TX: OK");
                }
            }
            catch (IOException)
            {
                // Connection lost while writing
                throw;
            }
        }
    }
}