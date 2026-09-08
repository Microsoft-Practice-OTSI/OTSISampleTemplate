using OTSISampleTemplate.Web.Filters;

namespace OTSISampleTemplate.Web.Compositions;

public static class ExceptionFilterExtensions
{
    public static void AddGlobalExceptionFilter(this IServiceCollection services)
    {
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });
    }
}
