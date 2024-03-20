﻿using System;
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

namespace PerformingArtsApplication.Controllers
{
    public class PerformanceDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        ///     Returns a list of all performances in the system. If there is a search input, returns a list of performances related to the search.
        /// </summary>
        /// <param name="SearchKey"> An optional parameter (null if not provided) of the user's search input (as a string) </param>
        /// <returns>
        ///     Returns all performances in the database including their performance id and performance name
        /// </returns>
        /// <example>
        ///     GET: api/PerformanceData/ListPerformances
        ///     GET: api/PerformanceData/ListPerformances/love
        /// </example>
        [HttpGet]
        [Route("api/performancedata/listperformances/{SearchKey?}")]
        public IEnumerable<PerformanceDto> ListPerformances(string SearchKey = null)
        {
            //select all from performances
            List<Performance> Performances = db.Performances.ToList();

            //if a searchkey is entered
            if (!string.IsNullOrEmpty(SearchKey))
            {
                //select all performances that have routine names that match the search key
                Performances = db.Performances.Where
                   (p => p.PerformanceName.Contains(SearchKey)).ToList();
            }

            List<PerformanceDto> PerformanceDtos = new List<PerformanceDto>();

            Performances.ForEach(p => PerformanceDtos.Add(new PerformanceDto()
            {
                PerformanceId = p.PerformanceId,
                PerformanceName = p.PerformanceName
            }
            ));

            return PerformanceDtos;
        }

        /// <summary>
        ///     Recieves a performance id and returns the corresponding performance
        /// </summary>
        /// <param name="id"> The performance's primary key, performance id (as an integer) </param>
        /// <returns>
        ///     Returns one performance for the given id including its performance id and performance name
        /// </returns>
        /// <example>
        ///     GET: api/PerformanceData/FindPerformance/5
        /// </example>
        [ResponseType(typeof(Performance))]
        [HttpGet]
        public IHttpActionResult FindPerformance(int id)
        {
            Performance Performance = db.Performances.Find(id);
            PerformanceDto PerformanceDto = new PerformanceDto()
            {
                PerformanceId = Performance.PerformanceId,
                PerformanceName = Performance.PerformanceName
            };
            if (Performance == null)
            {
                return NotFound();
            }

            return Ok(PerformanceDto);
        }

        /// <summary>
        ///     Recieves a performance id and the updated information about a performance, then updates the performance's information in the system with the data input
        /// </summary>
        /// <param name="id"> The performance's primary key, performance id (as an integer) of the performance to update </param>
        /// <param name="performance"> Updated information about a performance (performance object as JSON FORM DATA)
        ///                            - properties of performance object include performance id and performance name
        /// </param>
        /// <returns>
        ///     HEADER: 200 (Success, No Content Response)
        ///         or
        ///     HEADER: 400 (Bad Request)
        ///         or
        ///     HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        ///   POST: api/PerformanceData/UpdatePerformance/5
        ///   FORM DATA: Performance JSON object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdatePerformance(int id, Performance performance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //server side validation
            if (string.IsNullOrEmpty(performance.PerformanceName))
            {
                return BadRequest(ModelState);
            }

            if (id != performance.PerformanceId)
            {
                return BadRequest();
            }

            db.Entry(performance).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformanceExists(id))
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
        ///     Recieves information for a new performance, adds the new performance and its information to the system
        /// </summary>
        /// <param name="performance"> New performance and its information (performance object as JSON FORM DATA)
        ///                            - properties of performance object include performance id and performance name
        /// </param>        
        /// <returns>
        ///     HEADER: 201 (Created)
        ///     CONTENT: Performance Id, Performance Data
        ///         or
        ///     HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        ///     POST: api/PerformanceData/AddPerformance
        ///     FORM DATA: Performance JSON object
        /// </example>
        [ResponseType(typeof(Performance))]
        [HttpPost]
        public IHttpActionResult AddPerformance(Performance performance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //server side validation
            if (performance.PerformanceId <= 0 || string.IsNullOrEmpty(performance.PerformanceName))
            {
                return BadRequest(ModelState);
            }

            db.Performances.Add(performance);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = performance.PerformanceId }, performance);
        }

        /// <summary>
        ///     Recieves a performance id and deletes the corresponding performance from the system
        /// </summary>
        /// <param name="id"> The performance's primary key, performance id (as an integer) </param>
        /// <returns>
        ///     HEADER: 200 (Ok)
        ///         or
        ///     HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        ///     POST: api/PerformanceData/DeletePerformance/5
        ///     FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Performance))]
        [HttpPost]
        public IHttpActionResult DeletePerformance(int id)
        {
            Performance performance = db.Performances.Find(id);
            if (performance == null)
            {
                return NotFound();
            }

            db.Performances.Remove(performance);
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

        private bool PerformanceExists(int id)
        {
            return db.Performances.Count(e => e.PerformanceId == id) > 0;
        }
    }
}