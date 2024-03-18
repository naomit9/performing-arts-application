using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PerformingArtsApplication.Models
{
    public class Performance
    {
        [Key]
        public int PerformanceId {  get; set; }

        public string PerformanceName { get; set; }

        //A performance can have many students in it
        public ICollection<Student> Students { get; set; }

        //A performance can be in many showcases
        public ICollection<Showcase> Showcases { get; set; }
    }
}