﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helper.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommmitAsync();

        void Commit();
    }
}
