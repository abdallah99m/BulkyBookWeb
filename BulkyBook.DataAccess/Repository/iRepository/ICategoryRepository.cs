using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.iRepository
{
    public interface ICategoryRepository:iRepository<Category>
    {
        void update(Category obj);
        void Save();
    }
}
