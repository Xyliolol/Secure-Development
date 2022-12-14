using CardStorageService.Services.Impl;
using CardStorageService.Services;
using Microsoft.AspNetCore.HttpLogging;
using NLog.Web;
using CardStorageService.Models;
using CardStorageService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using CardStorageService.Models.Requests;
using AutoMapper;
using CardStorageService.Models.Validators;
using FluentValidation;
using CardStorageService.Mappings;
using System.Net;
using CardStorageService.Services.Implementation;

namespace CardStorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5001, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });

            builder.Services.AddGrpc();

            builder.Services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
            builder.Services.AddScoped<IValidator<CreateCardRequest>, CardRequestValidator>();
            builder.Services.AddScoped<IValidator<CreateClientRequest>, ClientRequestValidator>();

            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MappingsProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            builder.Services.AddSingleton(mapper);

            builder.Services.Configure<DatabaseOptions>(options =>
            {
                builder.Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });


            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });

            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();

            }).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });


            builder.Services.AddDbContext<CardStorageServiceDbContext>(options =>
            {

                options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });


            builder.Services.AddScoped<IClientRepositoryService, ClientRepository>();
            builder.Services.AddScoped<ICardRepositoryService, CardRepository>();
            builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new
               TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ClockSkew = TimeSpan.Zero
               };
           });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ìîé ñåðâèñ", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseWhen(c => c.Request.ContentType != "application/grpc",
               builder =>
               {
                   builder.UseHttpLogging();
               }
           );

            //app.UseHttpLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ClientService>();
                endpoints.MapGrpcService<CardService>();

            });
            
            app.MapControllers();

            app.Run();
        }
    }
}