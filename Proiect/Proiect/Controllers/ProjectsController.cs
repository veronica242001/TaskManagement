using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Projects
        private int _perPage = 3;
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Index()
        {
            var projects = db.Projects.Include("Team").Include("User").OrderBy(a => a.Name);

            var totalItems = projects.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }
            var paginatedProjects = projects.Skip(offset).Take(this._perPage);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Projects = paginatedProjects;
            return View();
        }
        //GET: Show
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Show(int id)
        {
            var project = db.Projects.Find(id);

            var projects = db.Projects.Include("Team");

            ViewBag.Membru = false;
            
            foreach (var mb in project.Team.Members)
            {
                if (mb.UserId == User.Identity.GetUserId())
                    ViewBag.Membru = true;
            }
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            SetAccessRights(id);
            ViewBag.UserList = GetAllUsers();

            /*
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Organizator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            */

            return View(project);// trimitem obiectul catre View
        }
        [HttpPost]
       [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Show(int id, Member member)
        {
            
            
            Project p = db.Projects.Find(id);
            try
            {
                if (ModelState.IsValid)
                {
                    if (p.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (db.Members.Any(anyObjectName => anyObjectName.TeamId == member.TeamId
                                      && anyObjectName.UserId == member.UserId) )
                        {
                            TempData["message"] = "Membrul exista deja";
                        }
                        else
                        {
                         
                            db.Members.Add(member);
                            db.SaveChanges();
                            TempData["message"] = "Membrul a fost adaugat";
                           
                        }
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul";

                    }
                    return Redirect("/Projects/Show/" + id);
                    
                }

                else
                {
                   

                    SetAccessRights(id);
                    ViewBag.UserList = GetAllUsers();

                    return View(p);
                }

            }

            catch (Exception e)
            {
                

                SetAccessRights(id);
                ViewBag.UserList = GetAllUsers();

                return View(p);
            }

        }


        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult New()
        {
            Project project = new Project();
            //GET ALL TEAMS
            project.Teamm = GetAllTeams();
            //preluam ID ul utilizatorului curent
            project.UserId= User.Identity.GetUserId();
            // project. pagina 8 curs 9

            return View(project);
        }
        [HttpPost]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult New(Project project)
        {
            project.Teamm = GetAllTeams();
     
            ApplicationUser user = db.Users.Find(project.UserId);
            //!!!!!!!!!!!!!!!!!!article.Date = DateTime.Now;

            try
            {
                if (ModelState.IsValid) {

                    
                        db.Projects.Add(project);   //daca nu punem cond modifica si adminul
                        if (User.IsInRole("User")) //utilizatorii care creeaza proiect devin organizatori
                        {
                            ApplicationDbContext context = new ApplicationDbContext();

                            var UserManager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(context));

                            // var roles = from role in db.Roles select role;
                            //foreach (var role in roles)
                            //{
                            UserManager.RemoveFromRole(user.Id, "User");
                            //}
                            UserManager.AddToRole(user.Id, "Organizator");



                        }
                        db.SaveChanges();
                        TempData["message"] = "Proiectul a fost adaugat";
                   
                    return Redirect("/Projects/Index/");
                }
               else { 
                  return View(project);
               }
                
            }
            catch (Exception e)
            {
                return  View(project);
            }
        }
        //GET
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Edit(int id)
        {
           Project project = db.Projects.Find(id);
            project.Teamm = GetAllTeams();
            return View(project);
        }
        [HttpPut]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Edit(int id, Project requesProject)
        {   //trb sa aiba atasata si lista de echipe
            requesProject.Teamm = GetAllTeams(); 
            try
            {
                if (ModelState.IsValid)
                { Project project = db.Projects.Find(id);
                    if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {


                        if (TryUpdateModel(project))
                        {   //article=requesArticle
                            project.Name = requesProject.Name;
                            project.Content = requesProject.Content;
                            //!!!!!!!!!!!!!!!!!!!1project.Date = requesProject.Date;
                            project.TeamId = requesProject.TeamId;
                            db.SaveChanges();
                            TempData["message"] = "Proiectul  a fot editat";

                        }
                        return Redirect("/Projects/Show/"+ id);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul";
                        return Redirect("/Projects/Show/" + id);

                    }
                }
                
                    
                else
                {
                    return View(requesProject);
                }
            }

            catch (Exception e)
            {
                return View(requesProject);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Delete(int id)
        { Project project = db.Projects.Find(id);
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            { 
                db.Projects.Remove(project);
                db.SaveChanges();
                TempData["message"] = "Proiectul a fost sters cu succes!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul";
                return Redirect("/Projects/Show/" + project.ProjectId);
            }
       }
        

        [NonAction]//pentru a nu putea fi accesata
        public IEnumerable<SelectListItem> GetAllTeams()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // extragem toate echipele din baza de date
            var teams = from t in db.Teams
                             select t;

            // iteram prin echipe
            foreach (var team in teams)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = team.TeamId.ToString(),
                    Text = team.Name.ToString()
                });
            }
            
            // returnam lista de echipe
            return selectList;
        }
        [NonAction]//pentru a nu putea fi accesata
        public IEnumerable<SelectListItem> GetAllUsers()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // extragem toate echipele din baza de date
            var users = from u in db.Users
                        select u;

            // iteram prin echipe
            foreach (var user in users)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.Email.ToString()
                });
            }

            // returnam lista de echipe
            return selectList;
        }
        private void SetAccessRights(int id)
        {
            ViewBag.afisareButoane = false;
            Project project = db.Projects.Find(id);
            Team team = db.Teams.Find(project.TeamId);//avem echipa care se ocupa de task
            
            foreach ( var m in team.Members)
            {
                if(m.UserId == User.Identity.GetUserId())
                {
                    ViewBag.afisareButoane = true;
                }
            }


            if (User.IsInRole("Organizator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
    
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }

    }
}