using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Interfaces
{
    public interface IHome
    {
       Task<string> Login(string userNm, string password);
        Task<string> LoginToken(string userNm, string password);
    }
}
