using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reversi_api.Data;
using Reversi_api.Services;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Reversi_api

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("https://localhost:8000",
                                        "http://localhost:8000")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                  });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("JWTIssuer") ?? throw new InvalidOperationException("No JWT issuer found"),
                        ValidAudience = Environment.GetEnvironmentVariable("JWTIssuer"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTKey") ?? throw new InvalidOperationException("No JWT key found")))
                    };
                });
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            //options => builder.Configuration.Bind("CookieSettings", options));

            builder.Services.AddAuthorization();

            // Add services to the container.
            builder.Services.AddDbContext<ReversiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ReversiContext") ?? throw new InvalidOperationException("Connection string 'ReversiContext' not found.")));

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllersWithViews();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddRouting();
            //builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);


            app.UseAuthentication();

            app.UseAuthorization();


            //app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}