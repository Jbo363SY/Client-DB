using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClientDatabase.Models;


namespace ClientDatabase.Controllers
{
    public class HomeController : Controller
    {
        //create an instance of a database connection object to allow a connection to the database.
        private ClientDetailsEntities2 db = new ClientDetailsEntities2();

        // GET: Home
        public ActionResult Index(string clientGender, string searchString)
        {
            //gender search
            var GenderList = new List<string>();

            //gets genders from the db and sorts them
            var GenderQuery = from c in db.Clients
                              orderby c.Name
                              select c.Gender;

            // adds unique genders to the list (just a precaution in this case)
            GenderList.AddRange(GenderQuery.Distinct());

            //use the genderList to create a new select list for the dropdown menu in the index view and pass it to the view using the Viewbag
            ViewBag.clientGender = new SelectList(GenderList);

            //LINQ query to select all records in the database
            var clients = from m in db.Clients
                          select m;
            // Name search
            if (!String.IsNullOrEmpty(searchString))
            {                                   //.contains means the title contains any words in the search string.
                clients = clients.Where(n => n.Name.Contains(searchString));
            }

            //last bit of gender search, 
            if (!String.IsNullOrEmpty(clientGender))
            {
                clients = clients.Where(x => x.Gender == clientGender);
            }

            // pass client data to the view
            return View(clients);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Client client)
        {
            //used to finish the validation code. this means is all the entry requirements are met the file is added, if not the user is returned to the same page to try again
            if (ModelState.IsValid)
            {
                // adds and saves the input from the create view
                db.Clients.Add(client);
                db.SaveChanges();

                //returns to the index page (ActionResult Index)
                return RedirectToAction("Index");
            }

            return View(client);

        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Client client = db.Clients.Find(id);

            if (client == null)
            {
                return HttpNotFound();

            }

            return View(client);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Client client = db.Clients.Find(id);

            if (client == null)
            {
                return HttpNotFound();

            }

            return View(client);
        }

        [HttpPost]
        public ActionResult Edit(Client client)
        {
            //used to finish the validation code. this means is all the entry requirements are met the file is changed, if not the user is returned to the same page to try again
            if (ModelState.IsValid)
            {
                //updates and saves the input from the edit view
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();

                //returns to the index page (ActionResult Index)
                return RedirectToAction("Index");
            }

            return View(client);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Client movie = db.Clients.Find(id);

            if (movie == null)
            {
                return HttpNotFound();

            }

            return View(movie);
        }

        //the parameters and name were the same which causes an erroe, the way around this is to change the name and put the actual name you wanted in an ActionName inside the HttpPost. 
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            //find movue to be deleted
            Client client = db.Clients.Find(id);

            //remove the client and save the changes to the database
            db.Clients.Remove(client);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}