using api_flms_service.Model;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using api_flms_service.ServiceInterface.api_flms_service.Services;
using api_flms_service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using api_auth_service.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the 
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient<AuthService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IReserveBookService, ReserveBookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IIssuedBookService, IssuedBookService>();

// Add controllers and Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Configure Swagger
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/admins", async (AppDbContext db) =>
    {
        return await db.Admins.ToListAsync();
    });
}

// Add static file serving (for Razor Pages)
app.UseStaticFiles();

app.UseAuthorization();

// Map Controllers and Razor Pages
app.MapControllers();
//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//app.Urls.Add($"http://0.0.0.0:{port}");
app.MapRazorPages();

app.Run();
