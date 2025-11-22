using AutoMapper;
using DataAccess.Context;
using Domain.Interfaces;
using Domain.Interfaces.GenericInterfaces;
using Infastructure.Repositories;
using Infastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Add services to the container.
//builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSwaggerGen();

// Dependancy injection
builder.Services.AddAutoMapper(typeof(Mapper));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

builder.Services.AddScoped<IDesignTimeDbContextFactory<StoreDbContext>, StoreDbContextFactory>();
builder.Services.AddTransient<DbContext, StoreDbContext>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


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

