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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            //_db.Products.Update(obj);
            var objFromDB = _db.Products.FirstOrDefault(u=>u.ID==obj.ID);
            if (objFromDB != null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.Description = obj.Description;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.Author = obj.Author;
                objFromDB.ListPrice = obj.ListPrice;
                objFromDB.Price1 = obj.Price1;
                objFromDB.Price50 = obj.Price50;
                objFromDB.Price100 = obj.Price100;
                objFromDB.CategoryID = obj.CategoryID;
                //if(obj.ImageUrl != null)
                //{
                //    objFromDB.ImageUrl = obj.ImageUrl;
                //}
            }
        }
    }
}
