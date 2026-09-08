using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTSISampleTemplate.Web.Models;
using System.Security.Claims;

namespace OTSISampleTemplate.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private static readonly List<SampleItemModel> SampleData =
    [
        new() { Id = "REC-1001", Name = "Batch Processing - North Hub", Category = "Production", Location = "New York, USA", AssignedTo = "Alice Morgan", TotalUnits = 1250, ProcessedUnits = 1250, Status = "Completed", Priority = "High", LastUpdated = DateTime.Now.AddHours(-2) },
        new() { Id = "REC-1002", Name = "Inventory Dispatch - Facility B", Category = "Logistics", Location = "Chicago, USA", AssignedTo = "Brian Vance", TotalUnits = 850, ProcessedUnits = 620, Status = "Active", Priority = "Medium", LastUpdated = DateTime.Now.AddMinutes(-45) },
        new() { Id = "REC-1003", Name = "Quality Audit & Calibration", Category = "Quality Control", Location = "Dallas, USA", AssignedTo = "Clara Oswald", TotalUnits = 400, ProcessedUnits = 390, Status = "Active", Priority = "High", LastUpdated = DateTime.Now.AddMinutes(-15) },
        new() { Id = "REC-1004", Name = "Security Allocation Sync", Category = "Compliance", Location = "Austin, USA", AssignedTo = "David Miller", TotalUnits = 300, ProcessedUnits = 50, Status = "Pending", Priority = "Low", LastUpdated = DateTime.Now.AddDays(-1) },
        new() { Id = "REC-1005", Name = "Firmware Rollout - Stage 1", Category = "IT & Systems", Location = "Seattle, USA", AssignedTo = "Emma Watson", TotalUnits = 2100, ProcessedUnits = 2100, Status = "Completed", Priority = "High", LastUpdated = DateTime.Now.AddHours(-6) },
        new() { Id = "REC-1006", Name = "Barcode Serialization Job", Category = "Production", Location = "New York, USA", AssignedTo = "Frank Wright", TotalUnits = 950, ProcessedUnits = 210, Status = "Critical", Priority = "High", LastUpdated = DateTime.Now.AddMinutes(-5) },
        new() { Id = "REC-1007", Name = "Material Transfer Manifest", Category = "Logistics", Location = "Atlanta, USA", AssignedTo = "Grace Hopper", TotalUnits = 1500, ProcessedUnits = 1450, Status = "Active", Priority = "Medium", LastUpdated = DateTime.Now.AddHours(-1) },
        new() { Id = "REC-1008", Name = "Safety Sensor Inspection", Category = "Maintenance", Location = "Denver, USA", AssignedTo = "Harry Osborn", TotalUnits = 220, ProcessedUnits = 0, Status = "Pending", Priority = "Low", LastUpdated = DateTime.Now.AddDays(-2) },
        new() { Id = "REC-1009", Name = "Regional Data Reconciliation", Category = "Analytics", Location = "San Francisco, USA", AssignedTo = "Iris West", TotalUnits = 5000, ProcessedUnits = 5000, Status = "Completed", Priority = "Medium", LastUpdated = DateTime.Now.AddHours(-12) },
        new() { Id = "REC-1010", Name = "Emergency Backup Generator Test", Category = "Facilities", Location = "Boston, USA", AssignedTo = "Jack Ryan", TotalUnits = 80, ProcessedUnits = 80, Status = "Completed", Priority = "High", LastUpdated = DateTime.Now.AddHours(-4) }
    ];

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string? search, string? status, string? location)
    {
        var query = SampleData.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                x.Id.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                x.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                x.Category.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                x.AssignedTo.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(status) && status != "All")
        {
            query = query.Where(x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(location) && location != "All")
        {
            query = query.Where(x => x.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
        }

        var items = query.ToList();

        var model = new SampleGridViewModel
        {
            Items = items,
            SearchQuery = search ?? string.Empty,
            SelectedStatus = status ?? "All",
            SelectedLocation = location ?? "All",
            TotalRecords = SampleData.Count,
            ActiveRecords = SampleData.Count(x => x.Status == "Active"),
            CompletedRecords = SampleData.Count(x => x.Status == "Completed"),
            PendingRecords = SampleData.Count(x => x.Status == "Pending" || x.Status == "Critical"),
            CurrentUser = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "User",
            UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Standard User"
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
