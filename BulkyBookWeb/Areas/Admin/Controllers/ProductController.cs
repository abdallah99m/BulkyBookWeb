
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
    private IWebHostEnvironment _hostEnviroment;
        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment hostEnvironment)
        { _unitOfWork = unitOfWork;
        _hostEnviroment = hostEnvironment;
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
            productVM.product=_unitOfWork.product.GetFirstOrDefault(u=>u.Id == id);
            return View(productVM);
             }
          
            
            return View(productVM);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            
            
            if (ModelState.IsValid)
            {
            string wwwRootPath = _hostEnviroment.WebRootPath;
            if(file!=null)
            { string fileName=Guid.NewGuid().ToString();
                var uploads=Path.Combine(wwwRootPath,@"images\products\");
                var extension=Path.GetExtension(file.Name);


                if(obj.product.ImageUrl!=null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(oldImagePath))
                    { 
                        System.IO.File.Delete(oldImagePath); 
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension),FileMode.Create))
                { file.CopyTo(fileStreams); 
                }
                obj.product.ImageUrl = @"\images\products\" + fileName + extension;
             }
            if (obj.product.Id == 0)
            {
                _unitOfWork.product.Add(obj.product);

            }
            else { _unitOfWork.product.update(obj.product); }
            _unitOfWork.Save();
                TempData["success"] = "product Created successfully";

                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //Get
      

    #region API CALLS
    [HttpGet]
    public IActionResult getAll()
    {
        var productList=_unitOfWork.product.GetAll(includeProperties:"Category,CoverType"); 
        return Json(new {data=productList});
    }


    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.product.GetFirstOrDefault(c => c.Id == id);
        if (obj == null)
        {
            return Json(new {success=false,message="Error while deleting"});
        }
        var oldImagePath = Path.Combine(_hostEnviroment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.product.Remove(obj);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });


    }

    #endregion
}

