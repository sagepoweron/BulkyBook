using BulkyBook.Models;
using BulkyBook.Data;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;//116
using System.Data;

//123
namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]//116
	public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitofwork;

        public CompanyController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
			if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company company = _unitofwork.Company.GetFirstOrDefault(u => u.Id == id);
                return View(company);
			}
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitofwork.Company.Add(obj);
                }
                else
                {
                    _unitofwork.Company.Update(obj);
                }

                _unitofwork.Save();
                TempData["success"] = "Company created successfully.";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companylist = _unitofwork.Company.GetAll().ToList();
            return Json(new { data = companylist });
        }

		//POST
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var obj = _unitofwork.Company.GetFirstOrDefault(u => u.Id == id);
			if (obj == null)
			{
                return Json(new {success = false, message = "Error while deleting."});
			}

			_unitofwork.Company.Remove(obj);
			_unitofwork.Save();
			return Json(new { success = true, message = "Delete successful." });
		}
		#endregion
	}
}
