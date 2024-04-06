using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformingArtsApplication.Models.ViewModels
{
    public class DetailsStudent
    {
        public StudentDto SelectedStudent { get; set; }
        public IEnumerable<PerformanceDto> PerformancesForStudent { get; set; }
        public IEnumerable<LessonDto> LessonsForStudent { get; set;}
    }
}