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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            client = new HttpClient(handler);
            //set base part of path for accessing information in performance controller
            client.BaseAddress = new Uri("https://localhost:44304/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";

            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Performance/List
        public ActionResult List(string SearchKey = null, int PageNum =0)
        {
            // get list of performances in the system through an HTTP request
            // GET {resource}/api/performancedata/listperformances
            // curl https://localhost:44304/api/performancedata/listperformances

            PerformanceList ViewModel = new PerformanceList();

            // set the url
            string url = "performancedata/listperformances/" + SearchKey;

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<PerformanceDto> Performances = response.Content.ReadAsAsync<IEnumerable<PerformanceDto>>().Result;

            // start of pagination algorithm

            // find the total number of performances
            int PerformanceCount = Performances.Count();
            // number of performances to display per page
            int PerPage = 4;
            // determines the maximum number of pages (rounded up), assuming a page 0 start.
            int MaxPage = (int)Math.Ceiling((decimal)PerformanceCount / PerPage) - 1;

            // lower boundary for max page
            if (MaxPage < 0) MaxPage = 0;
            // lower boundary for page number
            if (PageNum < 0) PageNum = 0;
            // upper bound for page number
            if (PageNum > MaxPage) PageNum = MaxPage;

            // the record index of page start
            int StartIndex = PerPage * PageNum;

            // helps generate the html which shows "Page 1 of ..." on the list view
            ViewModel.PageNum = PageNum;
            ViewModel.PageSummary = " " + (PageNum + 1) + " of " + (MaxPage + 1) + " ";

            // end of pagination algorithm

            //send another request to get the page slice of full list
            url = "performancedata/listperformancespage/" + StartIndex + "/" + PerPage + "/" + SearchKey;
            response = client.GetAsync(url).Result;

            // retrieve the response of the HTTP Request
            IEnumerable<PerformanceDto> SelectedPerformancesPage = response.Content.ReadAsAsync<IEnumerable<PerformanceDto>>().Result;

            ViewModel.Performances = SelectedPerformancesPage;

            //Views/Performance/List.cshtml
            return View(ViewModel);
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

            // show associated showcases with this performance
            url = "showcasedata/listshowcasesforperformance/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ShowcaseDto> PerformanceShowcases = response.Content.ReadAsAsync<IEnumerable<ShowcaseDto>>().Result;

            ViewModel.PerformanceShowcases = PerformanceShowcases;

            //Views/Performance/Details.cshtml
            return View(ViewModel);
        }

        //POST: Performance/Associate/{performanceid}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id, int StudentId)
        {
            GetApplicationCookie();

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
        [Authorize]
        public ActionResult UnAssociate(int id, int StudentId)
        {
            GetApplicationCookie();

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
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Performance/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Performance performance)
        {
            GetApplicationCookie();

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
        [Authorize]
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
        [Authorize]
        public ActionResult Update(int id, Performance performance)
        {
            GetApplicationCookie();

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
        [Authorize]
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();

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
