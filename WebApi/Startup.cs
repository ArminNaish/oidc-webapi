using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApi
{
    public class Startup
    {
        private const string AllowAllOriginsPolicy = "AllowAllOriginsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // IdentityModelEventSource.ShowPII = true; 

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:8080/auth/realms/master";
                    options.Audience = "test-api";
                    options.RequireHttpsMetadata = false; // used for development purposes only

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudience = "test-api",
                        ValidIssuer = "http://0.0.0.0:8080/auth/realms/master"
                    };

                    // options.Events = new JwtBearerEvents()
                    // {
                    //     OnAuthenticationFailed = context =>
                    //     {
                    //         log exception
                    //     }
                    // };
                });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOriginsPolicy,
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(AllowAllOriginsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
