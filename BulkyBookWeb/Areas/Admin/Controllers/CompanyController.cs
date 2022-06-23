
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Controllers;
    [Area("Admin")]

    public class CompanyController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        { 
        _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //IEnumerable<Product> objCategoryList= _unitOfWork.product.GetAll();
            return View(/*objCategoryList*/);
        }
        //Get
     
        //Get
        public IActionResult Upsert(int? id)
        {
       Company company = new();
        if (id == null|| id ==0)
            {
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
                return View(company);
            }
             else
             {
            company=_unitOfWork.company.GetFirstOrDefault(u=>u.Id == id);
            return View(company);
             }
          
            
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            
            
            if (ModelState.IsValid)
            {
            if (obj.Id == 0)
            {
                _unitOfWork.company.Add(obj);
                TempData["success"] = "Company Created successfully";


            }
            else { _unitOfWork.company.update(obj);
                TempData["success"] = "Company Update successfully";
            }
            _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //Get
      

    #region API CALLS
    [HttpGet]
    public IActionResult getAll()
    {
        var CompanyList=_unitOfWork.company.GetAll(); 
        return Json(new {data= CompanyList });
    }


    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.company.GetFirstOrDefault(c => c.Id == id);
        if (obj == null)
        {
            return Json(new {success=false,message="Error while deleting"});
        }
       

        _unitOfWork.company.Remove(obj);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });


    }

    #endregion
}

