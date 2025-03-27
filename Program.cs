using api_flms_service.Model;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using api_flms_service.ServiceInterface.api_flms_service.Services;
using api_flms_service.Services;
using Microsoft.EntityFrameworkCore;
using api_auth_service.Services;
using api_flms_service.Entity;

WebApplicationBuilder BuildApp()
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the 
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddHttpClient<AuthService>();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.Configure<VNPaySettings>(builder.Configuration.GetSection("VNPay"));
    
    builder.Services.AddScoped<VNPayService>();
    builder.Services.AddScoped<IReserveBookService, ReserveBookService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IBookService, BookService>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IAuthorService, AuthorService>();
    builder.Services.AddScoped<IIssuedBookService, IssuedBookService>();
    builder.Services.AddScoped<ILoanService, LoanService>();
    builder.Services.AddControllers();
    builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
    builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
    // Add controllers and Razor Pages
    builder.Services.AddControllers();
    builder.Services.AddRazorPages().AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true; // Bật client-side validation
    });



    // Configure Swagger
    builder.Services.AddHttpClient();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
   

    builder.Services.AddLogging(logging =>
    {
        logging.AddConsole();
    });


    return builder;
}



WebApplication RunApp()
{
    var app = BuildApp().Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Add static file serving (for Razor Pages)
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    // Map Controllers and Razor Pages
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
        endpoints.MapControllers();
    });
    return app;
}

/*try
{
    var productionApp = RunApp();
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    productionApp.Urls.Add($"http://0.0.0.0:{port}");
    productionApp.Run();
}
catch (IOException)
{
    var devApp = RunApp();
    devApp.Run();
}*/

var devApp = RunApp();
devApp.Run();