using BulkyBook.Models;
using BulkyBook.Data;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;//116
using System.Data;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]//116
	public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _hostenvironment;

        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment hostenvironment)
        {
            _unitofwork = unitofwork;
            _hostenvironment = hostenvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productviewmodel = new()
            {
                Product = new(),
                CategoryList = _unitofwork.Category.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CoverTypeList = _unitofwork.CoverType.GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };


			if (id == null || id == 0)
            {
                //create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                //return View(productviewmodel);
            }
            else
            {
                productviewmodel.Product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == id);
				//return View(productviewmodel);
				//update product
			}

            return View(productviewmodel);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostenvironment.WebRootPath;
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Product.ImageURL != null)
                    {
                        var oldimagepath = Path.Combine(wwwRootPath, obj.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimagepath))
                        {
                            System.IO.File.Delete(oldimagepath);
                        }
                    }

                    using (var filestreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        file.CopyTo(filestreams);
                    }
                    obj.Product.ImageURL = @"\images\products\" + filename + extension;
                }

                if (obj.Product.Id == 0)
                {
                    _unitofwork.Product.Add(obj.Product);
                }
                else
                {
                    _unitofwork.Product.Update(obj.Product);
                }

                _unitofwork.Save();
                TempData["success"] = "Product created successfully.";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productlist = _unitofwork.Product.GetAll(includeproperties:"Category,CoverType");
            return Json(new { data = productlist });
        }

		//POST
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var obj = _unitofwork.Product.GetFirstOrDefault(u => u.Id == id);
			if (obj == null)
			{
                return Json(new {success = false, message = "Error while deleting."});
			}

			var oldimagepath = Path.Combine(_hostenvironment.WebRootPath, obj.ImageURL.TrimStart('\\'));
			if (System.IO.File.Exists(oldimagepath))
			{
				System.IO.File.Delete(oldimagepath);
			}

			_unitofwork.Product.Remove(obj);
			_unitofwork.Save();
			return Json(new { success = true, message = "Delete successful." });
		}
		#endregion
	}
}
