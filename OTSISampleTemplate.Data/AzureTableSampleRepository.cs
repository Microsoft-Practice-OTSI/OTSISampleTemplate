using Azure;
using Azure.Data.Tables;
using OTSISampleTemplate.Data.Abstraction;
using OTSISampleTemplate.Data.Helpers;
using OTSISampleTemplate.Data.Models;
using OTSISampleTemplate.Data.TableEntity;

namespace OTSISampleTemplate.Data;

public class AzureTableSampleRepository(TableClient tableClient) : ISampleRepository
{
    public readonly TableClient _tableClient = tableClient;

    public async Task AddSampleAsync(SampleEntity entity)
    {
        var tableEntity = new SampleTableEntity
        {
            SampleId = entity.SampleId ?? Guid.NewGuid().ToString(),
            Name = entity.Name,
            Category = entity.Category,
            Location = entity.Location,
            AssignedTo = entity.AssignedTo,
            Description = entity.Description,
            Status = entity.Status,
            Priority = entity.Priority,
            TotalCount = entity.TotalCount,
            ProcessedCount = entity.ProcessedCount,
            ReportDate = entity.ReportDate,
            PartitionKey = entity.Location ?? "DefaultLocation",
            RowKey = Guid.NewGuid().ToString()
        };

        await _tableClient.AddEntityAsync(tableEntity);
    }

    public async Task DeleteSampleAsync(string location, string sampleId)
    {
        var filter = TableClient.CreateQueryFilter<SampleTableEntity>(e => e.PartitionKey == location && e.SampleId == sampleId);
        var results = await TableStorageHelper.QueryEntitiesAsync<SampleTableEntity>(_tableClient, filter);
        foreach (var result in results)
        {
            await _tableClient.DeleteEntityAsync(result.PartitionKey, result.RowKey);
        }
    }

    public async Task UpdateSampleAsync(SampleTableEntity entity)
    {
        await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
    }

    public async Task<SampleTableEntity?> GetSampleByIdAsync(string location, string sampleId)
    {
        try
        {
            var filter = TableClient.CreateQueryFilter<SampleTableEntity>(e => e.PartitionKey == location && e.SampleId == sampleId);
            var results = await TableStorageHelper.QueryEntitiesAsync<SampleTableEntity>(_tableClient, filter);
            return results?.FirstOrDefault();
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<IEnumerable<SampleEntity>> GetAllSamplesAsync()
    {
        var result = await TableStorageHelper.QueryEntitiesAsync<SampleTableEntity>(_tableClient);
        return result?.Cast<SampleEntity>() ?? [];
    }

    public async Task<IEnumerable<SampleTableEntity>> GetSamplesByLocationAsync(string location)
    {
        var filter = TableClient.CreateQueryFilter<SampleTableEntity>(e => e.Location == location);
        var result = await TableStorageHelper.QueryEntitiesAsync<SampleTableEntity>(_tableClient, filter);
        return result;
    }

    public async Task<IEnumerable<SampleEntity>> GetSamplesByDateRangeAsync(string fromDate, string toDate)
    {
        var result = new List<SampleEntity>();
        var filter = TableClient.CreateQueryFilter($"ReportDate ge '{fromDate}' and ReportDate le '{toDate}'");
        await foreach (var entity in _tableClient.QueryAsync<SampleTableEntity>(filter: filter))
        {
            result.Add(entity);
        }

        return result;
    }
}
