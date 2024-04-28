using Learn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        public void Update(OrderHeader obj);
        public void UpdateStatus(int ID, string orderStatus, string? paymentStatus = null);
        public void UpdateStripePaymentID(int ID, string sessionID, string paymentIntentID);
    }
}
