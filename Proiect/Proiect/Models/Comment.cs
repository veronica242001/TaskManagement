using Proiect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task_Management.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
         // camp obligatoriu
        [Required(ErrorMessage = "Descrierea comentariului este obligatorie")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
       //  +ID_TASK
       public int TaskId { get; set; }// cheia externa
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Task Task { get; set; }
    }
}