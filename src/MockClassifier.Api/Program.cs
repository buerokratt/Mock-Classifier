using Microsoft.Extensions.DependencyInjection.Extensions;
using MockClassifier.Api.Interfaces;
using MockClassifier.Api.Services;
using MockClassifier.Api.Services.Dmr;
using MockClassifier.Api.Services.Dmr.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace MockClassifierr.Api
{
    [ExcludeFromCodeCoverage] // This is not solution code, no need for unit tests
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var services = builder.Services;

            // Add services to the container.
            var dmrSettings = configuration.GetSection("DmrServiceSettings").Get<DmrServiceSettings>();
            services.AddDmrService(dmrSettings);
            services.TryAddSingleton<ITokenService, TokenService>();
            services.TryAddSingleton<INaturalLanguageService, NaturalLanguageService>();

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
        }
    }
}