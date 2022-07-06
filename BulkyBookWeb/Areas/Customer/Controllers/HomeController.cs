using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Controllers;
[Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.product.GetAll(includeProperties:"Category,CoverType");
            return View(productList);
        }

    public IActionResult Details(int productId)
    {
        ShoppingCart cartObj = new()
        {Count = 1,
        ProductId = productId,
            Product = _unitOfWork.product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType"),
        };
        return View(cartObj);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimIdentity=(ClaimsIdentity)User.Identity;
        var claim=claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserId = claim.Value;

        ShoppingCart cartFromDb = _unitOfWork.shoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);
        if (cartFromDb == null)
        {

            _unitOfWork.shoppingCart.Add(shoppingCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
        }
        else
        {
            _unitOfWork.shoppingCart.IncrementCount(cartFromDb,shoppingCart.Count);
            _unitOfWork.Save();
        }
        _unitOfWork.Save();
        ShoppingCart cartObj = new()
        {
            Count = 1,
           // Product = _unitOfWork.product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType"),
        };
        return RedirectToAction(nameof(Index));
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
