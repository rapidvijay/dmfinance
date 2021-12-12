using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModificationHousingFeesAdmin.Models;
using PagedList;

namespace ModificationHousingFeesAdmin.Controllers
{
    [ValidateInput(true)]
    public class CommunityPlacesController : Controller
    {
        private ModificationHousingFeesEntities db = new ModificationHousingFeesEntities();

        // GET: CommunityPlaces
        public ActionResult Index()
        {
            PagedList<Community_Places> model = null;
            List<Community_Places> communityPlaces = db.Community_Places.ToList();
            if (communityPlaces != null && communityPlaces.Count() > 0)
            {
                model = new PagedList<Community_Places>(communityPlaces, 1, communityPlaces.Count());
            }

            return View(model);
        }

        // GET: CommunityPlaces/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Community_Places community_Places = db.Community_Places.Find(id);
            if (community_Places == null)
            {
                return HttpNotFound();
            }
            return View(community_Places);
        }

        // GET: CommunityPlaces/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommunityPlaces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Arabic_Name,English_Name,Created_By,Creation_Date,Update_Date,Updated_By")] Community_Places community_Places)
        {
            if (ModelState.IsValid)
            {
                //##########################################################################
                community_Places.Created_By = Common.User.USER_NAME;
                community_Places.Creation_Date = DateTime.Now;
                //##########################################################################
                db.Community_Places.Add(community_Places);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(community_Places);
        }

        // GET: CommunityPlaces/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Community_Places community_Places = db.Community_Places.Find(id);
            if (community_Places == null)
            {
                return HttpNotFound();
            }
            return View(community_Places);
        }

        // POST: CommunityPlaces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Arabic_Name,English_Name,Created_By,Creation_Date,Update_Date,Updated_By")] Community_Places community_Places)
        {
            if (ModelState.IsValid)
            {
                //#####################################################################
                community_Places.Updated_By = Common.User.USER_NAME;
                community_Places.Update_Date = DateTime.Now;
                //#####################################################################
                db.Entry(community_Places).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(community_Places);
        }

        // GET: CommunityPlaces/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Community_Places community_Places = db.Community_Places.Find(id);
            if (community_Places == null)
            {
                return HttpNotFound();
            }
            return View(community_Places);
        }

        // POST: CommunityPlaces/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Community_Places community_Places = db.Community_Places.Find(id);
            db.Community_Places.Remove(community_Places);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
