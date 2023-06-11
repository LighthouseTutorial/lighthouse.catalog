using Lighthouse.Catalog.Service.Entities;
using Lighthouse.Common.Settings;
using Lighthouse.Common.MongoDb;
using Lighthouse.Common.MassTransit;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Lighthouse.Common.Identity;

namespace Lighthouse.Catalog.Service
{
    public class Startup
    {
        private const string AllowedOriginSetting = "AllowedOrigin";
        private ServiceSettings? _serviceSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            services.AddMongo().AddMongoRepository<Item>("items");
            services.AddMassTransitWithRabbitMQ();
            services.AddJwtBearerAuthentication();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Read, policy =>
                {
                    policy.RequireRole("Admin");
                    policy.RequireClaim("scope", "catalog.readaccess", "catalog.fullaccess");
                });

                options.AddPolicy(Policies.Write, policy =>
                {
                    policy.RequireRole("Admin");
                    policy.RequireClaim("scope", "catalog.writeaccess", "catalog.fullaccess");
                });
            });

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lighthouse.Catalog.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lighthouse.Catalog.Service v1"));

                app.UseCors(builder =>
                {
                    var allowedOrigin = Configuration[AllowedOriginSetting];

                    builder.WithOrigins("*")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
