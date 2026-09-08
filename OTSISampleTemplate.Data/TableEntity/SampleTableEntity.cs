using Azure;
using Azure.Data.Tables;
using OTSISampleTemplate.Data.Models;

namespace OTSISampleTemplate.Data.TableEntity;

public class SampleTableEntity : SampleEntity, ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
