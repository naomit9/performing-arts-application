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
using System.Diagnostics;
using PerformingArtsApplication.Migrations;

namespace PerformingArtsApplication.Controllers
{
    public class StudentDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        ///     Returns a list of all students in the system. If there is a search input, returns a list of students related to the search.
        /// </summary>
        /// <param name="SearchKey"> An optional parameter (null if not provided) of the user's search input (as a string) </param>
        /// <returns>
        ///     Returns all students in the database including their student id, first name, last name, and birthday
        /// </returns>
        /// <example>
        ///     GET: api/StudentData/ListStudents
        ///     GET: api/StudentData/ListStudents/Sophia
        /// </example>
        [HttpGet]
        [Route("api/studentdata/liststudents/{SearchKey?}")]
        public IEnumerable<StudentDto> ListStudents(string SearchKey = null)
        {
            //select all from students
            List<Student> Students = db.Students.ToList();

            //if a searchkey is entered
            if (!string.IsNullOrEmpty(SearchKey))
            {
                //select all students that have first or last names that match the search key
                Students = db.Students.Where
                   (s => s.FirstName.Contains(SearchKey) || s.LastName.Contains(SearchKey) || (s.FirstName + " " + s.LastName).Contains(SearchKey)).ToList();
            }

            List<StudentDto> StudentDtos = new List<StudentDto>();

            Students.ForEach(s => StudentDtos.Add(new StudentDto()
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Birthday = s.Birthday
            }
            ));

            return StudentDtos;
        }

        /// <summary>
        ///     Returns a list of all students in the system related to a particular lesson
        /// </summary>
        /// <returns>
        ///     Returns all students in the database related to a particular lesson id including their student id, first name, last name, and birthday
        /// </returns>
        /// <param name="id"> The lesson's primary key, lesson id (as an integer) </param>
        /// <example>
        ///     GET: api/StudentData/ListStudentsForLesson/2
        /// </example>
        [HttpGet]
        public IEnumerable<StudentDto> ListStudentsForLesson(int id)
        {
            //select all students that are in the lesson that match the id
            List<Student> Students = db.Students.Where(
                s => s.Lessons.Any(
                    l => l.LessonId == id)
                ).ToList();

            List<StudentDto> StudentDtos = new List<StudentDto>();

            Students.ForEach(s => StudentDtos.Add(new StudentDto()
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Birthday = s.Birthday
            }
            ));

            return StudentDtos;
        }

        /// <summary>
        ///     Returns a list of all students in the system not related to a particular lesson
        /// </summary>
        /// <returns>
        ///     Returns all students in the database not related to a particular lesson id including their student id, first name, last name, and birthday
        /// </returns>
        /// <param name="id"> The lesson's primary key, lesson id (as an integer) </param>
        /// <example>
        ///     GET: api/StudentData/ListStudentsNotInLesson/2
        /// </example>
        [HttpGet]
        public IEnumerable<StudentDto> ListStudentsNotInLesson(int id)
        {
            //select all students that are not in the performance that match the id
            List<Student> Students = db.Students.Where(
                s => !s.Lessons.Any(
                    l => l.LessonId == id)
                ).ToList();

            List<StudentDto> StudentDtos = new List<StudentDto>();

            Students.ForEach(s => StudentDtos.Add(new StudentDto()
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Birthday = s.Birthday
            }
            ));

            return StudentDtos;
        }

        /// <summary>
        ///     Returns a list of all students in the system related to a particular performance
        /// </summary>
        /// <returns>
        ///     Returns all students in the database related to a particular performance id including their student id, first name, last name, and birthday
        /// </returns>
        /// <param name="id"> The performance's primary key, performance id (as an integer) </param>
        /// <example>
        ///     GET: api/StudentData/ListStudentsForPerformance/2
        /// </example>
        [HttpGet]
        public IEnumerable<StudentDto> ListStudentsForPerformance(int id)
        {
            //select all students that are in the performance that match the id
            List<Student> Students = db.Students.Where(
                s => s.Performances.Any(
                    p => p.PerformanceId == id)
                ).ToList();

            List<StudentDto> StudentDtos = new List<StudentDto>();

            Students.ForEach(s => StudentDtos.Add(new StudentDto()
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Birthday = s.Birthday
            }
            ));

            return StudentDtos;
        }

        /// <summary>
        ///     Returns a list of all students in the system not related to a particular performance
        /// </summary>
        /// <returns>
        ///     Returns all students in the database not related to a particular performance id including their student id, first name, last name, and birthday
        /// </returns>
        /// <param name="id"> The performance's primary key, performance id (as an integer) </param>
        /// <example>
        ///     GET: api/StudentData/ListStudentsNotInPerformance/2
        /// </example>
        [HttpGet]
        public IEnumerable<StudentDto> ListStudentsNotInPerformance(int id)
        {
            //select all students that are not in the performance that match the id
            List<Student> Students = db.Students.Where(
                s => !s.Performances.Any(
                    p => p.PerformanceId == id)
                ).ToList();

            List<StudentDto> StudentDtos = new List<StudentDto>();

            Students.ForEach(s => StudentDtos.Add(new StudentDto()
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Birthday = s.Birthday
            }
            ));

            return StudentDtos;
        }


        /// <summary>
        ///     Recieves a student id and returns the corresponding student
        /// </summary>
        /// <param name="id"> The student's primary key, student id (as an integer) </param>
        /// <returns>
        ///     Returns one student for the given id including its student id, first name, last name, and birthday
        /// </returns>
        /// <example>
        ///     GET: api/StudentData/FindStudent/5
        /// </example>
        [ResponseType(typeof(Student))]
        [HttpGet]
        public IHttpActionResult FindStudent(int id)
        {
            Student Student = db.Students.Find(id);
            StudentDto StudentDto = new StudentDto()
            {
                StudentId = Student.StudentId,
                FirstName = Student.FirstName,
                LastName = Student.LastName,
                Birthday = Student.Birthday
            };
            if (Student == null)
            {
                return NotFound();
            }

            return Ok(StudentDto);
        }

        /// <summary>
        ///     Recieves a student id and the updated information about a student, then updates the student's information in the system with the data input
        /// </summary>
        /// <param name="id"> The student's primary key, student id (as an integer) of the student to update </param>
        /// <param name="student"> Updated information about a student (student object as JSON FORM DATA)
        ///                            - properties of student object include student id, first name, last name, and birthday
        /// </param>
        /// <returns>
        ///     HEADER: 200 (Success, No Content Response)
        ///         or
        ///     HEADER: 400 (Bad Request)
        ///         or
        ///     HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        ///   POST: api/StudentData/UpdateStudent/5
        ///   FORM DATA: Student JSON object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateStudent(int id, Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //server side validation
            if (string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName) || student.Birthday == DateTime.MinValue)
            {
                return BadRequest(ModelState);
            }

            if (id != student.StudentId)
            {
                return BadRequest();
            }

            db.Entry(student).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///     Recieves information for a new student, adds the new student and its information to the system
        /// </summary>
        /// <param name="student"> New student and its information (student object as JSON FORM DATA)
        ///                            - properties of student object include student id, first name, last name, and birthday
        /// </param>        
        /// <returns>
        ///     HEADER: 201 (Created)
        ///     CONTENT: Student Id, Student Data
        ///         or
        ///     HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        ///     POST: api/StudentData/AddStudent
        ///     FORM DATA: Student JSON object
        /// </example>
        [ResponseType(typeof(Student))]
        [HttpPost]
        public IHttpActionResult AddStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //server side validation
            if (string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName) || student.Birthday == DateTime.MinValue)
            {
                return BadRequest(ModelState);
            }

            db.Students.Add(student);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = student.StudentId }, student);
        }

        /// <summary>
        ///     Recieves a student id and deletes the corresponding student from the system
        /// </summary>
        /// <param name="id"> The student's primary key, student id (as an integer) </param>
        /// <returns>
        ///     HEADER: 200 (Ok)
        ///         or
        ///     HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        ///     POST: api/StudentData/DeleteStudent/5
        ///     FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Student))]
        [HttpPost]
        public IHttpActionResult DeleteStudent(int id)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            db.Students.Remove(student);
            db.SaveChanges();

            return Ok(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentExists(int id)
        {
            return db.Students.Count(e => e.StudentId == id) > 0;
        }
    }
}