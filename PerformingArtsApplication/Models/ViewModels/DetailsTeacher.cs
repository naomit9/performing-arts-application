using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformingArtsApplication.Models.ViewModels
{
    public class DetailsTeacher
    {
        public TeacherDto SelectedTeacher { get; set; }
        public IEnumerable<LessonDto> LessonsTaught { get; set; }
    }
}