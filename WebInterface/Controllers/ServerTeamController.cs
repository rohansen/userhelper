using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserHelperCL;
using UserHelperCL.Database;
using UserHelperCL.Models;

namespace WebInterface.Controllers
{
    [Authorize(Roles="Administrator")]
    public class ServerTeamController : Controller
    {
        StudentsModel db = new StudentsModel();
        UserInteraction interaction = new UserInteraction();
        // GET: ServerTeam
        public ActionResult Index()
        {
            return View(db.Teams.ToList());
        }

        public ActionResult Create()
        {
            Team t = new Team();
            return View(t);
        }

        [HttpPost]
        public ActionResult Create(Team t)
        {
            t.CreatedDate = DateTime.Now;
            db.Teams.Add(t);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            return View(db.Teams.Find(id));
        }

        public ActionResult Edit(int id){
        
            Team toEdit = db.Teams.Find(id);
            return View(toEdit);
        }

        [HttpPost]
        public ActionResult Edit(Team team)
        {
            Team t = db.Teams.Find(team.TeamId);
            if (TryUpdateModel<Team>(t))
            {
                try
                {
                    db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating model " + ex.Message);
                }
            }
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int? id)
        {
            Team t = db.Teams.Find(id);
            return View(t);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            
            interaction.DeleteTeam(id);
            
          
            return RedirectToAction("Index");
        }


        public ActionResult Print(int id)
        {
            Team t = db.Teams.Find(id);
            return View(t);
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