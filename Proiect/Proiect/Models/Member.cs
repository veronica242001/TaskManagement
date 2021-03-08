using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management.Models;

namespace Proiect.Models
{
    public class Member
    {
        
        [Key]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Alegerea userului este  obligatorie")]
        [StringLength(1000, ErrorMessage = "prea multe caractere")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        //public IEnumerable<SelectListItem> Userr { get; set; }
       
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }

        //public IEnumerable<SelectListItem> Teamm { get; set; }
        //proprietate cu ajutorul careia putem prelua si transfera toate
        // echipele din baza de date in helper-ul pentru DropDownList.
      
        public virtual ICollection<Task> Tasks { get; set; }

    }
}