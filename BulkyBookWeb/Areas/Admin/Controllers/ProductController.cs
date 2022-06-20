
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Controllers;
    [Area("Admin")]

    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        { _unitOfWork = unitOfWork; }
        public IActionResult Index()
        {
            IEnumerable<Product> objCategoryList= _unitOfWork.product.GetAll();
            return View(objCategoryList);
        }
        //Get
     
        //Get
        public IActionResult Upsert(int? id)
        {
        ProductVM productVM = new()
        {
            product = new(),
            CategoryList = _unitOfWork.category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            CoverTypeList = _unitOfWork.coverType.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            })
        };
        if (id == null|| id ==0)
            {
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            }
             else
             {

             }
          
            
            return View(productVM);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj,IFormFile file)
        {
            
            
            if (ModelState.IsValid)
            {
                //_unitOfWork.product.update(obj);
                _unitOfWork.Save();
                TempData["success"] = "product update successfully";

                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //Get
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var CoverTypeFromDb = _unitOfWork.product.GetFirstOrDefault(c => c.Id == id);
            if (CoverTypeFromDb == null)
            { return NotFound(); }

            return View(CoverTypeFromDb);
        }
        //post
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int ?id)
        {
            var obj = _unitOfWork.product.GetFirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }


            _unitOfWork.product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "product Delete successfully";

            return RedirectToAction("Index");
            
            
        }
    }

