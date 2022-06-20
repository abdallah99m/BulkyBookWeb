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
            product = new ProductRepository(_db);
        }
        public ICategoryRepository category { get;private set; }
        public ICoverTypeRepository coverType { get; private set; }
        public IProductRepository product { get; private  set; }


        public void Save()
        {
            _db.SaveChanges();       
        }
    }
}
