using api_flms_service.Model;
using api_flms_service.Service;
using api_flms_service.ServiceInterface;
using api_flms_service.Services;
using Microsoft.EntityFrameworkCore;
using api_auth_service.Services;
using api_flms_service.Entity;
using api_flms_service.ServiceInterface.api_flms_service.Services;

WebApplicationBuilder BuildApp()
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", builder =>
        {
            builder.WithOrigins("http://localhost:5000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
    });

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddHttpClient<AuthService>();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.Configure<LoanSettings>(builder.Configuration.GetSection("LoanSettings"));

    builder.Services.AddScoped<VnPayService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IBookService, BookService>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IAuthorService, AuthorService>();
    builder.Services.AddScoped<IIssuedBookService, IssuedBookService>();
    builder.Services.AddScoped<ILoanService, LoanService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
    builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

    builder.Services.AddRazorPages().AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

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

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("AllowFrontend");
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
        endpoints.MapControllers();
    });

    return app;
}

var devApp = RunApp();
devApp.Run();