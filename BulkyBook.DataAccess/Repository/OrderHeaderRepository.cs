using BulkyBook.DataAccess.Repository.iRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
       

        public void update(OrderHeader obj)
        {
            _db.orderHeaders.Update(obj);        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {var orderFromDb= _db.orderHeaders.FirstOrDefault(u=>u.Id==id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                { orderFromDb.PaymentStatus = paymentStatus; }
            }
        }

        public void UpdateStripPaymentID(int id, string sessionId, string paymentItenId)
        {
            var orderFromDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            orderFromDb.PaymentDate=DateTime.Now;
           orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId= paymentItenId;
        }

        
    }
}
