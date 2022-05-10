using DataAccess.Entities;
using Shared.Dto;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessRules.Interfaces
{
    public interface IAuthBusinessRules
    {
        Task<Response<AuthenticateResponseDto>> Authenticate(AuthenticateRequestDto model);
        Task<Response<UserDto>> GetById(int id);
        Task<Response<RegisterRequestDto>> Register(RegisterRequestDto model);        
    }
}
