using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerformingArtsApplication.Models
{
    public class Showcase
    {
        [Key]
        public int ShowcaseId { get; set; }

        public string ShowcaseName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ShowcaseDate { get; set; }
        
        public string Location { get; set; }

        //A showcase can have many performances in it
        public ICollection<Performance> Performances { get; set; }
    }

    public class ShowcaseDto 
    {
        public int ShowcaseId { get; set; }
        public string ShowcaseName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ShowcaseDate { get; set; }
        public string Location { get; set; }
    }

}