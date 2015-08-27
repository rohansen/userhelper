using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserHelperCL.Database;
using System.Data.Entity;
using UserHelperCL.Models;

namespace UserHelperCL.UserFunctions
{
    public class SQLManagement
    {

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
                using (StudentsModel ctx = new StudentsModel()) { 
                    string cmd = "CREATE DATABASE [" + databaseName + "]";
                    ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
                    BatchState.State = UserProcessState.SQL_DB_OK;
                }
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.SQL_DB_ERROR;
                throw;
            }
        }

        public void DeleteDatabase(string databaseName)
        {
            try
            {
                using (StudentsModel ctx = new StudentsModel())
                {
                    string cmd = "DROP DATABASE [" + databaseName + "]";
                    ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);
                    BatchState.State = UserProcessState.SQL_DELETE_DB_OK;
                }
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.SQL_DELETE_DB_ERROR;
                throw;
            }
        }

        public void DeleteDBServerLogin(string userName)
        {
            try
            {
                using (StudentsModel ctx = new StudentsModel())
                {
                    ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "DROP LOGIN " + userName);
                    BatchState.State = UserProcessState.SQL_LOGIN_DELETE_OK;
                }
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.SQL_LOGIN_DELETE_ERROR;
                throw;
            }
        }




        public void CreateDBServerLogin(string username, string password)
        {
            try
            {
                using (StudentsModel ctx = new StudentsModel())
                {
                    ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "CREATE LOGIN " + username + " WITH PASSWORD = '" + password + "'");
                    BatchState.State = UserProcessState.SQL_LOGIN_OK;
                }
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.SQL_LOGIN_ERROR;
                throw;
            }
        }
        public void CreateDBUser(string databaseName, string username)
        {
            try
            {
                using (StudentsModel ctx = new StudentsModel())
                {
                    ctx.Database.ExecuteSqlCommand("use " + databaseName + "; CREATE USER " + username + " FOR LOGIN " + username);
                    BatchState.State = UserProcessState.SQL_USER_OK;
                }
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.SQL_USER_ERROR;
                throw;
            }
        }
        public void AssignRoleToUser(string databaseName, string username)
        {
            try
            {
                using (StudentsModel ctx = new StudentsModel())
                {
                    ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "use " + databaseName + "; exec sp_addrolemember 'db_owner', " + username);
                    BatchState.State = UserProcessState.SQL_ROLE_OK;
                }
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.SQL_ROLE_ERROR;
                throw;
            }
        }

        public void InsertUserWithCredentialsOnTeam(Student student, Credentials cred, int teamId)
        {


            try
            {
                using (StudentsModel ctx = new StudentsModel())
                {
                    student.Credentials = cred;
                    student.TeamId = teamId;
                    ctx.Students.Add(student);
                    ctx.SaveChanges();
                    BatchState.State = UserProcessState.SQL_INSERT_USER_DATA_OK;
                }
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.SQL_INSERT_USER_DATA_ERROR;
                throw;
            }

        }

     
    }
}
