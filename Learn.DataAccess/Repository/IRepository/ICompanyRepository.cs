﻿using Learn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository: IRepository<Company>
    {
        public void Update(Company obj);
    }
}
