using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.iRepository
{
    public interface IOrderHeaderRepository : iRepository<OrderHeader>
    {
        void update(OrderHeader obj);
        void UpdateStatus(int id , string orderStatus,string? paymentStatus=null);
    }
}
