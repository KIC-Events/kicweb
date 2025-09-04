using KiCData.Services;

namespace KiCWeb.Helpers;
using Hangfire.Dashboard;

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}