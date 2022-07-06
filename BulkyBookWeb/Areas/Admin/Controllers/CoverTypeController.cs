
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers;
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

public class CoverTypeController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        { _unitOfWork = unitOfWork; }
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCategoryList= _unitOfWork.coverType.GetAll();
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
        public IActionResult Create(CoverType obj)
        {   
            if (ModelState.IsValid)
            {
                _unitOfWork.coverType.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "cover type created successfully";
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
            var CoverType = _unitOfWork.coverType.GetFirstOrDefault(c => c.Id == id);
            if(CoverType == null)
            { return NotFound(); }
            
            return View(CoverType);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType obj)
        {
            
            
            if (ModelState.IsValid)
            {
                _unitOfWork.coverType.update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type update successfully";

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
            var CoverTypeFromDb = _unitOfWork.coverType.GetFirstOrDefault(c => c.Id == id);
            if (CoverTypeFromDb == null)
            { return NotFound(); }

            return View(CoverTypeFromDb);
        }
        //post
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int ?id)
        {
            var obj = _unitOfWork.coverType.GetFirstOrDefault(c => c.Id == id);
            if (obj == null)
            { return NotFound();
            }


            _unitOfWork.coverType.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Cover Type Delete successfully";

            return RedirectToAction("Index");
            
            
        }
    }

