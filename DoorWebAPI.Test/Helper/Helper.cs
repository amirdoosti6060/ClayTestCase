using DoorWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorWebAPI.Test.Helper
{
    public static class Helper
    {
        public static GeneralResponse? GetGeneralResponse(this IActionResult actionResult)
        {
            ObjectResult? restResult = actionResult as ObjectResult;
            return restResult!.Value as GeneralResponse;
        }
    }
}
