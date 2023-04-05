using IotSupplyStore.DataAccess;
using IotSupplyStore.Models;
using IotSupplyStore.Service.IService;
using IotSupplyStore.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using IotSupplyStore.Repository.IRepository;
using IotSupplyStore.Repository;
using IotSupplyStore.Utility;
using IotSupplyStore.Settings;

var builder = WebApplication.CreateBuilder(args);

// Controller Service Configuration
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddResponseCaching();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

// "DevelopEnvironment" is using for database of developer, change it to "DefaultConnection" if want to use database server
#region Database configuration

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
//    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DevelopEnvironment")));

#endregion

// Add DI Service
#region DI Service

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IMailService, MailService>();

#endregion


//services cors
#region Service CORS

builder.Services.AddCors(p => p.AddPolicy("AllowAllHeadersPolicy", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

#endregion


// add authentication services to the application's service collection
#region Identity Services set up
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // ApplicationUsers settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

var secretKey = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(u =>
{
    u.RequireHttpsMetadata = false;
    u.SaveToken = true;
    u.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

#endregion

// add policy authentication
#region Policy Set up
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(SD.Policy_SuperAdmin, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        //policyBuilder.RequireRole(SD.Role_Admin);
        policyBuilder.RequireClaim("FullName", "Dao Trong Nhan");
    });

    options.AddPolicy(SD.Policy_AccountManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Admin);
    });

    options.AddPolicy(SD.Policy_CategoryManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Employee, SD.Role_Admin);
    });

    options.AddPolicy(SD.Policy_ProductManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Employee, SD.Role_Admin);
    });

    options.AddPolicy(SD.Policy_SupplierManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Employee, SD.Role_Admin);
    });

    options.AddPolicy(SD.Policy_OrderProcess, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Shipper, SD.Role_Admin, SD.Role_Customer);
    });

    options.AddPolicy(SD.Policy_EditProfile, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
    });

    options.AddPolicy(SD.Policy_MakeOrder, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Customer, SD.Role_Admin);
    });
});

#endregion

// add authorization for swagger
#region Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
           new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

#endregion

var app = builder.Build();

app.UseCors("AllowAllHeadersPolicy");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
