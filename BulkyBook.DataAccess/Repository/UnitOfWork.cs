using BulkyBook.DataAccess.Repository.iRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        { 
            _db = db;
            category = new CategoryRepository(_db);
            coverType = new CoverTypeRepository(_db);
        }
        public ICategoryRepository category { get; set; }
        public ICoverTypeRepository coverType { get; set; }

        public void Save()
        {
            _db.SaveChanges();       
        }
    }
}
