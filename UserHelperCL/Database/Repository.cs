using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserHelperCL.Models;

namespace UserHelperCL.Database
{
    public class Repository
    {
        
        public void CreateTeam(string name, DateTime startDate, DateTime endDate)
        {
            using (StudentsModel db = new StudentsModel())
            {
                db.Teams.Add(new Models.Team { Name = name, StartDate = startDate, EndDate = endDate, CreatedDate = DateTime.Now });
                db.SaveChanges();
            }
        }
        public List<Team> GetAllTeams()
        {
            using (StudentsModel db = new StudentsModel())
            {
                return db.Teams.ToList();
            }
        }

        public void InsertStudent(Student student)
        {
            using (StudentsModel db = new StudentsModel())
            {
                db.Students.Add(student);
                db.SaveChanges();
            }
        }
       

    }
}
