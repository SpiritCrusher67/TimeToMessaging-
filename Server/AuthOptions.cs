using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class AuthOptions
    {
        public const string ISSUER = "TTMServer"; // издатель токена
        public const string AUDIENCE = "TTMClient"; // потребитель токена
        const string KEY = "gkygtufvotv765jh77h699jh6";   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
