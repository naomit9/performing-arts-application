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
    public class TeacherDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/TeacherData/ListTeachers
        [HttpGet]
        [Route("api/teacherdata/listteachers/{SearchKey?}")]
        public IEnumerable<TeacherDto> ListTeachers(string SearchKey = null)
        {
            List<Teacher> Teachers = db.Teachers.ToList();
            List<TeacherDto> TeacherDtos = new List<TeacherDto>();

            if (!string.IsNullOrEmpty(SearchKey))
            {
                Teachers = db.Teachers.Where
                    (t => t.FirstName.Contains(SearchKey)).ToList();
            }

            Teachers.ForEach(t => TeacherDtos.Add(new TeacherDto()
            {
                TeacherId = t.TeacherId,
                FirstName = t.FirstName,
                LastName = t.LastName
            }));

            return TeacherDtos;
        }

        // GET: api/TeacherData/FindTeacher/5
        [HttpGet]
        [ResponseType(typeof(TeacherDto))]
        public IHttpActionResult FindTeacher(int id)
        {
            Teacher Teacher = db.Teachers.Find(id);
            TeacherDto TeacherDto = new TeacherDto()
            {
                TeacherId = Teacher.TeacherId,
                FirstName = Teacher.FirstName,
                LastName = Teacher.LastName
            };

            if (Teacher == null)
            {
                return NotFound();
            }

            return Ok(TeacherDto);
        }

        // POST: api/TeacherData/UpdateTeacher/5
        [HttpPost]
        [ResponseType(typeof(void))]
        [Authorize]
        public IHttpActionResult UpdateTeacher(int id, Teacher Teacher)
        {
            //Debug.WriteLine("I've reached the update teacher method");
            if (!ModelState.IsValid)
            {
                //Debug.WriteLine("Model state is invalid");
                return BadRequest(ModelState);
            }

            if (id != Teacher.TeacherId)
            {
                //Debug.WriteLine("ID mismatch");
                //Debug.WriteLine("GET parameter" + id);
                //Debug.WriteLine("POST parameter" + Teacher.TeacherId);
                return BadRequest();
            }

            db.Entry(Teacher).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
                {
                    //Debug.WriteLine("Teacher not found");
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

        // POST: api/TeacherData/AddTeacher
        [HttpPost]
        [ResponseType(typeof(Teacher))]
        [Authorize]
        public IHttpActionResult AddTeacher(Teacher Teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Teacher.TeacherId <= 0 || string.IsNullOrEmpty(Teacher.FirstName))
            {
                return BadRequest(ModelState);
            }

            db.Teachers.Add(Teacher);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Teacher.TeacherId }, Teacher);
        }

        // DELETE: api/TeacherData/DeleteTeacher/5
        [HttpPost]
        [ResponseType(typeof(Teacher))]
        [Authorize]
        public IHttpActionResult DeleteTeacher(int id)
        {
            Teacher Teacher = db.Teachers.Find(id);
            if (Teacher == null)
            {
                return NotFound();
            }

            db.Teachers.Remove(Teacher);
            db.SaveChanges();

            return Ok(Teacher);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TeacherExists(int id)
        {
            return db.Teachers.Count(e => e.TeacherId == id) > 0;
        }
    }
}