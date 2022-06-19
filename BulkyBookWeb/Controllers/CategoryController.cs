
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryRepository _db;
        public CategoryController(ICategoryRepository db)
        { _db = db; }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList=_db.GetAll();
            return View(objCategoryList);
        }
        //Get
        public IActionResult Create()
        {
            return View();
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {   if(obj.Name ==obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "the DisplayOrder cannot exactly match the Nmae.");

            }
            if (ModelState.IsValid)
            {
                _db.Add(obj);
                _db.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //Get
        public IActionResult Edit(int? id)
        {
            if(id == null|| id ==0)
            { 
                return NotFound();
            }
            //var categoryFromDb = _db.categories.Find(id);
            var category = _db.GetFirstOrDefault(c => c.Id == id);
            if(category == null)
            { return NotFound(); }
            
            return View(category);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "the DisplayOrder cannot exactly match the Nmae.");

            }
            if (ModelState.IsValid)
            {
                _db.update(obj);
                _db.Save();
                TempData["success"] = "Category update successfully";

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
            var categoryFromDb = _db.GetFirstOrDefault(c => c.Id == id);
            if (categoryFromDb == null)
            { return NotFound(); }

            return View(categoryFromDb);
        }
        //post
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int ?id)
        {var obj = _db.GetFirstOrDefault(c => c.Id == id);
            if (obj == null)
            { return NotFound();
            }

           
                _db.Remove(obj);
                _db.Save();
            TempData["success"] = "Category Delete successfully";

            return RedirectToAction("Index");
            
            
        }
    }
}
