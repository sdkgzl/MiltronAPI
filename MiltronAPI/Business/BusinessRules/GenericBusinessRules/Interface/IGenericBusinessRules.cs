using AutoMapper;
using Shared.Dto;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessRules.GenericBusinessRules.Interface
{
    public interface IGenericBusinessRules<TDto, TAutomapperProfile> where TDto : BaseDto, new()
                                                                                where TAutomapperProfile : Profile, new()
    {
        Task<Response<TDto>> Insert(TDto insertedDto, List<string> uniqueColumns = null, List<object> uniqueValues = null);
    }
}
