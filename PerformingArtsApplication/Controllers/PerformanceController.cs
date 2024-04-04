﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using System.Web.Script.Serialization;
using PerformingArtsApplication.Models;
using Newtonsoft.Json;
using PerformingArtsApplication.Migrations;
using PerformingArtsApplication.Models.ViewModels;

namespace PerformingArtsApplication.Controllers
{
    public class PerformanceController : Controller
    {
        // code factoring to reduce redundancy
        // instantiate HttpClient once which can then be re-used through the lifetime of the application
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static PerformanceController()
        {
            client = new HttpClient();
            //set base part of path for accessing information in performance controller
            client.BaseAddress = new Uri("https://localhost:44304/api/");
        }

        // GET: Performance/List
        public ActionResult List(string SearchKey = null)
        {
            // get list of performances in the system through an HTTP request
            // GET {resource}/api/performancedata/listperformances
            // curl https://localhost:44304/api/performancedata/listperformances

            // set the url
            string url = "performancedata/listperformances/" + SearchKey;

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<PerformanceDto> Performances = response.Content.ReadAsAsync<IEnumerable<PerformanceDto>>().Result;

            //Views/Performance/List.cshtml
            return View(Performances);
        }

        // GET: Performance/Details/5
        public ActionResult Details(int id)
        {
            DetailsPerformance ViewModel = new DetailsPerformance();

            // get one performance in the system through an HTTP request
            // GET {resource}/api/performancedata/findperformance/{id}
            // curl https://localhost:44304/api/performancedata/findperformance/{id}

            // set the url
            string url = "performancedata/findperformance/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            PerformanceDto SelectedPerformance = response.Content.ReadAsAsync<PerformanceDto>().Result;

            ViewModel.SelectedPerformance = SelectedPerformance;

            // show associated students with this performance
            url = "studentdata/liststudentsforperformance/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<StudentDto> StudentsInPerformance = response.Content.ReadAsAsync<IEnumerable<StudentDto>>().Result;

            ViewModel.StudentsInPerformance = StudentsInPerformance;

            url = "studentdata/liststudentsnotinperformance/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<StudentDto> AvailableStudents = response.Content.ReadAsAsync<IEnumerable<StudentDto>>().Result;

            ViewModel.AvailableStudents = AvailableStudents;

            //Views/Performance/List.cshtml
            return View(ViewModel);
        }

        //POST: Performance/Associate/{performanceid}
        [HttpPost]
        public ActionResult Associate(int id, int StudentId)
        {
            /*Debug.WriteLine("Attempting to associate performance: " + id + " with student: " + StudentId);*/

            // call api to add student to performance
            string url = "performancedata/addstudenttoperformance/" + id + "/" + StudentId;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //GET: Performance/UnAssociate/{id}?StudentId={studentid}
        [HttpGet]
        public ActionResult UnAssociate(int id, int StudentId)
        {
            /*Debug.WriteLine("Attempting to unassociate performance: " + id + " with student: " + StudentId);*/

            // call api to remove student from performance
            string url = "performancedata/removestudentfromperformance/" + id + "/" + StudentId;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Performance/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Performance/Create
        [HttpPost]
        public ActionResult Create(Performance performance)
        {
            string url = "performancedata/addperformance";

            string jsonpayload = jss.Serialize(performance);

            /*Debug.WriteLine(jsonpayload);*/

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Performance/Edit/5
        public ActionResult Edit(int id)
        {
            // existing performance information
            string url = "performancedata/findperformance/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PerformanceDto SelectedPerformance = response.Content.ReadAsAsync<PerformanceDto>().Result;

            return View(SelectedPerformance);
        }

        // POST: Performance/Update/5
        [HttpPost]
        public ActionResult Update(int id, Performance performance)
        {
            try
            {
                //send the request to the API
                string url = "performancedata/updateperformance/" + id;

                // serialize into JSON
                string jsonpayload = jss.Serialize(performance);
                /*Debug.WriteLine(jsonpayload);*/

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                // POST: api/PerformanceData/UpdatePerformance/{id}
                // Header : Content-Type: application/json

                HttpResponseMessage response = client.PostAsync(url, content).Result;

                return RedirectToAction("Details/" + id);
            }
            catch
            {
                return View();
            }
        }

        // GET: Performance/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            // get the performance information

            string url = "performancedata/findperformance/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            PerformanceDto SelectedPerformance = response.Content.ReadAsAsync<PerformanceDto>().Result;

            return View(SelectedPerformance);
        }

        // POST: Performance/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //send the request to the API
            string url = "performancedata/deleteperformance/" + id;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
