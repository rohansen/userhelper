using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserHelperCL.Models;

namespace UserHelperCL.UserFunctions
{
    public class UserManagement
    {
        public GroupPrincipal Group { get; set; }
        public UserPrincipal User { get; set; }
        public UserManagement(UserPrincipal user, GroupPrincipal group)
        {
          
            Group = group;
            User = user;
        }
        public UserManagement(UserPrincipal user)
        {
           
            User = user;
        }


        public void CreateLocalWindowsAccount(string username, string password, string displayName, string description, bool disablePWChange, bool pwdNeverExpires, UserPrincipal user)
        {
            try
            {
                user.SetPassword(password);
                user.DisplayName = displayName;
                user.Name = username;
                user.Description = description;
                user.UserCannotChangePassword = disablePWChange;
                user.PasswordNeverExpires = pwdNeverExpires;
                user.Save();
                BatchState.State = UserProcessState.WIN_USER_OK;
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.WIN_USER_ERROR;
                throw;
            }
                
          
        }

        public void AddUserToGroup(GroupPrincipal group, UserPrincipal user)
        {
            //now add user to "Users" group so it displays in Control Panel

            try
            {
                group.Members.Add(user);
                group.Save();
                BatchState.State = UserProcessState.WIN_GROUP_OK;
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.WIN_GROUP_ERROR;
                throw;
            }
            
        }

        public void RemoveWindowsAccount(UserPrincipal user)
        {
            try
            {
                user.Delete();
                BatchState.State = UserProcessState.WIN_DELETE_OK;
            }
            catch (Exception)
            {
                BatchState.State = UserProcessState.WIN_DELETE_ERROR;
                throw;
            }
           
        }
    }
}
