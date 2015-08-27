using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserHelper.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public virtual Team Team { get; set; }
        public virtual Credentials Credentials { get; set; }
    }
}
