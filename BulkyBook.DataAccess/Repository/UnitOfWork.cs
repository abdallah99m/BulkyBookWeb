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
            company = new CompanyRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            applicationUser = new ApplicationUserRepository(_db);
            orderDeatil=new OrderDetailsRepository(_db);
            orderHeader = new OrderHeaderRepository(_db);
        }
        public ICategoryRepository category { get;private set; }
        public ICoverTypeRepository coverType { get; private set; }
        public IProductRepository product { get; private  set; }
        public ICompanyRepository company { get; private set; }
        public IShoppingCartRepository shoppingCart { get; private set; }

        public IApplicationUserRepository applicationUser { get; private set; }
        

        public IOrderHeaderRepository orderHeader { get; private set; }
        public IOrderDeatilRepository orderDeatil { get; private set; }

        public void Save()
        {
            _db.SaveChanges();       
        }
    }
}
