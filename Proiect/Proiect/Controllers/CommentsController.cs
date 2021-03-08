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
    [Authorize(Roles = "User,Organizator,Admin")]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (User.IsInRole("Admin") || User.Identity.GetUserId() == comm.UserId)
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a sterge acest comentariu";
                return Redirect("/Tasks/Show/" + comm.TaskId);
            }
        }
        
        [HttpPut]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    Comment comm = db.Comments.Find(id);
                    if (User.IsInRole("Admin") || User.Identity.GetUserId() == comm.UserId)
                    {
                        

                        if (TryUpdateModel(comm))
                        {
                            comm.Content = requestComment.Content;
                            db.SaveChanges();
                            TempData["message"] = "Comentariu a fost editat!";
                            return Redirect("/Tasks/Show/" + comm.TaskId);
                        }
                        else
                        {
                            return View(requestComment);
                        }
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul de a edita acest comentariu";
                        return Redirect("/Tasks/Show/" + comm.TaskId);
                    }
                }
                else
                {
                    return View(requestComment);
                }
            }


            catch (Exception e)
            {
                return View(requestComment);
            }

    }
        [HttpDelete]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (User.IsInRole("Admin") || User.Identity.GetUserId() == comm.UserId)
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a sterge acest comentariu";
            }
            return Redirect("/Tasks/Show/" + comm.TaskId);
        }

        [Authorize(Roles = "Organizator,User,Admin")]
        public ActionResult New(int id)
        {                
            Comment comm = new Comment();
            comm.TaskId = id;
            Task task = db.Tasks.Find(id);
           // Project project = db.Projects.Find(Task.ProjectId); //luam proiectul la care apartine taskul
            Team team = db.Teams.Find(task.Project.TeamId);
             foreach(var mm in team.Members)
            {
                if(User.Identity.GetUserId()==mm.UserId)
                     return View(comm);
            }
             if (User.IsInRole("Admin") || User.IsInRole("Organizator"))
            {
                return View(comm);
            }
            TempData["message"] = "Nu aveti acces la adaugare de comentariu!";
            return Redirect("/Tasks/Show/" + comm.TaskId);


        }


        [HttpPost]
        [Authorize(Roles = "Organizator,User,Admin")]
        public ActionResult New(Comment comm)
        {
            comm.UserId = User.Identity.GetUserId(); //userul care adauga comentariu
            Task task = db.Tasks.Find(comm.TaskId);
            Team team = db.Teams.Find(task.Project.TeamId);
            bool check = false;
            foreach (var mm in team.Members)
            {
                if (User.Identity.GetUserId() == mm.UserId)
                    check = true;
            }
            if (User.IsInRole("Admin") || User.IsInRole("Organizator"))
            {
                check = true;
            }
            try
            {
                if (ModelState.IsValid)
                {   if (check == true)
                    {
                        db.Comments.Add(comm);
                        db.SaveChanges();
                        TempData["message"] = "Comentariul a fost adaugat!";
                        return Redirect("/Tasks/Show/" + comm.TaskId);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti acces la adaugare de comentariu!";
                        return Redirect("/Tasks/Show/" + comm.TaskId);
                    }
                }
                else
                {
                    return View(comm);
                }

            }
            catch (Exception e)
            {
                return View(comm);
            }

        }

    }
}