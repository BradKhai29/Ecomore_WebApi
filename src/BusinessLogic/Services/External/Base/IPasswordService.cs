using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.External.Base
{
    public interface IPasswordService
    {
        string GetHashPassword(string password);
    }
}
