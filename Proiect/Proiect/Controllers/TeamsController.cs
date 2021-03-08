
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    
    public class TeamsController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Teams
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Index()
        {
            var teams = from team in db.Teams
                        select team;
            ViewBag.Teams = teams;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        //GET: Show
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Show(int id)
        {
            var team = db.Teams.Find(id);//echipa cu acel id
            ViewBag.Tasks = GetAllTasks(id);
            ViewBag.esteAdmin = User.IsInRole("Admin");
              
         return View(team);
        }

        //GET: New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            Team team = new Team();
            ViewBag.esteAdmin = User.IsInRole("Admin");
            //TempData["message"] = "Nu aveti dreptul de a adauga echipa";
            return View(team);
              
                
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Team team)
        {
            ViewBag.esteAdmin = User.IsInRole("Admin");
            try
            {   if (ModelState.IsValid)
                {
                    db.Teams.Add(team);
                    db.SaveChanges();
                    TempData["message"] = "Echipa a fost adaugata!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(team);
                }
            }
            catch (Exception e)
            {
                return View(team);
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);
           
            return View(team);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Team requestTeam)
        {
            try
            {
                Team team = db.Teams.Find(id);
                if (TryUpdateModel(team))
                {
                    team.Name = requestTeam.Name;
                    team.Content = requestTeam.Content;
                    db.SaveChanges();
                    TempData["message"] = "Echipa a fost modificata!";
                    return  RedirectToAction("Index");
                }
                return View(requestTeam);

            }
            catch (Exception e)
            {
                return  View(requestTeam);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
            db.SaveChanges();
            TempData["message"] = "Echipa a fost stearsa!";
            return RedirectToAction("Index");
        }
        [NonAction]//pentru a nu putea fi accesata
        public List<Task> GetAllTasks(int id)
        {
            // generam o lista goala
            // extragem toate taskurile din baza de date
            var tasks = from t in db.Tasks
                        select t;

            var team = db.Teams.Find(id);//echipa cu acel id
            var selectList = new List<Task>();
            foreach (var project in team.Projects)
            {  
                foreach (var task in project.Tasks)
                {
                    selectList.Add(task);
                }
            }
            
            // returnam lista de echipe
            return selectList;
        }

    }
    


}
