using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Text.Json;
using ValidationLoggingBebugging.Helpers;
using ValidationLoggingBebugging.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers()
	.AddFluentValidation(fv =>
	{
		fv.RegisterValidatorsFromAssemblyContaining<UserRequestValidator>();
	});

builder.Services.AddScoped<IUserService, UserService>();

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.File(
		path: "Logs/logInfo.txt",
		rollingInterval: RollingInterval.Day,
		retainedFileCountLimit: 7,
		outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
	)
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services
	.AddControllers()
	.AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(config =>
{
	config.Run(async context =>
	{
		var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = exception is DuplicateUsernameException ? 400 : 500;

		var message = exception is DuplicateUsernameException
			? "There is already a user with username."
			: "Something went wrong. Please try again later.";

		Log.Error(exception, "Exception occurred");

		await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
	});
});



app.UseAuthorization();

app.MapControllers();

app.Run();
