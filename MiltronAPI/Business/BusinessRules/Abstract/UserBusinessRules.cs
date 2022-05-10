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
    public class UserBusinessRules : IUserBusinessRules
    {
        private MiltronDbContext _context;        
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;        
        private readonly IUserOperationHelper _userOperationHelper;
        public UserBusinessRules(
            MiltronDbContext context,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserOperationHelper userOperationHelper)
        {
            _context = context;            
            _mapper = mapper;
            _unitOfWork = unitOfWork;            
            _userOperationHelper = userOperationHelper;
        }
        public async Task<Response<List<UserDto>>> GetAll()
        {
            List<User> users;
            users =  await _context.Users.Where(x => x.IsDeleted == false && x.IsActive == true).ToListAsync();
            var convertedUserList = _mapper.Map<List<UserDto>>(users);
            return Response<List<UserDto>>.Success(convertedUserList, 200);
        }

        public async Task<Response<UserDto>> Update(UserDto model)
        {
            User userGetById = new();
            User convertedUser = new();
            userGetById = _context.Users.Where(x => x.Id == model.Id).FirstOrDefault();
            
            convertedUser = _mapper.Map<UserDto, User>(model);
            convertedUser.PasswordHash = userGetById.PasswordHash;
            convertedUser.PasswordSalt = userGetById.PasswordSalt;
            
            User updatedUser = new();
                        
            _context.Users.Update(convertedUser);            
            
            if (updatedUser == null)
            {
                return Response<UserDto>.Fail("İşlem Sırasında hata oluştu", 404, true);
            }

            await _unitOfWork.CommmitAsync();

            var convertedUpdatedUser = _mapper.Map<UserDto>(convertedUser);

            return Response<UserDto>.Success(convertedUpdatedUser, 200);
        }

        public async Task<Response<UserDto>> ChangePassword(UpdatePasswordDto model)
        {
            User entity = new User();

            entity = _context.Users.Find(model.UserId);

            if(entity == null)
            {
                return Response<UserDto>.Fail("İşlem Sırasında hata oluştu", 404, true);
            }

            var passwordHelper = _userOperationHelper.CreatePasswordHash(model.Password);

            entity.PasswordHash = Convert.ToBase64String(passwordHelper.PasswordHash);
            entity.PasswordSalt = Convert.ToBase64String(passwordHelper.PasswordSalt);

            var entityUser = _context.Users.Update(entity);

            await _unitOfWork.CommmitAsync();

            var convertedUser = _mapper.Map<UserDto>(entityUser.Entity);
            return Response<UserDto>.Success(convertedUser, 200);
        }
    }
}
