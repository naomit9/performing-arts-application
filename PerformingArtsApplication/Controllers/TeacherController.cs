using PerformingArtsApplication.Models;
using PerformingArtsApplication.Models.ViewModels;
using System;
using System.Collections.Generic;
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
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/api/");
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

            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Teacher/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        public ActionResult Create(Teacher Teacher)
        {
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

        // POST: Teacher/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Teacher Teacher)
        {
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

        // GET: Teacher/Delete/5
        public ActionResult Delete(int id)
        {
            string url = "teacherdata/findlesson/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            TeacherDto SelectedTeacher = response.Content.ReadAsAsync<TeacherDto>().Result;

            return View(SelectedTeacher);
        }

        // POST: Teacher/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Teacher Teacher)
        {
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
