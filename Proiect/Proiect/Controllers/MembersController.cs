using Microsoft.AspNet.Identity;
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management.Models;

namespace Proiect.Controllers
{
    public class MembersController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Members
        /*
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Index( int id, string user_id)
        {
            var members = db.Members.Include("User");
            ViewBag.UserId = user_id;
            ViewBag.Members = members;
            ViewBag.Id = id;

            ViewBag.UserList= GetAllUsers();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Organizator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View();
        }
        public ActionResult New()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Index( int id,Member member)
        {
           try
            {
                if (ModelState.IsValid)
                {
                    if (member.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        var TeamId = member.TeamId;
                        var UserId = member.UserId;
                        if (db.Members.Any(anyObjectName => anyObjectName.TeamId == TeamId
                                      && anyObjectName.UserId == UserId))
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
                    return Redirect("/Members/Index/" + id);

                }
                else
                {
                    //Team t = db.Teams.Find(member.TeamId);
                    return  View(id);
                }

            }
            catch (Exception e)
            {
                // Team t = db.Teams.Find(member.TeamId);
                return  View(id);
            }
        }*/

        [Authorize(Roles = "Organizator,Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Member member = db.Members.Find(id);
            var tasks = db.Tasks.Where(task => task.MemberId == id);
            foreach (var task in tasks)
            {
                task.MemberId = null;
            }

            
            db.Members.Remove(member);
            db.SaveChanges();
            TempData["message"] = "Membrul a fost sters!";
            return RedirectToAction("Index", "Projects");
        }



        /*
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





    */




    }
}