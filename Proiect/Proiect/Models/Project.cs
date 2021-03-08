
using Proiect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Task_Management.Models
{
     public class Project
        {
            [Key]
            public int ProjectId { get; set; }

            [Required(ErrorMessage = "Numele proiectului este obligatoriu")]
            [StringLength(100, ErrorMessage = "Numele nu poate avea mai mult de 24 caractere")]
            public string Name{ get; set; }

           [Required(ErrorMessage = "Descrierea proiectului este obligatorie")]
           [DataType(DataType.MultilineText)]
           public string Content{ get; set; }
        // TO DO:public DateTime DateStart { get; set; }

        // ID_ORGANIZATOR : user care a creat proiectul respectiv
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Alegerea echipei este obligatorie")]
          public int TeamId { get; set; }

            public virtual Team Team { get; set; }
            public virtual ICollection<Task> Tasks { get; set; }

            public IEnumerable<SelectListItem> Teamm { get; set; }
          //proprietate cu ajutorul careia putem prelua si transfera toate
         // echipele din baza de date in helper-ul pentru DropDownList.


    }
    
}