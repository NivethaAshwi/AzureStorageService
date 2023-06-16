using Azure.Core.Extensions;
using Azure.Storage.Blobs;
using AzureStorage.Service.Interface;
using AzureStorage.Service.Service;
using Serilog.AspNetCore;
using Serilog.Expressions;
using Serilog;
using System.Reflection.Metadata;
using Azure.Data.Tables;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region CoRS Origin for Sharing the resource in cross platform like mobile,ioT so the we need to config here.

builder.Services.AddCors(Options =>
{
    Options.AddPolicy("CustomPolicy", x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
#endregion
var connectionstring = builder.Configuration.GetConnectionString("StorageConnectionString");
builder.Services.AddTransient<IAzureBlobStorage, AzureBlobFileService>();
builder.Services.AddScoped<IAzureTableStorage, TableStorageService>();
builder.Services.AddScoped(_ =>
{
    return new BlobServiceClient(builder.Configuration.GetConnectionString("StorageConnectionString"));
});
#region serilog config for log files
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.File("ApiLogs/log.txt", rollingInterval: RollingInterval.Day);
    if (context.HostingEnvironment.IsProduction() == false)
    {
        config.WriteTo.Console();
    }
});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CustomPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
