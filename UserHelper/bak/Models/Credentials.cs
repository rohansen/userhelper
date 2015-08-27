using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserHelper.Models
{
    public class Credentials
    {
        
        public int CredentialsId { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public int WebsitePort { get; set; }




    }
}
