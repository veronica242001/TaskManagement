using Microsoft.AspNet.Identity;
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    public class TasksController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: Tasks
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Index()
        {
            var tasks = db.Tasks.Include("Project");
            ViewBag.Tasks = tasks;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }


        //GET: Show
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Show(int id)
        {


            Task task = db.Tasks.Find(id);
            Team team = db.Teams.Find(task.Project.TeamId);//avem echipa care se ocupa de task
            task.Members = GetAllMembers(task.Project.TeamId);
            //ViewBag.Membru = task.Member.UserId;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.afisareButoane = false;
            foreach (var mm in team.Members)
            {
                if (User.Identity.GetUserId() == mm.UserId)
                    ViewBag.afisareButoane = true;
            }
            if (User.IsInRole("Organizator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
          
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(task);
            
        }
        //GET: New
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult New(int id)
        {

            Task task = new Task();
            task.ProjectId = id;
            Project project = db.Projects.Find(id);
            task.Members = GetAllMembers(project.TeamId);

            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {

                return View(task);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul";
                return Redirect("/Projects/Show/" + task.ProjectId);

            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult New(Task task)
        {
            Project project = db.Projects.Find(task.ProjectId);
            task.Members = GetAllMembers(project.TeamId);
            try {
                if (ModelState.IsValid)
                {
                    if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        db.Tasks.Add(task);
                        db.SaveChanges();
                        TempData["message"] = "Taskul a fost adaugat!";


                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul";

                    }
                    return Redirect("/Projects/Show/" + task.ProjectId);
                }
                else
                {

                    return View(task);
                }

            } catch (Exception e)
            {
                return View(task);
            }
        }

        //GET
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit(int id)
        {   ViewBag.esteAdmin = User.IsInRole("Admin");
            //ViewBag.esteMembru = User.IsInRole("User");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            Task task = db.Tasks.Find(id);
            task.TaskId = id;            
            Team team = db.Teams.Find(task.Project.TeamId);//avem echipa care se ocupa de task
            task.Members = GetAllMembers(task.Project.TeamId);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
           
            if (task.MemberId !=null && User.Identity.GetUserId() ==task.Member.UserId)
                return View(task);
            if (task.Project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(task);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a modifica taskul";
                return Redirect("/Projects/Show/" + task.ProjectId);

            }
           
        }
        [HttpPut]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit(int id, Task requesTask)
        {
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.esteMembru = User.IsInRole("User");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            Task task = db.Tasks.Find(id);
            task.TaskId = id;
            Team team = db.Teams.Find(task.Project.TeamId);//avem echipa care se ocupa de task
            requesTask.Members = GetAllMembers(task.Project.TeamId);
            bool check = false;
           if (task.MemberId != null && User.Identity.GetUserId() == task.Member.UserId)
                    check = true;
           
           try
            {
                if (ModelState.IsValid)
                {
                    if (task.Project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin") || check == true)
                    {
                        if (TryUpdateModel(task))
                        {
                            task.Title = requesTask.Title;
                            task.Content = requesTask.Content;
                            task.ProjectId = requesTask.ProjectId;
                            task.Status = requesTask.Status;
                            task.DateStart = requesTask.DateStart;
                            task.DateFinish = requesTask.DateFinish;
                            task.MemberId = requesTask.MemberId;
                            db.SaveChanges();
                            TempData["message"] = "Taskul a fost editat!";
                        }
                        return Redirect("/Tasks/Show/" + task.TaskId);
                    }
                    
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul";
                        return Redirect("/Projects/Show/" + task.ProjectId);

                    }
                }

                else
                { //sa  faca redirect cu elem deja modificate
                    return View(requesTask);
                }
            }
             catch (Exception e)
            {
                return  View(requesTask);
            }
        }
        [NonAction]//pentru a nu putea fi accesata
        public IEnumerable<SelectListItem> GetAllMembers( int id)
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            Team team = db.Teams.Find(id);
            // extragem toate echipele din baza de date
            var members = from u in team.Members
                        select u;

            // iteram prin echipe
            foreach (var m in members)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.User.Email.ToString()
                });
            }

            // returnam lista de 
            return selectList;
        }

        [Authorize(Roles = "Organizator,Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks.Find(id);
            if (task.Project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {

                db.Tasks.Remove(task);
                db.SaveChanges();
                TempData["message"] = "Taskul a fost sters!";
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul";
                return Redirect("/Projects/Show/" + task.ProjectId);

            }

        }

    }

}
