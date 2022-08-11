using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(o => // currently all set to max, configure it to your needs!
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = long.MaxValue; // <-- !!! long.MaxValue
    o.MultipartBoundaryLengthLimit = int.MaxValue;
    o.MultipartHeadersCountLimit = int.MaxValue;
    o.MultipartHeadersLengthLimit = int.MaxValue;
    o.KeyLengthLimit = int.MaxValue;
    o.ValueCountLimit = int.MaxValue;
});
builder.Services.Configure<KestrelServerOptions>(opts => opts.AllowSynchronousIO = true);
builder.Services.Configure<IISServerOptions>(opts => opts.AllowSynchronousIO = true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCaching();

builder.Services.AddSingleton(x =>
{
    return new MinioClient()
    .WithEndpoint(
        Environment.GetEnvironmentVariable("minio_endpoint")
    )
    .WithCredentials(
        Environment.GetEnvironmentVariable("minio_accesskey"),
        Environment.GetEnvironmentVariable("minio_secretkey")
    )
    .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();
