using BLL;
using BOL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserInterface.HelperMethods;
using UserInterface.ViewModels;
using WebAPI.Data;

namespace UserInterface.Controllers
{
   
    public class ItemsController : Controller
    {
        private IItemsBs objItemsBs;
        private readonly Helpers apiHelper;
        public ItemsController(IItemsBs _objItemsBs)
        {
            objItemsBs = _objItemsBs;
            apiHelper = new Helpers();
        }

        public IActionResult Test()
        {
            return View();
          
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //throw new Exception("Custom Exception.");
            try
            {               

                var objListVM = new List<ItemsVM>();
               
                //objItemsBs.GetAll().ToList().ForEach(x =>
                //{
                //    objListVM.Add(new ItemsVM() { ItemId = x.ItemId, Name = x.Name, Description = x.Description, Price = x.Price, Course = x.Course, InStock = x.InStock });
                //});

                //Get Items from API
                HttpClient client = apiHelper.Initial();

                HttpResponseMessage res =await client.GetAsync("api/Items");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    var objList = JsonConvert.DeserializeObject <List<Items>>(result);

                    objList.ToList().ForEach(x => 
                    {
                        objListVM.Add(new ItemsVM() { ItemId = x.ItemId, Name = x.Name, Description = x.Description, Price = x.Price, Course = x.Course, InStock = x.InStock });
                    });
                }

                return View(objListVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
                return View();

            }
        }

        [HttpGet]
        public IActionResult CreateOrEdit(int id)
        {
            ItemsVM objVM = new ItemsVM();
            List<SelectListItem> courseList = new List<SelectListItem>();
            courseList.Add(new SelectListItem() { Text = "Starter", Value = "Starter" });
            courseList.Add(new SelectListItem() { Text = "Main", Value = "Main" });
            courseList.Add(new SelectListItem() { Text = "Sides", Value = "Sides" });
            courseList.Add(new SelectListItem() { Text = "Drinks", Value = "Drinks" });

            if (id > 0) //Update
            {
                var obj = objItemsBs.GetById(id);
                if (obj != null)
                {
                    objVM.ItemId = obj.ItemId; objVM.Name = obj.Name; objVM.Description = obj.Description;
                    objVM.Price = obj.Price; objVM.Course = obj.Course; objVM.InStock = obj.InStock;
                    objVM.CourseList = new SelectList(courseList, "Text", "Value");
                }
            }

            objVM.CourseList = new SelectList(courseList, "Text", "Value");
            //ViewBag.CourseList = new SelectList(courseList, "Text", "Value");


            return View(objVM);
        }
        [HttpPost]
        public IActionResult Save(ItemsVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Items obj = new Items() { ItemId = model.ItemId, Name = model.Name, Description = model.Description, Course = model.Course, Price = model.Price, InStock = model.InStock, IsActive = true };

                    if (model.ItemId == 0) //Insert
                        objItemsBs.Insert(obj);
                    else //Update
                        objItemsBs.Update(obj);

                    TempData["SuccessMsg"] = "Record saved successfully.";
                    return RedirectToAction("Index", "Items");
                }
                else
                {
                    return View("CreateOrEdit");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
                return RedirectToAction("CreateOrEdit", "Items");
            }
        }
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                objItemsBs.Delete(id);
                TempData["SuccessMsg"] = "Record deleted successfully.";
                return RedirectToAction("Index", "Items");
            }
            else
            {
                TempData["ErrorMsg"] = "Record can not be deleted.";
                return RedirectToAction("Index", "Items");
            }
        }
    }
}
