using ApiDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Services
{
   public interface ITokenService
    {
        string GenerateToken(Guid userId, string email, string role);
        RefreshToken GenerateRefreshToken(Guid authUserId);
    }
}
