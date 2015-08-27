using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UserHelperCL.Models;

namespace WebInterface.Models
{
    public class CreateManyViewModel
    {

        public string UserNamePrefix { get; set; }
        public int UserNameSuffix { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public string StudentNames { get; set; }
        public int CredentialsId { get; set; }
        public Credentials Credentials { get; set; }
    }
}