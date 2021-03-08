using Proiect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Task_Management.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        [Required(ErrorMessage = "Titlul taskului este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 15 caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Descrierea Taskului este obligatorie")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Required]
        public Stat Status { get; set; }

        [Required(ErrorMessage = "Data de inceput este obligatorie")][DataType(DataType.Date)]
        public DateTime DateStart { get; set; }
        [Required(ErrorMessage = "Data de final este obligatorie")][DataType(DataType.Date)]
        public DateTime DateFinish { get; set; }
        // ID_PROIECT
        public int ProjectId { get; set; }
        public int? MemberId { get; set; }

        public virtual Project Project { get; set; }
        // un task are mai multe comentarii
        public virtual ICollection<Comment> Comments { get; set; }
        //la un task poate fi asignat un singur membru
        public virtual Member Member { get; set; }

        public IEnumerable<SelectListItem> Members { get; set; }
        //proprietate cu ajutorul careia putem prelua si transfera toate
        // echipele din baza de date in helper-ul pentru DropDownList.
    }


    public enum Stat
    {
        Inceput,
        Terminat,
        Activ
    }
}