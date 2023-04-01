using BulkyBook.Data.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofwork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitofwork)
        {
            _logger = logger;
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productlist = _unitofwork.Product.GetAll(includeproperties:"Category,CoverType");
            return View(productlist);
        }

		public IActionResult Details(int id)
		{
            ShoppingCart cart = new()
            {
                Count = 1,
                Product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == id, includeproperties: "Category,CoverType")
            };
            
			return View(cart);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}