using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PerformingArtsApplication.Models;
using PerformingArtsApplication.Migrations;

namespace PerformingArtsApplication.Controllers
{
    public class ShowcaseDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all the showcases in the system
        /// </summary>
        /// <returns>A list of showcase events</returns>
        /// <example>GET: api/ShowcaseData/ListShowcases</example>
        [HttpGet]
        [ResponseType(typeof(ShowcaseDto))]
        public IHttpActionResult ListShowcases()
        {
            List<Showcase> Showcases = db.Showcases.ToList();
            List<ShowcaseDto> ShowcaseDtos = new List<ShowcaseDto>();

            Showcases.ForEach(s => ShowcaseDtos.Add(new ShowcaseDto()
            {
                ShowcaseId = s.ShowcaseId,
                ShowcaseName = s.ShowcaseName,
                ShowcaseDate = s.ShowcaseDate,
                Location = s.Location
            }));

            return Ok(ShowcaseDtos);
        }

        /// <summary>
        /// Returns a list of showcases that a particular performance is in
        /// </summary>
        /// <param name="id">Performance ID</param>
        /// <returns>List of Showcases</returns>
        /// <example>api/ShowcaseData/ListShowcasesForPerformance/1</example>

        [HttpGet]
        [ResponseType(typeof(ShowcaseDto))]
        public IHttpActionResult ListShowcasesForPerformance(int id)
        {
            List<Showcase> Showcases = db.Showcases.Where(
                s => s.Performances.Any(
                    p => p.PerformanceId == id
                )).ToList();
            List<ShowcaseDto> ShowcaseDtos = new List<ShowcaseDto>();

            Showcases.ForEach(s => ShowcaseDtos.Add(new ShowcaseDto()
            {
                ShowcaseId = s.ShowcaseId,
                ShowcaseName = s.ShowcaseName,
                ShowcaseDate = s.ShowcaseDate,
                Location = s.Location
            }));
            return Ok(ShowcaseDtos);
        }

        /// <summary>
        ///     Associate a particular performance with a particular showcase
        /// </summary>
        /// <param name="showcaseid"> The showcase's primary key, showcase id (as an integer) </param>
        /// <param name="performanceid"> The performance's primary key, performance id (as an integer) </param>
        /// <returns>
        ///     HEADER: 200 (OK)
        ///         or
        ///     HEADER: 404 (NOT FOUND)
        /// </returns>
        [HttpPost]
        [Route("api/showcasedata/AddPerformanceToShowcase/{showcaseid}/{performanceid}")]
        [Authorize]
        public IHttpActionResult AddPerformanceToShowcase(int showcaseid, int performanceid)
        {
            Showcase SelectedShowcase = db.Showcases.Include
                (s => s.Performances).Where
                (s => s.ShowcaseId == showcaseid).FirstOrDefault();

            Performance SelectedPerformance = db.Performances.Find(performanceid);

            if (SelectedShowcase == null || SelectedPerformance == null)
            {
                return NotFound();
            }

            SelectedShowcase.Performances.Add(SelectedPerformance);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        ///     Remove an association between a particular performance and a particular showcase
        /// </summary>
        /// <param name="showcaseid"> The showcase's primary key, showcase id (as an integer) </param>
        /// <param name="performanceid"> The performance's primary key, performance id (as an integer) </param>
        /// <returns>
        ///     HEADER: 200 (OK)
        ///         or
        ///     HEADER: 404 (NOT FOUND)
        /// </returns>
        [HttpPost]
        [Route("api/showcasedata/RemovePerformanceFromShowcase/{showcaseid}/{performanceid}")]
        [Authorize]
        public IHttpActionResult RemovePerformanceFromShowcase(int showcaseid, int performanceid)
        {
            Showcase SelectedShowcase = db.Showcases.Include
                (s => s.Performances).Where
                (s => s.ShowcaseId == showcaseid).FirstOrDefault();

            Performance SelectedPerformance = db.Performances.Find(performanceid);

            if (SelectedShowcase == null || SelectedPerformance == null)
            {
                return NotFound();
            }

            SelectedShowcase.Performances.Remove(SelectedPerformance);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns one particular showcase
        /// </summary>
        /// <param name="id">ID of Showcase</param>
        /// <returns>Returns info of one particular showcase event</returns>
        /// <example>GET: api/ShowcaseData/FindShowcase/1</example>
        [ResponseType(typeof(Showcase))]
        [HttpGet]
        public IHttpActionResult FindShowcase(int id)
        {
            Showcase Showcase = db.Showcases.Find(id);
            ShowcaseDto ShowcaseDto = new ShowcaseDto()
            {
                ShowcaseId = Showcase.ShowcaseId,
                ShowcaseName = Showcase.ShowcaseName,
                ShowcaseDate = Showcase.ShowcaseDate,
                Location = Showcase.Location
            };

            if (Showcase == null)
            {
                return NotFound();
            }

            return Ok(ShowcaseDto);
        }

        /// <summary>
        /// To send a POST request to the database to update the existing showcase through a json file
        /// </summary>
        /// <param name="id">ID of showcase</param>
        /// <param name="showcase">Info about the showcase</param>
        /// <returns>Returns new info about the showcase</returns>
        /// <example>POST: api/ShowcaseData/UpdateShowcase/5</example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateShowcase(int id, Showcase showcase)
        {
            Debug.WriteLine("The update showcase method is reached");

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != showcase.ShowcaseId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" + showcase.ShowcaseId);
                Debug.WriteLine("POST parameter" + showcase.ShowcaseId);
                Debug.WriteLine("POST parameter" + showcase.ShowcaseId);
                return BadRequest();
            }

            db.Entry(showcase).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowcaseExists(id))
                {
                    Debug.WriteLine("Showcase not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("None of the conditions triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }



        /// <summary>
        /// To send a POST request to the database to create a new showcase to the system
        /// </summary>
        /// <param name="showcase">Info about the showcase</param>
        /// <returns>Returns info about the new showcase</returns>
        /// <example>POST: api/ShowcaseData/AddShowcase/5</example>
        [ResponseType(typeof(Showcase))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddShowcase(Showcase showcase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Showcases.Add(showcase);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = showcase.ShowcaseId }, showcase);
        }


        /// <summary>
        /// To send a POST request to the database to delete a showcase by their ID
        /// </summary>
        /// <param name="id">ID of showcase</param>
        /// <returns>The deleted showcase</returns>
        /// <example>DELETE: api/ShowcaseData/DeleteShowcase/1</example>
        [ResponseType(typeof(Showcase))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteShowcase(int id)
        {
            Showcase showcase = db.Showcases.Find(id);
            if (showcase == null)
            {
                return NotFound();
            }

            db.Showcases.Remove(showcase);
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

        private bool ShowcaseExists(int id)
        {
            return db.Showcases.Count(e => e.ShowcaseId == id) > 0;
        }
    }
}