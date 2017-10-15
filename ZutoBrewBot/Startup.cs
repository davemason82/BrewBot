using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZutoBrewBot.Configuration;
using Microsoft.AspNetCore.Http;
using Common.Logging.Configuration;
using Common.Logging;
using ZutoBrewBot.Logging;
using ZutoBrewBot.Services.Interfaces;
using ZutoBrewBot.Services;
using Amazon.DynamoDBv2;
using ZutoBrewBot.Repositories.Interfaces;
using ZutoBrewBot.Repositories;
using ZutoBrewBot.Services.OrderDataExtractors.Interfaces;
using ZutoBrewBot.Services.OrderDataExtractors;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace ZutoBrewBot
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            var logConfiguration = new LogConfiguration();
            Configuration.GetSection("LogConfiguration").Bind(logConfiguration);
            LogManager.Configure(logConfiguration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)  
        {
            services.AddMvc();

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();

            services.AddSingleton<IMemoryLog, MemoryLog>();
            services.AddSingleton<IOrderBuilder, OrderBuilder>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IOrderFormatter, OrderFormatter>();
            services.AddSingleton<IOrderCache, OrderCache>();
            services.AddSingleton<ITableNumberExtractor, TableNumberExtractor>();
            services.AddSingleton<IMannersExtractor, MannersExtractor>();
            services.AddSingleton<ISlackRepository, SlackRepository>();
            services.AddSingleton<IMannersMessageGenerator, MannersMessageGenerator>();

            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env, IMemoryLog log)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var noobHost = new NoobotHost(new DotnetCoreConfigReader(Configuration.GetSection("bot")));
            applicationLifetime.ApplicationStarted.Register(() => noobHost.Start(log));
            applicationLifetime.ApplicationStopping.Register(noobHost.Stop);

            app.UseMvc();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                                        Path.Combine(Directory.GetCurrentDirectory(), @"js")),
                                            RequestPath = new PathString("/js")
                                        });

            app.Run(async context =>
            {
                await context.Response.WriteAsync(string.Empty);
            });
        }
    }
}
