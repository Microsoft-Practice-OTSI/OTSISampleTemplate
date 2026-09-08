using OTSISampleTemplate.Web.Compositions;

var builder = WebApplication.CreateBuilder(args);

// Add custom configurations and services
builder.AddConfiguration();
builder.AddSecurity();
builder.AddApplicationServices();
builder.AddDataServices();

// Add MVC services
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
