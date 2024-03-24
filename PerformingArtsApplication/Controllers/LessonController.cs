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
        public ActionResult List()
        {
            //communicate with lesson data to retrieve list of animals
            //curl https://localhost:44304/api/lessondata/listlessons

            string url = "listlessons";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<LessonDto> Lessons = response.Content.ReadAsAsync<IEnumerable<LessonDto>>().Result;

            return View(Lessons);
        }

        // GET: Lesson/Details/5
        public ActionResult Details(int id)
        {
            DetailsLesson ViewModel = new DetailsLesson();

            //communicate with lesson data to retrieve list of animals
            //curl https://localhost:44304/api/lessondata/findlesson/{id}

            string url = "findlesson/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            LessonDto SelectedLesson = response.Content.ReadAsAsync<LessonDto>().Result;
            ViewModel.SelectedLesson = SelectedLesson;

            return View(ViewModel);
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
            string url = "addlesson";

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
            UpdateLesson ViewModel = new UpdateLesson();

            //grab lesson information

            string url = "findlesson/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("Response code is ");
            //Debug.WriteLine(response.StatusCode);

            LessonDto SelectedLesson = response.Content.ReadAsAsync<LessonDto>().Result;
            ViewModel.SelectedLesson = SelectedLesson;
            //Debug.WriteLine("Lesson recieved: ");
            //Debug.WriteLine(selectedlesson.LessonName);

            return View(ViewModel);
        }

        // POST: Lesson/Update/5
        [HttpPost]
        public ActionResult Update(int id, Lesson Lesson)
        {
            //send request to the api
            string url = "updatelesson/" + id;

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
            string url = "findlesson/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            LessonDto SelectedLesson = response.Content.ReadAsAsync<LessonDto>().Result;

            return View(SelectedLesson);
        }

        // POST: Lesson/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            string url = "deletelesson/" + id;

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
