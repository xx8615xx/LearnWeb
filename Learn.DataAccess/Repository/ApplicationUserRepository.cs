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
    public class ApplicationUserRepository: Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly AppDbContext _db;
        public ApplicationUserRepository(AppDbContext db):base(db) {
            _db = db;
        }
    }
}
