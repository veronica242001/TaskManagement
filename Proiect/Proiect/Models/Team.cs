using Proiect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Task_Management.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        [Required(ErrorMessage = "Numele echipei este obligatoriu")]

        [StringLength(100, ErrorMessage = "Numele nu poate avea mai mult de 24 caractere")]
        public string Name{ get; set; }

        [Required(ErrorMessage = "Descrierea echipei este obligatorie")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        //o echipa are mai multi membri
        public virtual ICollection<Member> Members { get; set; }
        //o echipa are mai multe taskuri
        //public virtual ICollection<Task> Tasks { get; set; }


    }
}