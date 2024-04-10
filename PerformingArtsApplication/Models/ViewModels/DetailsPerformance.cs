using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformingArtsApplication.Models.ViewModels
{
    public class DetailsPerformance
    {
        public PerformanceDto SelectedPerformance { get; set; }
        public IEnumerable<StudentDto> StudentsInPerformance { get; set; }
        public IEnumerable<StudentDto> AvailableStudents { get; set; }
        public IEnumerable<ShowcaseDto> PerformanceShowcases { get; set; }
    }
}