using Xems.Application.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Xems.Api
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
			builder.Services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT"
				});

				options.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
				{
						new OpenApiSecurityScheme
						{
								Reference = new OpenApiReference
								{
										Type = ReferenceType.SecurityScheme,
										Id = "Bearer"
								}
						},
						Array.Empty<string>()
				}
		});
			});

			builder.Services.AddSingleton<IElevatorDispatcher, ElevatorDispatcher>();
			builder.Services.AddSingleton<ElevatorService>();

			var jwtSection = builder.Configuration.GetSection("Jwt");
			var jwtSecret = jwtSection["Secret"]!;

			builder.Services
					.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidateAudience = true,
							ValidateIssuerSigningKey = true,
							ValidateLifetime = true,
							ValidIssuer = jwtSection["Issuer"],
							ValidAudience = jwtSection["Audience"],
							IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(jwtSecret))
						};
					});

			builder.Services.AddAuthorization();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			//if (app.Environment.IsDevelopment())
			//{
			//	app.UseSwagger();
			//	app.UseSwaggerUI();
			//}
			// Swagger skal være tilgjengelig ved oppstart på /swagger
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.RoutePrefix = "swagger";
			});

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}

public partial class Program { }
