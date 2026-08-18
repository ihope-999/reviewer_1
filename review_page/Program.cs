using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using review_page.Domain.Controller;
using review_page.Domain.LocationDomain.Core.Data;
using review_page.Domain.LocationDomain.Core.Interfaces;
using review_page.Domain.LocationDomain.Database;
using review_page.Domain.LocationDomain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var reviewApiBaseUrl = builder.Configuration["ReviewApi:BaseUrl"] ?? "https://localhost:7099";
builder.Services.AddHttpClient("ReviewApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:5082");
});
builder.Services.AddScoped<ReviewLinkController, ReviewLinkController>();
builder.Services.AddScoped<IGooglePlacesService, GooglePlacesService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddControllers();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


var password = builder.Configuration["your-password"]; //usersecret
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString = connectionString.Replace("{YOURPASSWORD}", password);


builder.Services.AddDbContext<ReviewDBContext>(options =>
    options.UseNpgsql(connectionString));



var app = builder.Build();
QuestPDF.Settings.License = LicenseType.Community;
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
