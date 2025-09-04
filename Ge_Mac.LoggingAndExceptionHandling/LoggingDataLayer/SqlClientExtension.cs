using System.Data;
using System.Data.SqlClient;

namespace Ge_Mac.LoggingDataLayer
{
    public static class SqlClientExtension
    {
        #region No Resultset
        /// <summary>Execute a sql statement that returns no data</summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        public static int ExecuteNonQuery(this SqlCommand command, SqlDataConnection.DBConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }

            command.CommandTimeout = SqlDataConnection.Timeout;

            bool close = false;

            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                close = true;
            }

            int recordsAffected = 0;

            try
            {
                recordsAffected = command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (close)
                {
                    command.Connection.Close();
                }
            }

            return recordsAffected;
        }

        /// <summary>
        /// Execute a sql statement that returns a single value
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">the connection to use</param>
        /// <returns></returns>
        public static object ExecuteScalar(this SqlCommand command, SqlDataConnection.DBConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }
            command.CommandTimeout = SqlDataConnection.Timeout;

            bool close = false;

            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                close = true;
            }

            object scalarValue = null;

            try
            {
                scalarValue = command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (close)
                {
                    command.Connection.Close();
                }
            }

            return scalarValue;
        }
        #endregion
    }
}
