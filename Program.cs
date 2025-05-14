
using API_JWTToken_Products.Context;
using API_JWTToken_Products.Interface;
using API_JWTToken_Products.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API_JWTToken_Products
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new() { Title = "API_JWTToken_Products", Version = "v1" });

            //    // Add JWT Authentication to Swagger
            //    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            //    {
            //        Name = "Authorization",
            //        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            //        Description = "Enter 'Bearer' and then your valid JWT token."
            //    });

            //    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            //    {
            //         {
            //             new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            //             {
            //                 Reference = new Microsoft.OpenApi.Models.OpenApiReference
            //                 {
            //                     Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            //                     Id = "Bearer"
            //                 }
            //             },
            //             new string[] {}
            //         }
            //     });
            //});

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "API_JWTToken_Products", Version = "v1" });

                // Modify the security definition to remove "Bearer" requirement in description
                c.AddSecurityDefinition("JWT", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter your JWT token here (without 'Bearer ' prefix).",
                    Scheme = "JWT"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                 {
                     {
                         new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                         {
                             Reference = new Microsoft.OpenApi.Models.OpenApiReference
                             {
                                 Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                 Id = "JWT"
                             },
                             In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                             Name = "Authorization"
                         },
                         Array.Empty<string>()
                     }
                 });
            });



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });



            builder.Services.AddDbContext<productContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IAuthService, AuthService>();

            //builder.Services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            //            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            //            IssuerSigningKey = new SymmetricSecurityKey(
            //                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
            //        };
            //    });
            builder.Services.AddAuthentication("Bearer")
             .AddJwtBearer("Bearer", options =>
             {
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var token = context.Request.Headers["Authorization"].FirstOrDefault();

                         if (!string.IsNullOrEmpty(token))
                         {
                             // Automatically add Bearer prefix if missing
                             if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                             {
                                 token = $"Bearer {token}";
                             }

                             context.Token = token.Substring("Bearer ".Length).Trim();
                         }

                         return Task.CompletedTask;
                     }
                 };

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                     ValidAudience = builder.Configuration["JwtSettings:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                 };
             });


            builder.Services.AddAuthorization();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
