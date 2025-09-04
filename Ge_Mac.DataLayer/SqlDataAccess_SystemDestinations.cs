using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region System Destinations
        const string allSystemDestsCommandString =
            @"SELECT [idDest]
                   , [SystemID]
                   , [DestDescription]
                   , [Value]
                   , [Type]
                   , 0 AS Block
              FROM [dbo].[tblSystemDestinations]";

        const string StorageLineCommandString =
            @"SELECT sd.[idDest]
                   , sd.[SystemID]
                   , sd.[DestDescription]
                   , sd.[Value]
                   , sd.[Type]
                   , br.[BlockedState] AS Block
                FROM [dbo].[tblSystemDestinations] sd, dbo.tblBlockedRails br
                WHERE sd.SystemID = br.SystemID
	                AND sd.Value = br.LineID
	                AND sd.Type = 0";

        public enum LineType
        {
            Storage = 0,
            Washer = 1,
            Loop = 2
        }

        public enum DestBlock
        {
            NoBlock = 0,
            OnBlock = 1,
            OffBlock = 2,
            OnOffBlock = 3
        }

        #region Select Data
        public SystemDestinations GetAllSystemDestinations()
        {
            try
            {
                const string commandString = allSystemDestsCommandString;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    SystemDestinations dests = new SystemDestinations();
                    command.DataFill(dests, SqlDataConnection.DBConnection.Rail);
                    return dests;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        public SystemDestinations GetAllSystemDestinations(int systemId)
        {
            try
            {
                const string commandString = allSystemDestsCommandString +
                    @" WHERE SystemID = @SystemId";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SystemId", systemId);

                    SystemDestinations dests = new SystemDestinations();
                    command.DataFill(dests, SqlDataConnection.DBConnection.Rail);
                    return dests;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        public SystemDestinations GetSystemDestinations(int systemId, LineType lineType)
        {
            try
            {
                const string commandString = allSystemDestsCommandString +
                    @" WHERE SystemID = @SystemId
                       AND   [Type] = @LineType";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SystemId", systemId);
                    command.Parameters.AddWithValue("@LineType", lineType);

                    SystemDestinations dests = new SystemDestinations();
                    command.DataFill(dests, SqlDataConnection.DBConnection.Rail);
                    return dests;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        public SystemDestinations GetSystemStorageLines(int systemId)
        {
            try
            {
                const string commandString = StorageLineCommandString +
                    @" AND sd.SystemID=@SystemId";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SystemId", systemId);

                    SystemDestinations dests = new SystemDestinations();
                    command.DataFill(dests, SqlDataConnection.DBConnection.Rail);
                    return dests;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        public SystemDestination GetSystemDestination(int systemId, LineType lineType, int value)
        {
            try
            {
                const string commandString = allSystemDestsCommandString +
                    @" WHERE SystemID = @SystemId
                       AND   [Type] = @LineType
                       AND   [Value] = @Value";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SystemId", systemId);
                    command.Parameters.AddWithValue("@LineType", lineType);
                    command.Parameters.AddWithValue("@Value", value);

                    SystemDestinations dests = new SystemDestinations();
                    command.DataFill(dests, SqlDataConnection.DBConnection.Rail);

                    if (dests.Count > 0)
                    {
                        return dests[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        #endregion

        #region Update Data

        /// <summary>
        /// Updates the database with the current values of destination blocks.
        /// </summary>
        /// <param name="SystemDestination"></param>
        /// <returns>True if some records were updated</returns>
        public Boolean UpdateDestinationBlocks(SystemDestination SystemDestination)
        {
            try
            {
                const string commandString =
                    @"UPDATE RAIL_DB.dbo.tblBlockedRails
                      SET [RequestedBlockState] = @Block
                      WHERE SystemID = @System
                        AND LineID = @Line

                      SELECT @@ROWCOUNT";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@Block", SystemDestination.Block);
                    command.Parameters.AddWithValue("@System", SystemDestination.SystemID);
                    command.Parameters.AddWithValue("@Line", SystemDestination.Value);

                    object RecordCount = command.ExecuteScalar(SqlDataConnection.DBConnection.Rail);
                    return (RecordCount != null && (int)RecordCount > 0);
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        #endregion

        #endregion

    }

    public class SystemDestinations : List<SystemDestination>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int idDestPos = dr.GetOrdinal("idDest");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int DestDescriptionPos = dr.GetOrdinal("DestDescription");
            int ValuePos = dr.GetOrdinal("Value");
            int TypePos = dr.GetOrdinal("Type");
            int BlockPos = dr.GetOrdinal("Block");
            
            while (dr.Read())
            {
                SystemDestination dest = new SystemDestination()
                {
                    idDest = dr.GetInt32(idDestPos),
                    SystemID = dr.GetInt32(SystemIDPos),
                    DestDescription = dr.GetString(DestDescriptionPos),
                    Value = dr.GetInt32(ValuePos),
                    Type = dr.GetInt32(TypePos),
                    Block = dr.GetInt32(BlockPos)
                };

                // Add to sort categories collection
                this.Add(dest);
            }

            return this.Count;
        }

        public new void Sort()
        {
            SystemDestinationComparer comparer = new SystemDestinationComparer();
            base.Sort(comparer);
        }

        public SystemDestination GetByID(int id)
        {
            return this.Find(delegate(SystemDestination systemDestination)
            {
                return systemDestination.idDest == id;
            });
        }
    }

    public class SystemDestinationComparer : IComparer<SystemDestination>
    {
        public int Compare(SystemDestination x, SystemDestination y)
        {
            int valueX = (x != null) ? x.Value + 1 : 0;
            int valueY = (y != null) ? y.Value + 1 : 0;
            return Math.Sign(valueX - valueY);
        }
    }

    #region System Destination Class
    public class SystemDestination
    {
        public int    idDest          { get; set; }
        public int    SystemID        { get; set; }
        public string DestDescription { get; set; }
        public int    Value           { get; set; }
        public int    Type            { get; set; }
        public int    Block           { get; set; }

        public bool IsBlockedOn()
        {
            return (this.Block == 1) || (this.Block == 3);
        }

        public bool IsBlockedOff()
        {
            return (this.Block == 2) || (this.Block == 3);
        }

        public void SetBlockOn()
        {
            if (!this.IsBlockedOff())
            {
                this.Block = 1;
            }
            else
            {
                this.Block = 3;
            }
        }

        public void SetBlockOff()
        {
            if (!this.IsBlockedOn())
            {
                this.Block = 2;
            }
            else
            {
                this.Block = 3;
            }
        }
    }
    #endregion
}
