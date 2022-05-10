using Shared.Dto;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessRules.Interfaces
{
    public interface IUserBusinessRules
    {
        Task<Response<List<UserDto>>> GetAll();
        Task<Response<UserDto>> Update(UserDto model);
        Task<Response<UserDto>> ChangePassword(UpdatePasswordDto model);
    }
}
