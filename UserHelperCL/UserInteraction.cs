using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserHelperCL.Database;
using UserHelperCL.Models;
using UserHelperCL.UserFunctions;
using System.Data.Entity;
using iTextSharp.text;
using System.IO;

namespace UserHelperCL
{
    public class UserInteraction : IDisposable
    {
        private IISManagement iis = new IISManagement();
        private SQLManagement sql = new SQLManagement();
        private PrincipalContext context = new PrincipalContext(ContextType.Machine);
        private StudentsModel db = new StudentsModel();


        public UserInteraction()
        {
            BatchState.State = UserProcessState.INITIAL;
        }

        public PrincipalContext GetPrincipalContext()
        {
            return context;
        }

        public StudentsModel GetDbContext()
        {
            return db;
        }

        public Student DeleteUserWebsiteFTPDatabase(int studentId)
        {
            try
            {
                Student student = db.Students.Find(studentId);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, student.Credentials.FTPUserName);
                UserManagement mngtmnt = new UserManagement(user);
                
                mngtmnt.RemoveWindowsAccount(user);
                string physicalPath = "C:\\inetpub\\wwwroot\\" + student.Credentials.FTPUserName + "\\";
                iis.RemoveWebsite(student.Credentials.FTPUserName, physicalPath);
                iis.RemoveVirtualDirectory("_FTP", "/" + student.Credentials.FTPUserName, physicalPath);

                sql.DeleteLoginAndDB(student.Credentials.FTPUserName, student.Credentials.FTPUserName);

               
                db.Students.Remove(student);
                db.Credentials.Remove(db.Credentials.Find(student.CredentialsId));
                return student;

            }
            catch (Exception)
            {

                throw;
            }
        }




        public PrincipalSearchResult<Principal> GetUserGroups()
        {
            GroupPrincipal group = new GroupPrincipal(context);
            PrincipalSearcher src = new PrincipalSearcher(group);

            PrincipalSearchResult<Principal> groups = src.FindAll();
       
            return groups;
        }
        private void DeleteStudentAndCredentials(int studentId)
        {
            Student s = new Student { StudentId = studentId };
            try
            { 
                s = DeleteUserWebsiteFTPDatabase(studentId);
                db.SaveChanges();
                  
            }
            catch (Exception)
            {
                RollbackOnError(BatchState.State, s.Credentials.FTPUserName);
                throw;
            }
        }

        public void DeleteTeam(int teamId)
        {
            try
            {
               
                    //Find oprettede brugere på det angivne hold, der skal laves en ToList, da DeleteUserWebsiteFTPDatabase ellers ændrer i collectionen, der køres foreach over(må man ikke )
                var t = db.Teams.Find(teamId);
                if (t != null)
                {

               
                    var students = db.Teams.Find(t.TeamId).Students.ToList();
                    //Sletter Windows bruger, IIS Website, IIS FTP (VirtualDirectory), Database, samt databaseserver login, for hver funden bruger.
                    foreach (var item in students)
                    {
                        DeleteUserWebsiteFTPDatabase(item.StudentId);
                    }

                    //sletter herefter holdet i databasen, og gemmer ændringerne til databasen.
                    db.Teams.Remove(t);
                    db.SaveChanges();

                    //opdaterer GUI, så det fjernede hold ikke længere er der.
                    //cbTeam.Items.Remove(t);
                    //cbTeamMany.Items.Remove(t);
                    //lbExistingTeams.Items.Remove(t);
                    //comboCredentialsTeam.Items.Remove(t);

                    BatchState.State = UserProcessState.INITIAL;
                    //done
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public MemoryStream CreatePDF(Team teamToPrint)
        {
         
                try
                {
                    return PDFCreator.CreatePDF(db.Students.Include("Team").Include("Credentials").Where(x => x.Team.TeamId == teamToPrint.TeamId).ToArray());
                    
                }
                catch (Exception)
                {
                    throw;
                }

            
        }

        public Team CreateTeam(string teamName, DateTime startDate, DateTime endDate)
        {
            try
            {
                Team t = db.Teams.Add(new Team { Name = teamName, StartDate = startDate, EndDate = endDate, CreatedDate = DateTime.Now });
                db.SaveChanges();
                return t;
                //update checkboxes and lists
                //lbExistingTeams.Items.Add(t);
                //cbTeam.Items.Add(t);
                //cbTeamMany.Items.Add(t);
                //comboCredentialsTeam.Items.Add(t);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void RollbackOnError(UserProcessState state, string userName)
        {

            switch (state)
            {

                case UserProcessState.WIN_USER_ERROR:
                    //Hvis brugeroprettelse er færdig, stop her (der kan være nogel oprettede brugere i batch oprettelse)
                    break;
                case UserProcessState.WIN_GROUP_ERROR:
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
                        UserManagement management = new UserManagement(user);
                        management.RemoveWindowsAccount(user);
                        //textBox2.Text += Enum.GetName(typeof(UserProcessState), BatchState.State) + "\n";
                        break;
                    }
                case UserProcessState.IIS_WEBSITE_ERROR:
                    {

                        break;
                    }
                case UserProcessState.VIRTUAL_DIRECTORY_ERROR:
                    {
                        //Hvis virtual directory oprettelse fejler, slet bruger og website
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
                        UserManagement management = new UserManagement(user);
                        string physicalPath = "C:\\inetpub\\wwwroot\\" + userName + "\\";
                        iis.RemoveWebsite(userName, physicalPath);

                    }

                    break;
                case UserProcessState.SQL_LOGIN_ERROR:
                    {
                        //hvis sql login fejler, slet vdir, website,bruger
                        Student student = db.Students.Where(x => x.Credentials.FTPUserName == userName).Single();
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, student.Credentials.FTPUserName);
                        UserManagement management = new UserManagement(user);
                        string physicalPath = "C:\\inetpub\\wwwroot\\" + student.Credentials.FTPUserName + "\\";
                        iis.RemoveWebsite(student.Credentials.FTPUserName, physicalPath);
                        iis.RemoveVirtualDirectory("_FTP", "/" + student.Credentials.FTPUserName, physicalPath);
                    }

                    break;
                case UserProcessState.SQL_DB_ERROR:
                    {
                        //hvis dboprettelse fejler, slet dblogin,vdir,website og bruger
                        Student student = db.Students.Where(x => x.Credentials.FTPUserName == userName).Single();
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, student.Credentials.FTPUserName);
                        UserManagement management = new UserManagement(user);
                        string physicalPath = "C:\\inetpub\\wwwroot\\" + student.Credentials.FTPUserName + "\\";
                        iis.RemoveWebsite(student.Credentials.FTPUserName, physicalPath);
                        iis.RemoveVirtualDirectory("_FTP", "/" + student.Credentials.FTPUserName, physicalPath);
                    }
                    break;
                case UserProcessState.SQL_USER_ERROR:
                    {
                        //hvis dboprettelse fejler, slet alt for denne bruger
                        Student student = db.Students.Where(x => x.Credentials.FTPUserName == userName).Single();
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, student.Credentials.FTPUserName);
                        UserManagement management = new UserManagement(user);
                        string physicalPath = "C:\\inetpub\\wwwroot\\" + student.Credentials.FTPUserName + "\\";
                        iis.RemoveWebsite(student.Credentials.FTPUserName, physicalPath);
                        iis.RemoveVirtualDirectory("_FTP", "/" + student.Credentials.FTPUserName, physicalPath);
                        sql.DeleteLoginAndDB(student.Credentials.FTPUserName, student.Credentials.FTPUserName);
                        break;
                    }
                case UserProcessState.SQL_INSERT_USER_DATA_ERROR:
                    {
                        //hvis dboprettelse fejler, slet alt for denne bruger
                      
                        //UserPrincipal user = UserPrincipal.FindByIdentity(context, student.Credentials.FTPUserName);
                        //UserManagement management = new UserManagement(user);
                        //string physicalPath = "C:\\inetpub\\wwwroot\\" + student.Credentials.FTPUserName + "\\";
                        //iis.RemoveWebsite(student.Credentials.FTPUserName, physicalPath);
                        //iis.RemoveVirtualDirectory("_FTP", "/" + student.Credentials.FTPUserName, physicalPath);
                        //sql.DeleteLoginAndDB(student.Credentials.FTPUserName, student.Credentials.FTPUserName);
                        break;
                    }


            }




        }
        public void CreateSingleUser(string userName, string password, int port, string name, int teamId, string userGroupName, bool disablepwchange, bool pwneverexpires)
        {
            
                UserPrincipal user = new UserPrincipal(context);
                GroupPrincipal group = GroupPrincipal.FindByIdentity(context, userGroupName);
                Repository rep = new Repository();
                UserManagement management = new UserManagement(user, group);
                string username = userName.Replace(" ", "");
                string description = "Bruger oprettet med UserHelper";
                string physicalPath = "C:\\inetpub\\wwwroot\\" + username + "\\";
                try
                {
                    //Create Windows User
                    management.CreateLocalWindowsAccount(username, password, username, description, disablepwchange, pwneverexpires, user);
                    management.AddUserToGroup(group, user);
                    //Create IIS Website
                    iis.CreateWebsite(username, "DefaultAppPool", "*:" + port + ":", physicalPath);

                    //Create FTP Virtual Directory
                    //txtStatusMessages.Text += iis.CreateFTPVDir("localhost", username, physicalPath, username);
                    iis.CreateVirtualDirectory("_FTP", username, physicalPath);


                    //Create database for user
                    sql.CreateSQLLoginUserAndDatabase(username, username, password);

                  
                    

                    Credentials cred = new Credentials();
                    cred.DatabaseUserName = username;
                    cred.DatabasePassword = password;
                    cred.FTPUserName = username;
                    cred.FTPPassword = password;
                    cred.WebsitePort = port;
                    cred.WindowsUserGroupName = group.Name;
                    Student student = new Student();
                    student.Name = name;
                    sql.InsertUserWithCredentialsOnTeam(student,cred,teamId);



                    BatchState.State = UserProcessState.INITIAL;
                    //done


                }
                catch (Exception)
                {
                    RollbackOnError(BatchState.State, username);
                    throw;
                }
           
        }
        public void CreateMany(string userNamePrefix, int usernameSuffix, int teamId, string password, int port, string userGroupName, string userNames, bool disablepwchange, bool pwneverexpires)
        {

                 GroupPrincipal group = GroupPrincipal.FindByIdentity(context, userGroupName);


                string[] studentNames = userNames.Replace(Environment.NewLine, "").Split(',').Select(x => x.Trim()).ToArray();
                string usernamePrefix = userNamePrefix.Replace(" ", "");
                string username = usernamePrefix + usernameSuffix;
                string description = "Bruger oprettet med UserHelper";
                string physicalPath = "C:\\inetpub\\wwwroot\\" + username + "\\";
                try
                {
                    for (int i = 0; i < studentNames.Length; i++)
                    {
                        UserPrincipal user = new UserPrincipal(context);
                        UserManagement management = new UserManagement(user, group);
                        //Create Windows User
                        management.CreateLocalWindowsAccount(username, password, username, description, disablepwchange, pwneverexpires, user);
                        management.AddUserToGroup(group, user);
                        //Create IIS Website
                        iis.CreateWebsite(username, "DefaultAppPool", "*:" + port + ":", physicalPath);


                        //Create FTP Virtual Directory
                        //txtStatusMessages.Text += iis.CreateFTPVDir("localhost", username, physicalPath, username);
                        iis.CreateVirtualDirectory("_FTP", username, physicalPath);


                        //create databases
                        sql.CreateSQLLoginUserAndDatabase(username, username, password);



                        Credentials cred = new Credentials();
                        cred.DatabaseUserName = username;
                        cred.DatabasePassword = password;
                        cred.FTPUserName = username;
                        cred.FTPPassword = password;
                        cred.WebsitePort = port;
                        cred.WindowsUserGroupName = group.Name;

                        Student student = new Student();
                        student.Name = studentNames[i];
                        student.Team = db.Teams.Find(teamId);
                        student.Credentials = cred;
                        db.Students.Add(student);

                        //Change username and port for next iteration
                        usernameSuffix++;
                        username = usernamePrefix + usernameSuffix;
                        physicalPath = "C:\\inetpub\\wwwroot\\" + username + "\\";
                        port++;


                    }

                    db.SaveChanges();
          
                    BatchState.State = UserProcessState.INITIAL;
                    //done
                }
                catch (Exception)
                {
                    throw;
                }
            }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null) db.Dispose();
                if (context != null) context.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }





    }
}
