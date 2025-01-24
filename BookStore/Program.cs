using BusinessLayer.Interface;
using BusinessLayer.Services;
using DataLayer.Context;
using DataLayer.Interface;
using DataLayer.Repository;
using DataLayer.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Preserve object references to avoid circular reference issues
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Optional: Ignore null properties
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => {
        builder
            .AllowAnyOrigin()     // Allow all origins
            .AllowAnyMethod()     // Allow all HTTP methods
            .AllowAnyHeader();    // Allow all headers
    });

    options.AddPolicy("SpecificOrigins",
        builder =>
        {
            builder.WithOrigins(
                "http://localhost:4200"
                )
            .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });

});

builder.Services.AddDbContext<BookStoreContext>(cfg =>
    cfg.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
// Add services to the container.

builder.Services.AddTransient<IUserDL, UserDL>();
builder.Services.AddTransient<IUserBL, UserBL>();
builder.Services.AddTransient<IBookDL, BookDL>();
builder.Services.AddTransient<IBookBL, BookBL>();
builder.Services.AddTransient<IAddressDL, AddressDL>();
builder.Services.AddTransient<IAddressBL, AddressBL>();
builder.Services.AddTransient<ICartBL,CartBL>();
builder.Services.AddTransient<ICartDL, CartDL>();
builder.Services.AddTransient<IOrderBL, OrderBL>();
builder.Services.AddTransient<IOrderDL, OrderDL>(); 
builder.Services.AddTransient<IWishlistBL, WishlistBL>();
builder.Services.AddTransient<IWishlistDL, WishlistDL>();   
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<TokenUtility>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configure Swagger for API documentation with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {your-jwt-token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
