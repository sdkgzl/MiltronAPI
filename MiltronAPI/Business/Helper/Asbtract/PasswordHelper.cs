using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helper.Abstract
{
    public class PasswordHelper
    {
        public byte[] PasswordHash  { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
