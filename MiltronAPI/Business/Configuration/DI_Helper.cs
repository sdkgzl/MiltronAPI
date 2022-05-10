using Business.Automapper.Profiles;
using Business.BusinessRules.Abstract;
using Business.BusinessRules.GenericBusinessRules.Abstract;
using Business.BusinessRules.GenericBusinessRules.Interface;
using Business.BusinessRules.Interfaces;
using Business.Helper.Abstract;
using Business.Helper.Asbtract;
using Business.Helper.Interfaces;
using DataAccess.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Dto;

namespace Business
{
    public static class DI_Helper
    {
        public static IServiceCollection BusinessLayerDI(this IServiceCollection services)
        {
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IAuthBusinessRules, AuthBusinessRules>();
            services.AddScoped<IUserOperationHelper, UserOperationHelper>();
            services.AddScoped<IUnitOfWork, UnitOfWorkHelper>();
            services.AddScoped<IUnitOfWork, UnitOfWorkHelper>();
            services.AddScoped<IUserBusinessRules, UserBusinessRules>();            
            return services;
        }
    }
}
