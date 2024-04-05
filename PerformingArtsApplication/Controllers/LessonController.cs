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
    public class LessonController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static LessonController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44304/api/");
        }

        // GET: Lesson/List
        public ActionResult List(string SearchKey = null)
        {
            //communicate with lesson data to retrieve list of lessons
            //curl https://localhost:44304/api/lessondata/listlessons

            string url = "lessondata/listlessons/" + SearchKey;
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<LessonDto> Lessons = response.Content.ReadAsAsync<IEnumerable<LessonDto>>().Result;

            return View(Lessons);
        }

        // GET: Lesson/Details/5
        public ActionResult Details(int id)
        {
            DetailsLesson ViewModel = new DetailsLesson();

            //communicate with lesson data to retrieve one lesson
            //curl https://localhost:44304/api/lessondata/findlesson/{id}

            string url = "lessondata/findlesson/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            LessonDto SelectedLesson = response.Content.ReadAsAsync<LessonDto>().Result;
            ViewModel.SelectedLesson = SelectedLesson;

            // show associated students with this lesson
            url = "studentdata/liststudentsforlesson/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<StudentDto> StudentsInLesson = response.Content.ReadAsAsync<IEnumerable<StudentDto>>().Result;

            ViewModel.StudentsInLesson = StudentsInLesson;

            url = "studentdata/liststudentsnotinlesson/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<StudentDto> AvailableStudents = response.Content.ReadAsAsync<IEnumerable<StudentDto>>().Result;

            ViewModel.AvailableStudents = AvailableStudents;

            return View(ViewModel);
        }

        //POST: Lesson/Associate/{lessonid}
        [HttpPost]
        public ActionResult Associate(int id, int StudentId)
        {
            // call api to add student to lesson
            string url = "lessondata/addstudenttolesson/" + id + "/" + StudentId;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //GET: Lesson/UnAssociate/{id}?StudentId={studentid}
        [HttpGet]
        public ActionResult UnAssociate(int id, int StudentId)
        {
            // call api to remove student from lesson
            string url = "lessondata/removestudentfromlesson/" + id + "/" + StudentId;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Lesson/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Lesson/Create
        [HttpPost]
        public ActionResult Create(Lesson Lesson)
        {
            //Debug.WriteLine("the jsonpayload is: ");
            //Debug.WriteLine(Lesson.LessonName);
            //add new lesson to system 
            //curl -H "Content-type:application/json" -d @lesson.json https://localhost:44304/api/lessondata/addlesson
            string url = "lessondata/addlesson";

            string jsonpayload = jss.Serialize(Lesson);
            //Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
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

        // GET: Lesson/Edit/5
        public ActionResult Edit(int id)
        {
            //grab lesson information

            string url = "lessondata/findlesson/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Response code is ");
            //Debug.WriteLine(response.StatusCode);

            LessonDto SelectedLesson = response.Content.ReadAsAsync<LessonDto>().Result;
            //ViewModel.SelectedLesson = SelectedLesson;
            //Debug.WriteLine("Lesson recieved: ");
            //Debug.WriteLine(selectedlesson.LessonName);

            return View(SelectedLesson);
        }

        // POST: Lesson/Update/5
        [HttpPost]
        public ActionResult Update(int id, Lesson Lesson)
        {
            //send request to the api
            string url = "lessondata/updatelesson/" + id;

            string jsonplayload = jss.Serialize(Lesson);
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

        // GET: Lesson/Delete/5
        public ActionResult Delete(int id)
        {
            string url = "lessondata/findlesson/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            LessonDto SelectedLesson = response.Content.ReadAsAsync<LessonDto>().Result;

            return View(SelectedLesson);
        }

        // POST: Lesson/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Lesson Lesson)
        {
            string url = "lessondata/deletelesson/" + id;

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
