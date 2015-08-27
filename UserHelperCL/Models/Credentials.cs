using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserHelperCL.Models
{
    public class Credentials
    {
        
        public int CredentialsId { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public int WebsitePort { get; set; }
        public string WindowsUserGroupName { get; set; }
        public bool DisablePasswordChange { get; set; }
        public bool PasswordNeverExpires { get; set; }
       



    }
}
