using AutoMapper;
using Business.BusinessRules.Interfaces;
using Business.Helper.Interfaces;
using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Dto;
using Shared.Helpers;

namespace Business.BusinessRules.Abstract
{

    public class AuthBusinessRules : IAuthBusinessRules
    {
        private MiltronDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserOperationHelper _userOperationHelper;
        public AuthBusinessRules(MiltronDbContext context,IJwtUtils jwtUtils,IMapper mapper, IUnitOfWork unitOfWork, IUserOperationHelper userOperationHelper)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userOperationHelper = userOperationHelper;
        }

        public async Task<Response<AuthenticateResponseDto>> Authenticate(AuthenticateRequestDto model)
        {
            var passwordHelper = _userOperationHelper.CreatePasswordHash(model.Password);            

            if (!_userOperationHelper.VerifyPasswordHash(model.Password, passwordHelper))
            {
                return Response<AuthenticateResponseDto>.Fail("Kullanıcı adı veya şifre yanlıştır", 404, true);
            }

            var user = _context.Users.SingleOrDefault(x => x.UserName == model.UserName);

            if (user == null)
            {
                return Response<AuthenticateResponseDto>.Fail("Kullanıcı Bulunamamıştır", 404, true);
            }            


            var response = _mapper.Map<AuthenticateResponseDto>(user);
            response.Token = _jwtUtils.GenerateToken(user);
            return Response<AuthenticateResponseDto>.Success(response, 200);
        }

        public async Task<Response<UserDto>> GetById(int id)
        {
            User user = new User();

            user =  await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Response<UserDto>.Fail("Kullanıcı Bulunamamıştır", 404, true);
            }
            var convertedUser = _mapper.Map<UserDto>(user);
            return Response<UserDto>.Success(convertedUser, 200);
        }

        public async Task<Response<RegisterRequestDto>> Register(RegisterRequestDto model)
        {
            if (_context.Users.Any(x => x.UserName == model.UserName))
            {
                return Response<RegisterRequestDto>.Fail("UserName already created", 404, true);
            }
                            
            var user = _mapper.Map<User>(model);                    
            var passwordHelper = _userOperationHelper.CreatePasswordHash(model.Password);
            user.PasswordHash = Convert.ToBase64String(passwordHelper.PasswordHash);
            user.PasswordSalt = Convert.ToBase64String(passwordHelper.PasswordSalt);
            //user.AddedByUserId = 5;
            var entityUser = _context.Users.Add(user);        
            await _unitOfWork.CommmitAsync();
            var convertedUser = _mapper.Map<RegisterRequestDto>(entityUser.Entity);

            return Response<RegisterRequestDto>.Success(convertedUser, 200);
        }

        
    }
}
