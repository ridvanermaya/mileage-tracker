using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MileageTracker.WebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Text;

namespace MileageTracker.WebAPI
{
    public class Startup
    {

        public static string AuthenticationSecret = "RIDVAN-3455-TR-USA";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                ));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var key = Encoding.ASCII.GetBytes(AuthenticationSecret);
            services.AddAuthentication(x =>
                       {
                           x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                           x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                       })
                       .AddJwtBearer(x =>
                       {
                           x.Events = new JwtBearerEvents
                           {
                               OnTokenValidated = context =>
                               {
                                   var userService = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                                   var userId = context.Principal.Identity.Name;
                                   var user = userService.FindByIdAsync(userId).GetAwaiter().GetResult();
                                   if (user == null)
                                   {
                                       // return unauthorized if user no longer exists
                                       context.Fail("Unauthorized");
                                   }
                                   return Task.CompletedTask;
                               }
                           };
                           x.RequireHttpsMetadata = false;
                           x.SaveToken = true;
                           x.TokenValidationParameters = new TokenValidationParameters
                           {
                               ValidateIssuerSigningKey = true,
                               IssuerSigningKey = new SymmetricSecurityKey(key),
                               ValidateIssuer = false,
                               ValidateAudience = false
                           };
                       });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://127.0.0.1:5500").AllowCredentials().AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });

            services.AddHangfire(options =>
                options.UseSqlServerStorage(
                    Configuration.GetConnectionString("HangfireConnection")
                ));
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
            app.UseHangfireDashboard();
        }
    }
}
