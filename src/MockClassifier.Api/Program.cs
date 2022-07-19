using Buerokratt.Common.CentOps;
using Buerokratt.Common.Dmr;
using Buerokratt.Common.Dmr.Extensions;
using Buerokratt.Common.Encoder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MockClassifier.Api.Interfaces;
using MockClassifier.Api.Services;
using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api
{
    [ExcludeFromCodeCoverage] // This is not solution code, no need for unit tests
    public static class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var services = builder.Services;

            // Add services to the container.
            var dmrSettings = configuration.GetSection("DmrServiceSettings").Get<DmrServiceSettings>();
            var centOpsSettings = configuration.GetSection("DmrServiceSettings").Get<CentOpsServiceSettings>();

            services.AddDmrService(dmrSettings, centOpsSettings);
            services.TryAddSingleton<ITokenService, TokenService>();
            services.TryAddSingleton<INaturalLanguageService, NaturalLanguageService>();
            services.TryAddSingleton<IEncodingService, EncodingService>();

            _ = services.AddControllers();
            _ = services.AddEndpointsApiExplorer();
            _ = services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}