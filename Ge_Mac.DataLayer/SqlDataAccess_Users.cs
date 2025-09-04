using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region All Users Command
        const string allUsersCommand =
            @"SELECT [RecNum]
                   , [LoginName]
                   , [Department]
                   , [EncPassword]
                   , [LogoutTime]
                   , [Retired]
              FROM [dbo].[tblUsers]";
        #endregion
        
        public Users GetAllUsers()
        {
            try
            {
                const string commandString = allUsersCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Users users = new Users();
                    users.Add(new User()
                    {
                        UserID = 0,
                        LoginName = "No User Selected",
                        EncPassword = "",
                        Department = "",
                        LogoutTime = 0,
                        Retired = false,
                        LoggedIn = false
                    }
                    );
                    command.DataFill(users, SqlDataConnection.DBConnection.Rail);
                    return users;
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

        public User GetUser(int userId)
        {
            try
            {
                const string commandString = allUsersCommand
                    + " WHERE RecNum = @UserID";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    Users users = new Users();

                    command.DataFill(users, SqlDataConnection.DBConnection.Rail);
                    if (users.Count > 0)
                    {
                        return users[0];
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

        public User GetUser(string loginName)
        {
            try
            {
                const string commandString = allUsersCommand
                    + " WHERE LoginName = @LoginName";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@LoginName", loginName);
                    Users users = new Users();

                    command.DataFill(users, SqlDataConnection.DBConnection.Rail);
                    if (users.Count > 0)
                    {
                        return users[0];
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

        public bool CheckUserAccess(User user, int systemID, string requiredAccess)
        {
            try
            {
                const string commandString = "dbo.spCheckUserRights";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@SystemID", systemID);
                    command.Parameters.AddWithValue("@Access", requiredAccess);
                    SqlParameter permit = command.Parameters.Add("@Permit", SqlDbType.Bit);
                    permit.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);

                    if (permit.Value != null)
                    {
                        return (bool)permit.Value;
                    }
                    else
                    {
                        return false;
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

        public void UpdateUserDetails(User user)
        {
            const string commandString =
                @"UPDATE [dbo].tblUsers
                  SET LoginName = @LoginName
                    , EncPassword = @EncPassword
                    , Department = @Department
                    , LogoutTime = @LogoutTime
                    , Retired = @Retired
                  WHERE RecNum = @UserID";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@UserID", user.UserID);
                command.Parameters.AddWithValue("@LoginName", user.LoginName);
                command.Parameters.AddWithValue("@EncPassword", user.EncPassword);
                command.Parameters.AddWithValue("@Department", user.Department);
                command.Parameters.AddWithValue("@LogoutTime", user.LogoutTime);
                command.Parameters.AddWithValue("@Retired", user.Retired);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
            }
        }
    }

    public class Users : List<User>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int LoginNamePos = dr.GetOrdinal("LoginName");
            int EncPasswordPos = dr.GetOrdinal("EncPassword");
            int DepartmentPos = dr.GetOrdinal("Department");
            int LogoutTimePos = dr.GetOrdinal("LogoutTime");
            int RetiredPos = dr.GetOrdinal("Retired"); 
            
            while (dr.Read())
            {
                User customer = new User()
                {
                    UserID = dr.GetInt32(RecNumPos),
                    LoginName = dr.GetString(LoginNamePos),
                    EncPassword = Convert.ToString(dr.GetSqlBinary(EncPasswordPos)),
                    Department = dr.GetString(DepartmentPos),
                    LogoutTime = dr.GetInt32(LogoutTimePos),
                    Retired = dr.GetBoolean(RetiredPos)
                };

                this.Add(customer);
            }

            return this.Count;
        }

        public User GetById(int id)
        {
            return this.Find(delegate(User user)
            {
                return user.UserID == id;
            });
        }
    }

    #region User Class
    public class User
    {
        /// <summary>UserId / RecNum</summary>
        public int UserID         { get; set; }
        public string LoginName   { get; set; }
        // EncPassword = VariantType
        public string EncPassword { get; set; }
        public string Department  { get; set; }
        public int LogoutTime     { get; set; }
        public bool Retired       { get; set; }
        public bool LoggedIn      { get; set; }

        public void Logout()
        {
            UserID = 0;
            LoginName = "";
            Department = "";
            LogoutTime = 0;
            LoggedIn = false;
        }

    }
    #endregion
}
