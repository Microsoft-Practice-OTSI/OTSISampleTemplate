using OTSISampleTemplate.Data.Models;
using OTSISampleTemplate.Data.TableEntity;

namespace OTSISampleTemplate.Data.Abstraction;

public interface ISampleRepository
{
    Task AddSampleAsync(SampleEntity entity);
    Task UpdateSampleAsync(SampleTableEntity entity);
    Task DeleteSampleAsync(string location, string sampleId);
    Task<SampleTableEntity?> GetSampleByIdAsync(string location, string sampleId);
    Task<IEnumerable<SampleEntity>> GetAllSamplesAsync();
    Task<IEnumerable<SampleTableEntity>> GetSamplesByLocationAsync(string location);
    Task<IEnumerable<SampleEntity>> GetSamplesByDateRangeAsync(string fromDate, string toDate);
}
