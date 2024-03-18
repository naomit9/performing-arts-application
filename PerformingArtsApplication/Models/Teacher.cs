using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace PerformingArtsApplication.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
    }
}