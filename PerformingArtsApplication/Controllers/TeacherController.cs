using PerformingArtsApplication.Models;
using PerformingArtsApplication.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PerformingArtsApplication.Controllers
{
    public class TeacherController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static TeacherController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            client = new HttpClient(handler);
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

        // GET: Teacher/List
        public ActionResult List(string SearchKey = null)
        {
            //communicate with lesson data to retrieve list of animals
            //curl https://localhost:44304/api/teacherdata/listteachers

            string url = "teacherdata/listteachers/" + SearchKey;
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<TeacherDto> Teachers = response.Content.ReadAsAsync<IEnumerable<TeacherDto>>().Result;

            return View(Teachers);
        }

        // GET: Teacher/Details/5
        public ActionResult Details(int id)
        {
            DetailsTeacher ViewModel = new DetailsTeacher();

            //communicate with lesson data to retrieve one lesson
            //curl https://localhost:44304/api/teacherdata/findteacher/{id}

            string url = "teacherdata/findteacher/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            TeacherDto SelectedTeacher = response.Content.ReadAsAsync<TeacherDto>().Result;
            ViewModel.SelectedTeacher = SelectedTeacher;

            // showcase all lessons in the system taught by the selected teacher
            url = "lessondata/listlessonsforteacher/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<LessonDto> LessonsTaught = response.Content.ReadAsAsync<IEnumerable<LessonDto>>().Result; ;
            ViewModel.LessonsTaught = LessonsTaught;

            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Teacher/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Teacher Teacher)
        {
            GetApplicationCookie();

            string url = "teacherdata/addteacher";

            string jsonpayload = jss.Serialize(Teacher);
            //Debug.WriteLine(jsonpayload);

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

        // GET: Teacher/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            //grab teacher information

            string url = "teacherdata/findteacher/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Response code is ");
            //Debug.WriteLine(response.StatusCode);

            TeacherDto SelectedTeacher = response.Content.ReadAsAsync<TeacherDto>().Result;
            //ViewModel.SelectedTeacher = SelectedTeacher;
            //Debug.WriteLine("Lesson recieved: ");
            //Debug.WriteLine(selectedlesson.LessonName);

            return View(SelectedTeacher);
        }

        // POST: Teacher/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Teacher Teacher)
        {
            GetApplicationCookie();

            //send request to the api
            string url = "teacherdata/updateteacher/" + id;

            string jsonplayload = jss.Serialize(Teacher);
            //Debug.WriteLine(jsonplayload);

            HttpContent content = new StringContent(jsonplayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details/" + id);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Teacher/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "teacherdata/findteacher/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            TeacherDto SelectedTeacher = response.Content.ReadAsAsync<TeacherDto>().Result;

            return View(SelectedTeacher);
        }

        // POST: Teacher/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, Teacher Teacher)
        {
            GetApplicationCookie();

            string url = "teacherdata/deleteteacher/" + id;

            //string jsonpayload = jss.Serialize(lesson);
            //Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            //client.PostAsync(url, content);
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
