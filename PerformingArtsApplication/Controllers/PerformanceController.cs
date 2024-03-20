using System;
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
            // communicate with performance data api to retrieve one performance
            // curl https://localhost:44304/api/performancedata/findperformance/{id}

            // set the url
            string url = "performancedata/findperformance/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            PerformanceDto SelectedPerformance = response.Content.ReadAsAsync<PerformanceDto>().Result;

            return View(SelectedPerformance);
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
        public ActionResult Delete(int id, FormCollection collection)
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
