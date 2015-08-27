using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserHelper.Models;


namespace UserHelper.UserFunctions
{
    public class IISManagement
    {
     
        public ServerManager ServerMngr { get; set; }
        public IISManagement()
        {
         
            ServerMngr = new ServerManager();
        }


        /// <summary>
        /// Creates a website on the webserver and creates a directory for the website
        /// </summary>
        /// <param name="websiteName">website name as a string</param>
        /// <param name="applicationPoolName">application pool name as a string</param>
        /// <param name="bindingInfo">the binding string</param>
        /// <param name="physicalPath">physical path for the website as a string</param>
        /// <returns></returns>
        public void CreateWebsite(string websiteName, string applicationPoolName, string bindingInfo, string physicalPath)
        {
            try
            {
                Boolean bWebsite = IsWebsiteExists(websiteName);
                if (!bWebsite)
                {
                    Site mySite = ServerMngr.Sites.Add(websiteName.ToString(), "http", bindingInfo, physicalPath);
                    mySite.ApplicationDefaults.ApplicationPoolName = applicationPoolName;
                    mySite.TraceFailedRequestsLogging.Enabled = true;
                    mySite.TraceFailedRequestsLogging.Directory = "C:\\inetpub\\customfolder\\site";
                    if (!Directory.Exists(physicalPath))
                    {
                        Directory.CreateDirectory(physicalPath);
                    }
                    ServerMngr.CommitChanges();
                    BatchState.State = UserProcessState.IIS_WEBSITE_OK;
                }
                else
                {
                    //website exists already
                }
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.IIS_WEBSITE_ERROR;
                throw ex;
            }


        }

        public void RemoveWebsite(string websiteName, string physicalPath)
        {
            try
            {
                Boolean bWebsite = IsWebsiteExists(websiteName);
                if (bWebsite)
                {
                    Site mySite = ServerMngr.Sites[websiteName];
                    ServerMngr.Sites.Remove(mySite);
                    if (Directory.Exists(physicalPath))
                    {
                        Directory.Delete(physicalPath, true);
                    }

                    ServerMngr.CommitChanges();
                    BatchState.State = UserProcessState.IIS_DELETE_WEBSITE_OK;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.IIS_DELETE_WEBSITE_ERROR;
                throw ex;
            }
        }

      

        /// <summary>
        /// Checks if a website exists on the webserver
        /// </summary>
        /// <param name="strWebsitename">name of the website as a string</param>
        /// <returns></returns>
        public bool IsWebsiteExists(string strWebsitename)
        {
            Boolean flagset = false;
            SiteCollection sitecollection = ServerMngr.Sites;
            foreach (Site site in sitecollection)
            {
                if (site.Name == strWebsitename.ToString())
                {
                    flagset = true;
                    break;
                }
                else
                {
                    flagset = false;
                }
            }
            return flagset;
        }

        /// <summary>
        /// Sitename == _FTP
        /// Appname = brugernavnet (navnet på virt directory)
        /// </summary>
        /// <param name="siteName"></param>
        public void CreateVirtualDirectory(string siteName, string appName, string physicalPath)
        {
            try
            {
                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }
                /*Create Authoorization rule */
                Configuration config = ServerMngr.GetApplicationHostConfiguration();
                ConfigurationSection authorizationSection = config.GetSection("system.ftpServer/security/authorization", siteName + "/" + appName);
                ConfigurationElementCollection authorizationCollection = authorizationSection.GetCollection();


                authorizationCollection.Clear();
                ConfigurationElement addElement = authorizationCollection.CreateElement("add");
                addElement["accessType"] = @"Allow";
                addElement["users"] = appName;
                addElement["permissions"] = @"Read, Write";
                authorizationCollection.Add(addElement);

                var vd = ServerMngr.Sites[siteName].Applications["/"].VirtualDirectories;
                vd.Add("/" + appName, physicalPath);
                ServerMngr.CommitChanges();
                BatchState.State = UserProcessState.VIRTUAL_DIRECTORY_OK;
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.VIRTUAL_DIRECTORY_ERROR;
                throw ex;
            }

        }
        /// <summary>
        /// Sitename == _FTP
        /// userName = brugernavnet (navnet på virt directory)
        /// </summary>
        /// <param name="siteName"></param>
        public void RemoveVirtualDirectory(string siteName, string userName, string physicalPath)
        {
            try
            {
                if (Directory.Exists(physicalPath))
                {
                    Directory.Delete(physicalPath);
                }

                var vdCollection = ServerMngr.Sites[siteName].Applications["/"].VirtualDirectories;
                var vd = ServerMngr.Sites[siteName].Applications["/"].VirtualDirectories.Where(x => x.Path == userName).Single();
                vdCollection.Remove(vd);
                ServerMngr.CommitChanges();
                BatchState.State = UserProcessState.VIRTUAL_DIRECTORY_REMOVE_OK;
            }
            catch (Exception ex)
            {
                BatchState.State = UserProcessState.VIRTUAL_DIRECTORY_REMOVE_ERROR;
                
                throw ex;
            }

        }


    }
}
