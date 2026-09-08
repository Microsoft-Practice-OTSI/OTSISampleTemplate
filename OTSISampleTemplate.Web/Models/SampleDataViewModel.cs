namespace OTSISampleTemplate.Web.Models;

public class SampleItemModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public int TotalUnits { get; set; }
    public int ProcessedUnits { get; set; }
    public string Status { get; set; } = string.Empty; // "Active", "Completed", "Pending", "Critical"
    public string Priority { get; set; } = string.Empty; // "High", "Medium", "Low"
    public DateTime LastUpdated { get; set; }
}

public class SampleGridViewModel
{
    public List<SampleItemModel> Items { get; set; } = [];
    public string SearchQuery { get; set; } = string.Empty;
    public string SelectedStatus { get; set; } = "All";
    public string SelectedLocation { get; set; } = "All";
    public int TotalRecords { get; set; }
    public int ActiveRecords { get; set; }
    public int CompletedRecords { get; set; }
    public int PendingRecords { get; set; }
    public string CurrentUser { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
}
