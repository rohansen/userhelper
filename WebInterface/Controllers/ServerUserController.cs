using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserHelperCL;
using UserHelperCL.Database;
using UserHelperCL.Models;
using UserHelperCL.UserFunctions;
using WebInterface.Models;

namespace WebInterface.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ServerUserController : Controller
    {
        StudentsModel db = new StudentsModel();
        UserInteraction interaction = new UserInteraction();
        // GET: ServerUser
        public ActionResult Index()
        {

            return View(db.Students.Include("Team").Include("Credentials").ToList());
        }


        public ActionResult Create()
        {
            Student student = new Student();
            Credentials cred = new Credentials();
            student.Credentials = cred;
            ViewBag.StartPort = (db.Credentials.Max(x => x.WebsitePort)+1).ToString();
            SelectList list = new SelectList(db.Teams.ToList(), "TeamId", "Name");
            ViewBag.Teams = list;

            PrincipalSearchResult<Principal> groups = interaction.GetUserGroups();
            ViewBag.UserGroup = new SelectList(groups.ToList(),"Name","Name");

            return View(student);
        }
        [HttpPost]
        public ActionResult Create(Student student)
        {
            ViewBag.StartPort = (db.Credentials.Max(x => x.WebsitePort) + 1).ToString();
            student.Credentials.FTPUserName = student.Credentials.DatabaseUserName;
            student.Credentials.FTPPassword = student.Credentials.DatabasePassword;
            interaction.CreateSingleUser(student.Credentials.DatabaseUserName, student.Credentials.DatabasePassword, student.Credentials.WebsitePort, student.Name, student.TeamId, student.Credentials.WindowsUserGroupName, true, true);
            return RedirectToAction("Index");
        }

        public ActionResult CreateMany()
        {
            ViewBag.StartPort = (db.Credentials.Max(x => x.WebsitePort) + 1).ToString();
            CreateManyViewModel vm = new CreateManyViewModel();
            Credentials cred = new Credentials();
            vm.Credentials = cred;

            SelectList list = new SelectList(db.Teams.ToList(), "TeamId", "Name");
            ViewBag.Teams = list;

            PrincipalSearchResult<Principal> groups = interaction.GetUserGroups();
            ViewBag.UserGroup = new SelectList(groups.ToList(), "Name", "Name");

            return View(vm);
        }
        [HttpPost]
        public ActionResult CreateMany(CreateManyViewModel vm)
        {
          
            interaction.CreateMany(vm.UserNamePrefix,vm.UserNameSuffix, vm.Team.TeamId,vm.Credentials.DatabasePassword,vm.Credentials.WebsitePort,vm.Credentials.WindowsUserGroupName, vm.StudentNames, vm.Credentials.DisablePasswordChange,vm.Credentials.PasswordNeverExpires);
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            SelectList list = new SelectList(db.Teams.ToList(), "TeamId", "Name");
            ViewBag.Teams = list;

            PrincipalSearchResult<Principal> groups = interaction.GetUserGroups();

            ViewBag.UserGroup = new SelectList(groups.ToList(), "Name", "Name");

            return View(db.Students.Find(id));
        }
        public ActionResult Edit(int id)
        {
            Student toEdit = db.Students.Find(id);
            return View(toEdit);
        }
        [HttpPost]
        public ActionResult Edit(Student student)
        {
            Student s = db.Students.Find(student.TeamId);
            if (TryUpdateModel<Student>(student))
            {
                try
                {
                    db.Entry(student).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating model " + ex.Message);
                }
            }
            return RedirectToAction("Index");
          
        }

        public ActionResult Delete(int id)
        {
            Student toDelete = db.Students.Find(id);
            return View(toDelete);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            int studId = 0;
            if (id != null)
            {
                studId = Convert.ToInt32(id);
            
            try {
                interaction.DeleteUserWebsiteFTPDatabase(studId);
            interaction.GetDbContext().SaveChanges();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + "\n" + ex.StackTrace + "\n" + ex.InnerException);
            }
            }
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                interaction.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}