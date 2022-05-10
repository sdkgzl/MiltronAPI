using Business.Helper.Abstract;
using DataAccess.Entities;

namespace Business.Helper.Interfaces
{
    public interface IUserOperationHelper
    {
        public string CreateToken(User model);
        public PasswordHelper CreatePasswordHash(string password);

        public bool VerifyPasswordHash(string password, PasswordHelper passwordHelper);       
    }
}
