
using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        { _db = db; }
        public IActionResult Index()
        {IEnumerable<Category> objCategoryList=_db.categories;
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
                _db.categories.Add(obj);
                _db.SaveChanges();
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
            var categoryFromDb = _db.categories.Find(id);
            if(categoryFromDb == null)
            { return NotFound(); }
            
            return View(categoryFromDb);
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
                _db.categories.Update(obj);
                _db.SaveChanges();
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
            var categoryFromDb = _db.categories.Find(id);
            if (categoryFromDb == null)
            { return NotFound(); }

            return View(categoryFromDb);
        }
        //post
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int ?id)
        {var obj = _db.categories.Find(id);
            if(obj == null)
            { return NotFound();
            }

           
                _db.categories.Remove(obj);
                _db.SaveChanges();
            TempData["success"] = "Category Delete successfully";

            return RedirectToAction("Index");
            
            
        }
    }
}
