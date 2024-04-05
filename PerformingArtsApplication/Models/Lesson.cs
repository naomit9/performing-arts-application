using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerformingArtsApplication.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        public string LessonName { get; set; }

        public DateTime LessonTime {  get; set; }

        public int Room {  get; set; }

        //A lesson has one teacher
        //A teacher can teach many lessons
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }

        //A lesson can have many students in it
        public ICollection<Student> Students { get; set; }
    }

    public class LessonDto
    {
        public int LessonId { get; set; }

        public string LessonName { get; set; }

        public DateTime LessonTime { get; set; }

        public int Room { get; set; }

        public int TeacherId { get; set; }


    }
}