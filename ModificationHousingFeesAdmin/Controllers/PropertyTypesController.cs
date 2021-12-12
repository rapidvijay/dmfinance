using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModificationHousingFeesAdmin.Models;
using PagedList;
using DataTables.Mvc;
using ModificationHousingFeesAdmin.ViewModel;
using System.Threading.Tasks;
using PagedList;

namespace ModificationHousingFeesAdmin.Controllers
{
    [ValidateInput(true)]
    public class PropertyTypesController : Controller
    {
        private ModificationHousingFeesEntities db = new ModificationHousingFeesEntities();


        public ActionResult addArea(int propertyId , String areaIds = null)
        {
            List<string> arrayOfId = new List<string>();
            if (!string.IsNullOrEmpty(areaIds))
            {
                arrayOfId = areaIds.Split(',').ToList();
            }

            List<Area> areasData = db.Areas.
                                   Where(x=> !arrayOfId.
                                   Contains(x.Id.ToString())).
                                   ToList();

            PagedList<Area> model = null;
            if (areasData != null && areasData.Count() > 0)
            {
                model = new PagedList<Area>(areasData, 1, areasData.Count());
            }

            ViewBag.Message = "add Area.";
            ViewBag.propertyId = propertyId;
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveNewArea(int areaId , int propertyId)
        {
            if (!ModelState.IsValid) {
                return View();
            }

            Area area = db.Areas.Where(x => x.Id == areaId).FirstOrDefault();
            if (area == null)
            {
                throw new HttpException("The Area Id is not Valid");
            }

            property_Areas propertyAreas = new property_Areas();
            propertyAreas.Property_Id = propertyId;
            propertyAreas.Area_Id = areaId;
            propertyAreas.Created_By = Common.User.USER_NAME;
            propertyAreas.Creation_Date = DateTime.Now;

            db.property_Areas.Add(propertyAreas);
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
        public async Task<ActionResult> deletePropertyAreaById(int propertyAreaId, int propertyId)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            property_Areas propertyAreas = db.property_Areas.Where(x => x.Id == propertyAreaId).FirstOrDefault();

            db.Entry(propertyAreas).State = EntityState.Deleted; 
            var task = db.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to Delete the Area");
                return View();
            }

            return Content(propertyAreas.Area_Id.ToString());
        }

        //Install-Package Mvc.JQuery.Datatables -Version 1.5.44
        //add dll manually from the example project 
        //https://www.c-sharpcorner.com/article/ajax-crud-operation-with-jquery-datatables-in-asp-net-mvc-5-for-beginners/
        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, propertyAreaModelView searchViewModel, int? id)
        {
            IQueryable<propertyAreaModelView> query = db.property_Areas.
                                            Where(x => x.Property_Id == id).
                                            Select(x => new ViewModel.propertyAreaModelView
                                                {
                                            areaArabicName =  x.Area.Arabic_Name,
                                           areaEnglishName = x.Area.English_Name,
                                           Id = x.Id
                                            });

            //IQueryable<property_Areas> query = db.property_Areas.
            //                              Where(x => x.Property_Id == id);
                                           


            var totalCount = query.Count();

            // searching and sorting
            query = searchAreas(requestModel, searchViewModel, query);
            var filteredCount = query.Count();
            //query = query.OrderBy(x => x.Id);
            // Paging
            query = query.Skip(requestModel.Start).Take(requestModel.Length);


            var data = query.ToList();
            var json = Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);

            return json;

        }

        private IQueryable<propertyAreaModelView> searchAreas(IDataTablesRequest requestModel, propertyAreaModelView searchViewModel, IQueryable<propertyAreaModelView> query)
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

        private string getAreaIds(int propertyId)
        {
            List<property_Areas> propertyAreas = db.property_Areas.Where(x => x.Property_Id == propertyId).ToList();
            string areaIds = "";
            if (propertyAreas != null)
            {
                foreach (var propertyArea in propertyAreas)
                {
                    if (areaIds == "")
                    {
                        areaIds = propertyArea.Area.Id.ToString();
                    }
                    else
                    {
                        areaIds = areaIds + "," + propertyArea.Area.Id.ToString();
                    }
                }
            }

            return areaIds;
        }



        // GET: PropertyTypes
        public ActionResult Index()
        {
            PagedList<Property_Types> model = null;
            List<Property_Types> propertyTypes = db.Property_Types.ToList();
            if (propertyTypes != null && propertyTypes.Count() > 0)
            {
                model = new PagedList<Property_Types>(propertyTypes, 1, propertyTypes.Count());
            }

            return View(model);
        }

        // GET: PropertyTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property_Types property_Types = db.Property_Types.Find(id);
            if (property_Types == null)
            {
                return HttpNotFound();
            }

            ViewBag.display = "display:none";
            return View(property_Types);
        }

        // GET: PropertyTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropertyTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Arabic_Name,English_Name,Created_By,Updated_By,Creation_Date,Update_Date")] Property_Types property_Types)
        {
            if (ModelState.IsValid)
            {
                //##########################################################################
                property_Types.Created_By = Common.User.USER_NAME;
                property_Types.Creation_Date = DateTime.Now;
                //##########################################################################
                db.Property_Types.Add(property_Types);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(property_Types);
        }

        // GET: PropertyTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property_Types property_Types = db.Property_Types.Find(id);
            if (property_Types == null)
            {
                return HttpNotFound();
            }

            //#################################################################################
            ViewBag.areaIds = getAreaIds(property_Types.Id);
            //#################################################################################
            return View(property_Types);
        }

        // POST: PropertyTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Arabic_Name,English_Name,Created_By,Updated_By,Creation_Date,Update_Date")] Property_Types property_Types)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //#####################################################################
                    property_Types.Updated_By = Common.User.USER_NAME;
                    property_Types.Update_Date = DateTime.Now;
                    //#####################################################################
                    db.Entry(property_Types).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            return View(property_Types);
        }


        // GET: PropertyTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property_Types property_Types = db.Property_Types.Find(id);
            if (property_Types == null)
            {
                return HttpNotFound();
            }
            return View(property_Types);
        }

        // POST: PropertyTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Property_Types property_Types = db.Property_Types.Find(id);
            db.Property_Types.Remove(property_Types);
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
