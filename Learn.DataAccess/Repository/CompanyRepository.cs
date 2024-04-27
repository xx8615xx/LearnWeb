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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _db;
        public CompanyRepository(AppDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Company obj)
        {
            _db.Companies.Update(obj);
        }
    }
}
