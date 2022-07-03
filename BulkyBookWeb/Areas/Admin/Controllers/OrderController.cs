using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        { _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            orderVM = new OrderVM()
            {
                orderHeader = _unitOfWork.orderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                orderDetails = _unitOfWork.orderDeatil.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),


            };
            return View(orderVM);
        }
        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Details_PAY_NOW(int orderId)
        {

            orderVM.orderHeader = _unitOfWork.orderHeader.GetFirstOrDefault(u => u.Id == orderVM.orderHeader.Id, includeProperties: "ApplicationUser");



            orderVM.orderDetails = _unitOfWork.orderDeatil.GetAll(u => u.OrderId == orderVM.orderHeader.Id, includeProperties: "Product");
            //////////// strip session ////////////
            var domain = "https://localhost:7259/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                LineItems = new List<SessionLineItemOptions>()
                ,

                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={orderVM.orderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.orderHeader.Id}",
            };
            foreach (var item in orderVM.orderDetails)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Tittle,
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.orderHeader.UpdateStripPaymentID(orderVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


        }


        public IActionResult PaymentConfirmation(int orderHeaderid)
        {
            OrderHeader orderHeader = _unitOfWork.orderHeader.GetFirstOrDefault(x => x.Id == orderHeaderid);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.orderHeader.UpdateStatus(orderHeaderid, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
           

            return View(orderHeaderid);
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail()
        {
            var orderHEaderFromDb = _unitOfWork.orderHeader.GetFirstOrDefault(u => u.Id == orderVM.orderHeader.Id,tracked: false);
            orderHEaderFromDb.Name = orderVM.orderHeader.Name;
            orderHEaderFromDb.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            orderHEaderFromDb.StreetAddress = orderVM.orderHeader.StreetAddress;
            orderHEaderFromDb.City = orderVM.orderHeader.City;
            orderHEaderFromDb.State = orderVM.orderHeader.State;
            orderHEaderFromDb.PostalCode = orderVM.orderHeader.PostalCode;
            if(orderVM.orderHeader.Carrier!=null)
            { orderHEaderFromDb.Carrier= orderVM.orderHeader.Carrier; }
            
            if(orderVM.orderHeader.TrackingNumber!=null)
            {
                orderHEaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber; 
            }
            _unitOfWork.orderHeader.update(orderHEaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details update Successfully";
            return RedirectToAction("Details", "Order", new {orderId=orderHEaderFromDb.Id});
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        [ValidateAntiForgeryToken]

        public IActionResult StartProcessing()
        {
            _unitOfWork.orderHeader.UpdateStatus(orderVM.orderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details update Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.orderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        [ValidateAntiForgeryToken]

        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.orderHeader.GetFirstOrDefault(u => u.Id == orderVM.orderHeader.Id, tracked: false);
            orderHeader.TrackingNumber=orderVM.orderHeader.TrackingNumber;
            orderHeader.Carrier=orderVM.orderHeader.Carrier;
            orderHeader.OrderStatus=SD.StatusInProcess;
            orderHeader.ShippingDate=DateTime.Now;

            if(orderHeader.PaymentStatus==SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate=DateTime.Now.AddDays(30);
            }

            _unitOfWork.orderHeader.update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.orderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        [ValidateAntiForgeryToken]

        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.orderHeader.GetFirstOrDefault(u => u.Id == orderVM.orderHeader.Id, tracked: false);
            if(orderHeader.PaymentStatus==SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund=service.Create(options);

                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);


            }
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.orderHeader.Id });
        }




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.orderHeader.GetAll(includeProperties: "ApplicationUser");
            }else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.orderHeader.GetAll(u => u.ApplicationUserId==claim.Value, includeProperties: "ApplicationUser") ;


            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);

                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus==SD.StatusShipped);

                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);

                    break;
                default:
                    
                    break;
            }



            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
