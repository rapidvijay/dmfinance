using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataTables.Mvc;
using ModificationHousingFeesAdmin.Models;
using ModificationHousingFeesAdmin.ViewModel;
using PagedList;


namespace ModificationHousingFeesAdmin.Controllers
{
    [ValidateInput(true)]
    public class AreasController : Controller
    {
        private ModificationHousingFeesEntities db = new ModificationHousingFeesEntities();


        public ActionResult addCommunityPlace(int areaId, String communityPlaceIds = null)
        {
            List<string> arrayOfId = new List<string>();
            if (!string.IsNullOrEmpty(communityPlaceIds))
            {
                arrayOfId = communityPlaceIds.Split(',').ToList();
            }

            List<Community_Places> communityPlacesData = db.Community_Places.
                                               Where(x => !arrayOfId.
                                               Contains(x.Id.ToString())).
                                               ToList();

            PagedList<Community_Places> model = null;
            if (communityPlacesData != null && communityPlacesData.Count() > 0)
            {
                model = new PagedList<Community_Places>(communityPlacesData, 1, communityPlacesData.Count());
            }

            ViewBag.Message = "add CommunityPlace.";
            ViewBag.areaId = areaId;
            return PartialView(model);
        }


        [HttpPost]
        public async Task<ActionResult> saveCommunityPlace(int communityPlaceId, int areaId)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Community_Places area = db.Community_Places.Where(x => x.Id == communityPlaceId).FirstOrDefault();
            if (area == null)
            {
                throw new HttpException("The Community Place is not Valid");
            }

            Area_CommunitiesPlaces areaCommunitiesPlace = new Area_CommunitiesPlaces();
            areaCommunitiesPlace.Community_Id = communityPlaceId;
            areaCommunitiesPlace.Area_Id = areaId;
            areaCommunitiesPlace.Created_By = Common.User.USER_NAME;
            areaCommunitiesPlace.Creation_Date = DateTime.Now;

            db.Area_CommunitiesPlaces.Add(areaCommunitiesPlace);
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to add the Area");
                return View();
            }

            return Content("success");
        }


        [HttpPost]
        public async Task<ActionResult> deleteAreaCommunityPlacesById(int areaCommunitiesPlaceId, int areaId)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Area_CommunitiesPlaces area_CommunitiesPlaces = db.Area_CommunitiesPlaces.Where(x => x.Id == areaCommunitiesPlaceId).FirstOrDefault();

            db.Entry(area_CommunitiesPlaces).State = EntityState.Deleted;
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to Delete the Area Community Place");
                return View();
            }

            return Content(area_CommunitiesPlaces.Community_Id.ToString());
        }


        //Install-Package Mvc.JQuery.Datatables -Version 1.5.44
        //add dll manually from the example project 
        //https://www.c-sharpcorner.com/article/ajax-crud-operation-with-jquery-datatables-in-asp-net-mvc-5-for-beginners/
        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, AreaCommunitiesPlacesModelView searchViewModel, int? id)
        {
            IQueryable<AreaCommunitiesPlacesModelView> query =  db.Area_CommunitiesPlaces.
                                                                Where(x => x.Area_Id == id).
                                                                Select(x => new ViewModel.AreaCommunitiesPlacesModelView
                                                                {
                                                                areaArabicName =  x.Community_Places.Arabic_Name,
                                                                areaEnglishName = x.Community_Places.English_Name,
                                                                Id = x.Id
                                                                });

            //IQueryable<property_Areas> query = db.property_Areas.
            //                              Where(x => x.Property_Id == id);
                                           

            var totalCount = query.Count();

            // searching and sorting
            query = searchAreaCommunitiesPlaces(requestModel, searchViewModel, query);
            var filteredCount = query.Count();
            //query = query.OrderBy(x => x.Id);
            // Paging
            query = query.Skip(requestModel.Start).Take(requestModel.Length);

            var data = query.ToList();
            var json = Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);

            return json;
        }

        private IQueryable<AreaCommunitiesPlacesModelView> searchAreaCommunitiesPlaces(IDataTablesRequest requestModel, AreaCommunitiesPlacesModelView searchViewModel, IQueryable<AreaCommunitiesPlacesModelView> query)
        {

            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                int outVal = 0;
                if (int.TryParse(value, out outVal))
                {
                    query = query.Where(p => p.areaArabicName.Contains(value) ||
                                         p.areaEnglishName.Contains(value) ||
                                         p.Id == outVal
                                   );
                }
                else
                {
                    query = query.Where(p => p.areaArabicName.Contains(value) ||
                                         p.areaEnglishName.Contains(value));
                }
            }

            /***** Advanced Search ******/
            var filteredCount = query.Count();

            // Sort
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            foreach (var column in sortedColumns)
            {
                orderByString += orderByString != String.Empty ? "," : "";
                orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
            }

            query = query.OrderBy(x => x.Id);//orderByString == string.Empty ? "Id asc" : orderByString

            return query;

        }

        private string getCommunitiesPlaces(int areaId)
        {
            List<Area_CommunitiesPlaces> area_CommunitiesPlaces = db.Area_CommunitiesPlaces.Where(x => x.Area_Id == areaId).ToList();
            string CommunitiesPlaces = "";
            if (area_CommunitiesPlaces != null)
            {
                foreach (var areaCommunitiesPlace in area_CommunitiesPlaces)
                {
                    if (CommunitiesPlaces == "")
                    {
                        CommunitiesPlaces = areaCommunitiesPlace.Community_Places.Id.ToString();
                    }
                    else
                    {
                        CommunitiesPlaces = CommunitiesPlaces + "," + areaCommunitiesPlace.Community_Places.Id.ToString();
                    }
                }
            }

            return CommunitiesPlaces;
        }


        // GET: Areas
        public ActionResult Index()
        {
            PagedList<Area> model = null;
            List<Area> areas = db.Areas.ToList();
            if (areas != null && areas.Count() > 0)
            {
                model = new PagedList<Area>(areas, 1, areas.Count());
            }

            return View(model);
        }

        // GET: Areas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }

            ViewBag.display = "display:none";
            return View(area);
        }

        // GET: Areas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Areas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Arabic_Name,English_Name,Created_By,Creation_Date,Update_Date,Updated_By")] Area area)
        {
            if (ModelState.IsValid)
            {
                //##########################################################################
                area.Created_By = Common.User.USER_NAME;
                area.Creation_Date = DateTime.Now;
                //##########################################################################
                db.Areas.Add(area);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(area);
        }

        // GET: Areas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }

            //#################################################################################
            ViewBag.communitiesPlacesIds = getCommunitiesPlaces(area.Id);
            //#################################################################################

            return View(area);
        }

        // POST: Areas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Arabic_Name,English_Name,Created_By,Creation_Date,Update_Date,Updated_By")] Area area)
        {
            if (ModelState.IsValid)
            {
                //#####################################################################
                area.Updated_By = Common.User.USER_NAME;
                area.Update_Date = DateTime.Now;
                //#####################################################################
                db.Entry(area).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(area);
        }

        // GET: Areas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }
            return View(area);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Area area = db.Areas.Find(id);
            db.Areas.Remove(area);
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
