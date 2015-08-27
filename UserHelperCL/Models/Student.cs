using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserHelperCL.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        public int CredentialsId { get; set; }
        public virtual Credentials Credentials { get; set; }
    }
}
