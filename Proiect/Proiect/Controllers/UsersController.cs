using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management.Models;

namespace Proiect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Users
        public ActionResult Index()
        {
            var users = from user in db.Users
                         orderby user.UserName
                         select user;
            ViewBag.UsersList = users;
            return View();
           

        }
        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            string currentRole = user.Roles.FirstOrDefault().RoleId;
            var userRoleName = (from role in db.Roles
                                where role.Id == currentRole
                                select role.Name).First();
            ViewBag.roleName = userRoleName;
            return View(user);
        }
        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();  //retinem toate rolurile existente pt a le pune in dropdownlist

            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            return View(user);
        }
      
        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData)
        {

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new
               RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new
               UserStore<ApplicationUser>(context));

                if (TryUpdateModel(user))
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;
                  
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);  // eliminam toate rolurile userului, in cazul nostru are un singur rol
                    }
                    var selectedRole =
                    db.Roles.Find(HttpContext.Request.Params.Get("newRole")); // preluam rolul selectat in dropdownlist

                    UserManager.AddToRole(id, selectedRole.Name); // adugam userului noul rol
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                newData.Id = id;
                return View(newData);
            }

        }



        [HttpDelete]
        public ActionResult Delete(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new
           UserStore<ApplicationUser>(context));
            var user = UserManager.Users.FirstOrDefault(u => u.Id == id);
            //stergem comm urile pe care le a lasat userul respectiv
            var comments = db.Comments.Where(comm => comm.UserId == id);
            foreach (var comment in comments)
            {
                db.Comments.Remove(comment);
            }
            //stergem proiectele pe care le-a creat userul respectiv
            var projects = db.Projects.Where(prj => prj.UserId == id);
            foreach ( var project in projects)
            {
                db.Projects.Remove(project);
            }

            //stergem membrii coresp userului
            var members = db.Members.Where(mb => mb.UserId == id);
            
            var selectList = new List<Member>();
            foreach (var m in members)
            {
               
                    selectList.Add(m);
                
            }
          
            foreach (var member in selectList)
            {
                 //setam la task-uri val null pt MemberId
                foreach (var task in member.Tasks)
                {   
                        task.MemberId = null;
                    
                }
                db.SaveChanges();
                db.Members.Remove(member);
            }
            db.SaveChanges();
            UserManager.Delete(user);
            return RedirectToAction("Index");
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()  
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

    }
}