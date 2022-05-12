using MockClassifier.Api.Services.Dmr;
using MockClassifier.Api.Services.Dmr.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
var dmrSettings = configuration.GetSection("DmrServiceSettings").Get<DmrServiceSettings>();
services.AddDmrService(dmrSettings);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

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
