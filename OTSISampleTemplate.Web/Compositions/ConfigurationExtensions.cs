namespace OTSISampleTemplate.Web.Compositions;

public static partial class WebApplicationBuilderExtensions
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        }
    }
}
