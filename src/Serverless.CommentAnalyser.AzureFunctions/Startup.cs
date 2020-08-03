using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Serverless.CommentAnalyser.AzureFunctions;
using Serverless.CommentAnalyser.AzureFunctions.Abstractions;
using Serverless.CommentAnalyser.Services;
using Serverless.CommentAnalyser.Services.Abstractions;
using Serverless.CommentAnalyser.Services.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Serverless.CommentAnalyser.AzureFunctions
{
    internal class Startup :  FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<ISettings, Settings>();

            services.AddLogging();

            services.AddHttpClient("watsonClient", (sp, clientConfiguration) =>
            {
                var settings = sp.GetService<ISettings>();

                clientConfiguration.BaseAddress = new Uri(settings.TextAnalyserUrl);

                clientConfiguration.DefaultRequestHeaders.Add("ContentType", settings.TextAnalyserContentType);

                var authToken = Encoding.ASCII.GetBytes(settings.TextAnalyserAuthToken);

                clientConfiguration.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            });

            services.AddSingleton<ITextAnalyzer, TextAnalyzer>();

            services.AddSingleton<ICommentRepository, CommentRepository>(sp => {
                var settings = sp.GetService<ISettings>();

                return new CommentRepository(settings.AzureStorageConenectionString, sp.GetService<ILogger<ICommentRepository>>());
            });

            services.AddSingleton<IJobRepository, JobRepository>(sp => {
                var settings = sp.GetService<ISettings>();

                return new JobRepository(settings.AzureStorageConenectionString, sp.GetService<ILogger<IJobRepository>>());
            });

            services.AddSingleton<IJobService, JobService>();

            
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.SpecVersion = OpenApiSpecVersion.OpenApi3_0;
                opts.AddCodeParameter = true;
                opts.PrependOperationWithRoutePrefix = true;
                opts.XmlPath = "CommentAnalyser.xml";
                opts.Documents = new[]
                {
                    new SwaggerDocument
                    {
                        Name = "v1",
                        Title = "Comment Analyser Api",
                        Description = "The api allows to analyse comment tone (using Watson service from IBM) and save them to a database (Azure Storage Table)",
                        Version = "v1"
                    }
                };
                opts.Title = "Comment Analyser Api";

                opts.ConfigureSwaggerGen = (x =>
                {
                    x.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                            ? methodInfo.Name
                            : new Guid().ToString();
                    });
                });
            });
        }
    }
}
