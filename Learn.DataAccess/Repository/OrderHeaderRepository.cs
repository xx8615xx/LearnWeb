using Learn.DataAccess.Data;
using Learn.DataAccess.Repository.IRepository;
using Learn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext _db;
        public OrderHeaderRepository(AppDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

		public void UpdateStatus(int ID, string orderStatus, string? paymentStatus = null)
		{
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u=>u.ID == ID);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
		}

		public void UpdateStripePaymentID(int ID, string sessionID, string paymentIntentID)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.ID == ID);
			if (orderFromDb != null)
			{
                if (!string.IsNullOrEmpty(sessionID))
                {
                    orderFromDb.SessionID = sessionID;
                }
                if (!string.IsNullOrEmpty(paymentIntentID))
                {
                    orderFromDb.PaymentIntentID = paymentIntentID;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
			}
		}
	}
}
