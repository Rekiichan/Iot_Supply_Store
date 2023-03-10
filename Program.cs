using IotSupplyStore.DataAccess;
using Microsoft.EntityFrameworkCore;
using IotSupplyStore.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// "DevelopEnvironment" is using for database of developer, change it to "DefaultConnection" if want to use database server
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DevelopEnvironment")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Authenication

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("UserRole", "Admin"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireClaim("UserRole", "Customer"));
    options.AddPolicy("RequireEmployeeRole", policy => policy.RequireClaim("UserRole", "Employee"));
});

//services cors
builder.Services.AddCors(p => p.AddPolicy("AllowAllHeadersPolicy", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("AllowAllHeadersPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
