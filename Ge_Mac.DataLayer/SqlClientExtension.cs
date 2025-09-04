using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Ge_Mac.DataLayer
{
    #region fieldexists
    public static class DataRecordExts
    {
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            } 
            return false;
        }

        public static int ColumnIndex(this IDataRecord dr, string columnName)
        {
            int columnIndex = -1;
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    columnIndex=i;
            }
            return columnIndex;
        }

    }
    #endregion

    public static class SqlClientExtension
    {
        #region DataTable
        /// <summary>Read data into a datatable</summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        /// <returns>The filled datatable</returns>
        public static DataTable ReadTable(this SqlCommand command, SqlDataConnection.DBConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }

            command.CommandTimeout = SqlDataConnection.Timeout;

            DataTable dataTable;

            using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
            {
                dataTable = new DataTable();
                dbAdapter.Fill(dataTable);
            }

            return dataTable;
        }

        /// <summary>Read data into a datatable</summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        /// <returns>The filled datatable</returns>
        public static int Fill(this SqlDataAdapter adapter, DataSet dataSet, string tableName, SqlDataConnection.DBConnection dBConnection)
        {
            SqlCommand command = adapter.SelectCommand;
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }
            command.CommandTimeout = SqlDataConnection.Timeout;

            int records = 0;

            using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
            {
                records = dbAdapter.Fill(dataSet, tableName);
            }

            return records;
        }

        /// <summary>
        /// Read Xml data into a class collection
        /// </summary>
        /// <param name="dc">The class to fill</param>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">the connection to use</param>
        public static void XmlDataFill(this SqlCommand command, IXmlDataFiller dc, SqlDataConnection.DBConnection dBConnection)
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

            try
            {
                using (XmlReader dr = command.ExecuteXmlReader())
                {
                    dc.FillXml(dr);
                }
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
        }
        #endregion

        #region Reader
        /// <summary>
        /// Opens the specified connection and executes the reader.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="dBConnection">The Ge-Mac Connection Type</param>
        /// <returns>The Open Reader, ready to Read</returns>
        public static SqlDataReader ExecuteReader(this SqlCommand command, SqlDataConnection.DBConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }
            command.CommandTimeout = SqlDataConnection.Timeout;

            SqlDataReader reader;

            command.Connection.Open();

            using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
            {
                reader = command.ExecuteReader();
            }

            return reader;
        }

        /// <summary>
        /// Opens the specified connection and executes the reader.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="behaviour">The Command Behavior</param>
        /// <param name="dBConnection">The Ge-Mac Connection Type</param>
        /// <returns>The Open Reader, ready to Read</returns>
        public static SqlDataReader ExecuteReader(this SqlCommand command, CommandBehavior behaviour, 
            SqlDataConnection.DBConnection dBConnection, int timeoutSeconds)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }
            if (timeoutSeconds <= 0)
            { command.CommandTimeout = SqlDataConnection.Timeout; }
            else { command.CommandTimeout = timeoutSeconds; }

            SqlDataReader reader;

            command.Connection.Open();

            using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
            {
                reader = command.ExecuteReader(behaviour);
            }

            return reader;
        }

        public static SqlDataReader ExecuteReader(this SqlCommand command, CommandBehavior behaviour, SqlDataConnection.DBConnection dBConnection)
        {

            SqlDataReader reader = ExecuteReader(command, behaviour, dBConnection, -1);

            return reader;
        }
        #endregion

        #region Collection
        /// <summary>Read data into a class collection</summary>
        /// <param name="dc">The class to fill</param>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        public static int DataFill(this SqlCommand command, IDataFiller dc, SqlDataConnection.DBConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }
            command.CommandTimeout = SqlDataConnection.Timeout;

            int recordsRead;

            // Open the connection if necessary
            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    recordsRead = dc.Fill(dr);
                }
            }
            else
            {
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    recordsRead = dc.Fill(dr);
                }
            }

            return recordsRead;
        }


        #endregion

        #region Single Class
        /// <summary>Read data into a single class</summary>
        /// <param name="dc">The class to fill</param>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        public static bool DataFillSingle(this SqlCommand command, IDataFillerSingle dc, SqlDataConnection.DBConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = SqlDataConnection.GetConnection(dBConnection);
            }
            command.CommandTimeout = SqlDataConnection.Timeout;

            int recordsRead;

            // Open the connection if necessary
            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    recordsRead = dc.FillSingle(dr);
                }
            }
            else
            {
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    recordsRead = dc.FillSingle(dr);
                }
            }

            return recordsRead > 0;
        }
        #endregion

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
