using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PerformingArtsApplication.Models
{
    public class Showcase
    {
        [Key]
        public int ShowcaseId { get; set; }

        public string ShowcaseName { get; set; }

        public DateTime ShowcaseDate { get; set; }
        
        public string Location { get; set; }

        //A showcase can have many performances in it
        public ICollection<Performance> Performances { get; set; }
    }
}