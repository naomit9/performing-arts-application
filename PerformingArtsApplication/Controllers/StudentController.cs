using PerformingArtsApplication.Migrations;
using PerformingArtsApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Diagnostics;
using PerformingArtsApplication.Models.ViewModels;

namespace PerformingArtsApplication.Controllers
{
    public class StudentController : Controller
    {
        // code factoring to reduce redundancy
        // instantiate HttpClient once which can then be re-used through the lifetime of the application
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static StudentController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            client = new HttpClient(handler);
            //set base part of path for accessing information in student controller
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

        // GET: Student/List
        public ActionResult List(string SearchKey = null)
        {
            // get list of students in the system through an HTTP request
            // GET {resource}/api/studentdata/liststudents
            // curl https://localhost:44304/api/studentdata/liststudents

            // set the url
            string url = "studentdata/liststudents/" + SearchKey;

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<StudentDto> Students = response.Content.ReadAsAsync<IEnumerable<StudentDto>>().Result;

            //Views/Student/List.cshtml
            return View(Students);
        }

        // GET: Student/Details/5
        public ActionResult Details(int id)
        {
            DetailsStudent ViewModel = new DetailsStudent();

            // get one student in the system through an HTTP request
            // GET {resource}/api/studentdata/findstudent/{id}
            // curl https://localhost:44304/api/studentdata/findstudent/{id}

            // set the url
            string url = "studentdata/findstudent/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            StudentDto SelectedStudent = response.Content.ReadAsAsync<StudentDto>().Result;

            ViewModel.SelectedStudent = SelectedStudent;

            //show all lessons associated with this student
            url = "lessondata/listlessonsforstudent/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<LessonDto> LessonsForStudent = response.Content.ReadAsAsync<IEnumerable<LessonDto>>().Result;

            ViewModel.LessonsForStudent = LessonsForStudent;

            //show all performances associated with this student
            url = "performancedata/listperformancesforstudent/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<PerformanceDto> PerformancesForStudent = response.Content.ReadAsAsync<IEnumerable<PerformanceDto>>().Result;

            ViewModel.PerformancesForStudent = PerformancesForStudent;

            //Views/Student/Details.cshtml
            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Student/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Student student)
        {
            GetApplicationCookie();
            string url = "studentdata/addstudent";

            string jsonpayload = jss.Serialize(student);

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

        // GET: Student/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            // existing student information
            string url = "studentdata/findstudent/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            StudentDto SelectedStudent = response.Content.ReadAsAsync<StudentDto>().Result;

            return View(SelectedStudent);
        }

        // POST: Student/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Student student)
        {
            GetApplicationCookie();

            try
            {
                //send the request to the API
                string url = "studentdata/updatestudent/" + id;

                // serialize into JSON
                string jsonpayload = jss.Serialize(student);
                /*Debug.WriteLine(jsonpayload);*/

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                // POST: api/StudentData/UpdateStudent/{id}
                // Header : Content-Type: application/json

                HttpResponseMessage response = client.PostAsync(url, content).Result;

                return RedirectToAction("Details/" + id);
            }
            catch
            {
                return View();
            }
        }

        // GET: Student/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            // get the student information

            string url = "studentdata/findstudent/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            StudentDto SelectedStudent = response.Content.ReadAsAsync<StudentDto>().Result;

            return View(SelectedStudent);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();

            //send the request to the API
            string url = "studentdata/deletestudent/" + id;

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
