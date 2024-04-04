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
            client = new HttpClient();
            //set base part of path for accessing information in student controller
            client.BaseAddress = new Uri("https://localhost:44304/api/");
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

            //show all performances associated with this student
            url = "performancedata/listperformancesforstudent/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<PerformanceDto> PerformancesForStudent = response.Content.ReadAsAsync<IEnumerable<PerformanceDto>>().Result;

            ViewModel.PerformancesForStudent = PerformancesForStudent;

            //Views/Student/List.cshtml
            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Student/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        public ActionResult Create(Student student)
        {
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
        public ActionResult Update(int id, Student student)
        {
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
        public ActionResult Delete(int id)
        {
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
