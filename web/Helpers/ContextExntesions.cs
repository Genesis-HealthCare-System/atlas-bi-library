using System.Security.Claims;
using Atlas_Web.Models;

namespace Atlas_Web.Helpers;

public static class ContextExtensions
{
    public static bool IsHyperspace(this HttpContext context)
    {
        if (context.Request.Cookies["EPIC"] == "1" || context.Request.Query["EPIC"] == "1")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
