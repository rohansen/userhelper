using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserHelper.Database;
using System.Data.Entity;
using UserHelper.Models;

namespace UserHelper.UserFunctions
{
    public class SQLManagement
    {
        private StudentsModel ctx = new StudentsModel();

        public SQLManagement()
        {
            
        }

        /// <summary>
        /// Remember to dispose after method call!
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void CreateSQLLoginUserAndDatabase(string databaseName, string username, string password)
        {
            //Create database må ikke laves sammen med flere statements i transaktion, transaktioner disables
            //da nogle TSQL commands ikke kan kædes sammen i transaktioner
            //Disse commands understøtter heller ikek parametiserede queries
            //Husk at dispose context efter brug af metode
            CreateDatabase(databaseName);
          
            CreateDBServerLogin(username, password);
        
            CreateDBUser(databaseName, username);
         
            AssignRoleToUser(databaseName, username);
          

        }

        public void DeleteLoginAndDB(string databaseName, string username)
        {
            DeleteDatabase(databaseName);
            DeleteDBServerLogin(username);
        }
        public void CreateDatabase(string databaseName)
        {
            try
            {
                string cmd = "CREATE DATABASE [" + databaseName + "]";
                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
                BatchState.State = UserProcessState.SQL_DB_OK;
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.SQL_DB_ERROR;
                throw ex;
            }
        }

        public void DeleteDatabase(string databaseName)
        {
            try
            {
                string cmd = "DROP DATABASE [" + databaseName + "]";
                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
                BatchState.State = UserProcessState.SQL_DELETE_DB_OK;

            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.SQL_DELETE_DB_ERROR;
                throw ex;
            }
        }

        public void DeleteDBServerLogin(string userName)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "DROP LOGIN " + userName);
                BatchState.State = UserProcessState.SQL_LOGIN_DELETE_OK;
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.SQL_LOGIN_DELETE_ERROR;
                throw ex;
            }
        }




        public void CreateDBServerLogin(string username, string password)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "CREATE LOGIN " + username + " WITH PASSWORD = '" + password + "'");
                BatchState.State = UserProcessState.SQL_LOGIN_OK;
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.SQL_LOGIN_ERROR;
                throw ex;
            }
        }
        public void CreateDBUser(string databaseName, string username)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand("use " + databaseName + "; CREATE USER " + username + " FOR LOGIN " + username);
                BatchState.State = UserProcessState.SQL_USER_OK;
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.SQL_USER_ERROR;
                throw ex;
            }
        }
        public void AssignRoleToUser(string databaseName, string username)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "use " + databaseName + "; exec sp_addrolemember 'db_owner', " + username);
                BatchState.State = UserProcessState.SQL_ROLE_OK;
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.SQL_ROLE_ERROR;
                throw ex;
            }
        }

        

        public void CloseDbConnections()
        {

            ctx.Dispose();

        }
    }
}
