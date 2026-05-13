using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesProject.Application;
using SalesProject.IoC;
using SalesProject.Common.HealthChecks;
using SalesProject.Common.Logging;
using SalesProject.Common.Security;
using SalesProject.Common.Validation;
using SalesProject.ORM;
using SalesProject.WebApi.Middleware;
using Serilog;


namespace SalesProject.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SalesProject.ORM")
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(
                cfg => { },
                typeof(Program).Assembly,
                typeof(ApplicationLayer).Assembly
            );

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var app = builder.Build();

            if (builder.Configuration.GetValue<bool>("APPLY_MIGRATIONS"))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    var retries = 10;

                    while (retries > 0)
                    {
                        try
                        {
                            logger.LogInformation("Applying database migrations...");

                            db.Database.Migrate();

                            logger.LogInformation("Database ready.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            retries--;

                            logger.LogWarning(ex, "Error connecting to database. Retries left: {Retries}", retries);

                            if (retries == 0)
                                throw;

                            Thread.Sleep(3000);
                        }
                    }
                }
            }

            app.UseMiddleware<ValidationExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

