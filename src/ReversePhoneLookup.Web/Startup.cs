using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Api;
using ReversePhoneLookup.Api.Repositories;
using ReversePhoneLookup.Api.Services;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Responses;
using ReversePhoneLookup.Web.Filters;

namespace ReversePhoneLookup.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PhoneLookupContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("ReversePhoneLookup.Web"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhoneLookup", Version = "v1" });
            });

            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<IPhoneRepository, PhoneRepository>();
            services.AddTransient<IPhoneService, PhoneService>();
            services.AddTransient<ILookupService, LookupService>();

            services.AddControllers(options =>
            {
                options.Filters.Add(new ApiExceptionFilter());
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(new ErrorResponse()
                    {
                        Code = StatusCode.RequestError,
                        Message = string.Join(". ", context.ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage))
                    });

                    return result;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneLookup v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello ReversePhoneLookup.\nTODO: Add POST /phone, POST /contacts and Integration tests!");
                });
            });
        }
    }
}
