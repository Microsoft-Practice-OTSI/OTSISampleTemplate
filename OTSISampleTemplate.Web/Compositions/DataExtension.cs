using Azure.Data.Tables;
using OTSISampleTemplate.Data;
using OTSISampleTemplate.Data.Abstraction;

namespace OTSISampleTemplate.Web.Compositions;

public static partial class WebApplicationBuilderExtensions
{
    public static void AddDataServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["AzureStorageAccountConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.Services.AddScoped<ISampleRepository>(s =>
            {
                var sampleTableClient = new TableClient(connectionString, "SampleRecords");
                sampleTableClient.CreateIfNotExists();
                return new AzureTableSampleRepository(sampleTableClient);
            });
        }
    }
}
