using Microsoft.AspNetCore.Mvc;
using HistoryWebAPI.Interfaces;

namespace HistoryWebAPI.Test.Helper
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
