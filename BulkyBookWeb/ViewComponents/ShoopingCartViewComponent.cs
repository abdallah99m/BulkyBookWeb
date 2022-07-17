using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    [ViewComponent(Name = "ShoppingCart")]
    public class ShoopingCartViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoopingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            { 
                if(HttpContext.Session.GetInt32(SD.SessionCart)!=null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));

                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,_unitOfWork.shoppingCart.GetAll(u=>u.ApplicationUserId==claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));

                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    
    
    }
}
