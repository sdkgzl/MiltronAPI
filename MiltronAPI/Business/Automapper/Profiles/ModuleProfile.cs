using AutoMapper;
using DataAccess.Entities;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Automapper.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile() { }

        public ModuleProfile(Type dataContextType)
        {

        }
    }
}
