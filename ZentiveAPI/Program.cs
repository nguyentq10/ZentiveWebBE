using DAL.DBcontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Repo;
using Repository.Repo.Repository.Repo;
using Services.Configuration;
using Services.Interface;
using Services.Services;
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// --- CẤU HÌNH CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<ZenthicDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorization();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

// -- Service và Interface --
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IProjectServices, ProjectServices>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<IRewardTierServices, RewardTierServices>();
builder.Services.AddScoped<IPaymentServices, PaymentServices>();
builder.Services.AddScoped<IPledgeServices, PledgeService>();
builder.Services.AddScoped<ISiteDonationServices,SiteDonationSerivces>();
builder.Services.AddScoped<IMediaAssetServices, MediaAssetServices>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/home/DataProtection-Keys"));
// --- PHẦN 1: CẤU HÌNH DỊCH VỤ JWT ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            NameClaimType = JwtRegisteredClaimNames.Sub
        };
    });

// --- PHẦN 2: CẤU HÌNH SWAGGER ĐỂ HIỂN THỊ NÚT "AUTHORIZE" ---
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer' [dấu cách] rồi dán token vào đây."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});
// -- Stripe --- 

var stripeSettings = builder.Configuration.GetSection("StripeSettings").Get<StripeSettings>();
StripeConfiguration.ApiKey = stripeSettings.SecretKey;

var app = builder.Build();
app.Urls.Add($"http://0.0.0.0:{port}");

// --- CẤU HÌNH HTTP REQUEST PIPELINE ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zentive API V1");
    c.RoutePrefix = "swagger"; 
});


//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
    
// THÊM DÒNG NÀY VÀO
app.MapGet("/health", () => Results.Ok("Healthy"));

// --- PHẦN 3: KÍCH HOẠT MIDDLEWARE CỦA JWT ---


app.Run();