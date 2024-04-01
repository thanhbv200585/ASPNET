using ASPNET.Data;
using ASPNET.Filter;
using ASPNET.Repositories;
using ASPNET.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<BookStoreContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<BookStoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStore"));
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IBookRepository, BookRepository>();

// In-memory cache
builder.Services.AddMemoryCache();

// Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379, password=eYVX7EwVmmxKPCDmwMtyKVge8oLd2t81";
});


// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new MediaTypeApiVersionReader();
    options.ApiVersionReader = ApiVersionReader.Combine
           (new MediaTypeApiVersionReader(),
            new HeaderApiVersionReader("X-Version"),
            new QueryStringApiVersionReader());
});

builder.Services.AddMvc(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// API Filtering
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<MyLoggingAttribute>();
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
