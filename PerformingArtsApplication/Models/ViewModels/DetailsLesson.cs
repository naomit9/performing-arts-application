using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformingArtsApplication.Models.ViewModels
{
    public class DetailsLesson
    {
        public LessonDto SelectedLesson { get; set; }
        public IEnumerable<StudentDto> StudentsInLesson { get; set; }
        public IEnumerable<StudentDto> AvailableStudents { get; set; }
    }
}