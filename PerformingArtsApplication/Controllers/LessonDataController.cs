using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PerformingArtsApplication.Models;

namespace PerformingArtsApplication.Controllers
{
    public class LessonDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/LessonData/ListLessons
        [HttpGet]
        [Route("api/lessondata/listlessons/{SearchKey?}")]
        public IEnumerable<LessonDto> ListLessons(string SearchKey = null)
        {
            List<Lesson> Lessons = db.Lessons.ToList();
            List<LessonDto> LessonDtos = new List<LessonDto>();

            if (!string.IsNullOrEmpty(SearchKey))
            {
                Lessons = db.Lessons.Where
                    (l => l.LessonName.Contains(SearchKey)).ToList();
            }

            Lessons.ForEach(l => LessonDtos.Add(new LessonDto()
            {
                LessonId = l.LessonId,
                LessonName = l.LessonName,
                LessonTime = l.LessonTime,
                Room = l.Room,
                TeacherId = l.TeacherId
            }));

            return LessonDtos;
        }

        /// <summary>
        ///     Returns a list of all lessons in the system taught by a particular teacher
        /// </summary>
        /// <returns>
        ///     Returns all lessons in the database associated with a specific teacher id including their lesson id and lesson name, lesson time, room, and teacher id
        /// </returns>
        /// <param name="id"> The teacher's primary key, teacher id (as an integer) </param>
        /// <example>
        ///     GET: api/LessonData/ListLessonsForTeacher
        /// </example>
        [HttpGet]
        public IEnumerable<LessonDto> ListLessonsForTeacher(int id)
        {
            //select all from lessons
            List<Lesson> Lessons = db.Lessons.Where(l => l.TeacherId == id).ToList();

            List<LessonDto> LessonDtos = new List<LessonDto>();

            Lessons.ForEach(l => LessonDtos.Add(new LessonDto()
            {
                LessonId = l.LessonId,
                LessonName = l.LessonName,
                LessonTime = l.LessonTime,
                Room = l.Room,
                TeacherId = l.TeacherId
            }
            ));

            return LessonDtos;
        }

        /// <summary>
        ///     Returns a list of all lessons in the system related to a particular student
        /// </summary>
        /// <returns>
        ///     Returns all lessons in the database associated with a specific student id including their lesson id and lesson name, lesson time, room, and teacher id
        /// </returns>
        /// <param name="id"> The student's primary key, student id (as an integer) </param>
        /// <example>
        ///     GET: api/LessonData/ListLessonsForStudent/1
        /// </example>
        [HttpGet]
        public IEnumerable<LessonDto> ListLessonsForStudent(int id)
        {
            //select all lessons that have the student that matches with the id in it
            List<Lesson> Lessons = db.Lessons.Where(
                l => l.Students.Any(
                    s => s.StudentId == id
                )).ToList();

            List<LessonDto> LessonDtos = new List<LessonDto>();

            Lessons.ForEach(l => LessonDtos.Add(new LessonDto()
            {
                LessonId = l.LessonId,
                LessonName = l.LessonName,
                LessonTime = l.LessonTime,
                Room = l.Room,
                TeacherId = l.TeacherId
            }
            ));

            return LessonDtos;
        }

        /// <summary>
        ///     Associate a particular student with a particular lesson
        /// </summary>
        /// <param name="lessonid"> The lesson's primary key, lesson id (as an integer) </param>
        /// <param name="studentid"> The student's primary key, student id (as an integer) </param>
        /// <returns>
        ///     HEADER: 200 (OK)
        ///         or
        ///     HEADER: 404 (NOT FOUND)
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("api/lessondata/AddStudentToLesson/{lessonid}/{studentid}")]
        public IHttpActionResult AddStudentToLesson(int lessonid, int studentid)
        {
            Lesson SelectedLesson = db.Lessons.Include
                (l => l.Students).Where
                (l => l.LessonId == lessonid).FirstOrDefault();

            Student SelectedStudent = db.Students.Find(studentid);

            if (SelectedLesson == null || SelectedStudent == null)
            {
                return NotFound();
            }

            SelectedLesson.Students.Add(SelectedStudent);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        ///     Remove an association between a particular student with a particular lesson
        /// </summary>
        /// <param name="lessonid"> The lesson's primary key, lesson id (as an integer) </param>
        /// <param name="studentid"> The student's primary key, student id (as an integer) </param>
        /// <returns>
        ///     HEADER: 200 (OK)
        ///         or
        ///     HEADER: 404 (NOT FOUND)
        /// </returns>
        [HttpPost]
        [Authorize]
        [Route("api/lessondata/RemoveStudentFromLesson/{lessonid}/{studentid}")]
        public IHttpActionResult RemoveStudentFromLesson(int lessonid, int studentid)
        {
            Lesson SelectedLesson = db.Lessons.Include
                (l => l.Students).Where
                (l => l.LessonId == lessonid).FirstOrDefault();

            Student SelectedStudent = db.Students.Find(studentid);

            if (SelectedLesson == null || SelectedStudent == null)
            {
                return NotFound();
            }

            SelectedLesson.Students.Remove(SelectedStudent);
            db.SaveChanges();

            return Ok();
        }

        // GET: api/LessonData/FindLesson/5
        [HttpGet]
        [ResponseType(typeof(LessonDto))]
        public IHttpActionResult FindLesson(int id)
        {
            Lesson Lesson = db.Lessons.Find(id);
            LessonDto LessonDto = new LessonDto()
            {
                LessonId = Lesson.LessonId,
                LessonName = Lesson.LessonName,
                LessonTime = Lesson.LessonTime,
                Room = Lesson.Room,
                TeacherId = Lesson.TeacherId
            };

            if (Lesson == null)
            {
                return NotFound();
            }

            return Ok(LessonDto);
        }

        // POST: api/LessonData/UpdateLesson/5
        [HttpPost]
        [ResponseType(typeof(void))]
        [Authorize]
        public IHttpActionResult UpdateLesson(int id, Lesson Lesson)
        {
            //Debug.WriteLine("I've reached the update lesson method");
            if (!ModelState.IsValid)
            {
                //Debug.WriteLine("Model state is invalid");
                return BadRequest(ModelState);
            }

            if (id != Lesson.LessonId)
            {
                //Debug.WriteLine("ID mismatch");
                //Debug.WriteLine("GET parameter" + id);
                //Debug.WriteLine("POST parameter" + Lesson.LessonId);
                return BadRequest();
            }

            db.Entry(Lesson).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
                {
                    //Debug.WriteLine("Lesson not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //Debug.WriteLine("None of the conditions triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LessonData/AddLesson
        [HttpPost]
        [ResponseType(typeof(Lesson))]
        [Authorize]
        public IHttpActionResult AddLesson(Lesson Lesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Lessons.Add(Lesson);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Lesson.LessonId }, Lesson);
        }

        // DELETE: api/LessonData/DeleteLesson/5
        [HttpPost]
        [ResponseType(typeof(Lesson))]
        [Authorize]
        public IHttpActionResult DeleteLesson(int id)
        {
            Lesson Lesson = db.Lessons.Find(id);
            if (Lesson == null)
            {
                return NotFound();
            }

            db.Lessons.Remove(Lesson);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LessonExists(int id)
        {
            return db.Lessons.Count(e => e.LessonId == id) > 0;
        }
    }
}