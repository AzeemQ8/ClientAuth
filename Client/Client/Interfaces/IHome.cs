using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Interfaces
{
    public interface IHome
    {
       Task<bool> LoginClient(string token);
    }
}
