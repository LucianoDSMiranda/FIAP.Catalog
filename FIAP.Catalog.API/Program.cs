using FIAP.Catalog.API.Logging;
using FIAP.Catalog.Application.Abstractions.Repositories;
using FIAP.Catalog.Application.UseCases;
using FIAP.Catalog.Infrastructure.Messaging;
using FIAP.Catalog.Infrastructure.Persistence;
using FIAP.Catalog.Infrastructure.Persistence.Mappings;
using FIAP.Catalog.Infrastructure.Repositories;
using FIAP.Messages;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Prometheus;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseCloudGamesLogging("catalog-api");

    Log.Information("Starting FIAP.Catalog API...");

    var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq-service";
    var paymentProcessedQueue = Environment.GetEnvironmentVariable("PAYMENT_PROCESSED_QUEUE") ?? "PaymentProcessedEvent";
    var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "CloudGames.Users";
    var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "CloudGames.Users.Client";
    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "SUPER_SECRET_KEY_123456789_123456789";

    Log.Information("RabbitMQ Host: {RabbitHost}", rabbitHost);

    #region SWAGGER

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "FIAP.Catalog API",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Informe: Bearer {token}"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    #endregion

    builder.Services.AddControllers();

    #region AUTH

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

    builder.Services.AddAuthorization();

    #endregion

    #region DATABASE

    CatalogClassMaps.Register();

    var mongoConnection = Environment.GetEnvironmentVariable("MongoSettings__ConnectionString")
                          ?? builder.Configuration["MongoSettings:ConnectionString"]
                          ?? throw new InvalidOperationException("MongoSettings:ConnectionString não configurado.");

    var mongoDatabase = Environment.GetEnvironmentVariable("MongoSettings__Database")
                        ?? builder.Configuration["MongoSettings:Database"]
                        ?? throw new InvalidOperationException("MongoSettings:Database não configurado.");

    var mongoSettings = new MongoSettings
    {
        ConnectionString = mongoConnection,
        Database = mongoDatabase
    };

    builder.Services.AddSingleton(mongoSettings);
    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnection));
    builder.Services.AddScoped<MongoContext>();

    #endregion

    #region REPOSITORIES

    builder.Services.AddScoped<IGameRepository, GameRepository>();
    builder.Services.AddScoped<IUserGameRepository, UserGameRepository>();

    #endregion

    #region USE CASES

    builder.Services.AddScoped<ListGamesUseCase>();
    builder.Services.AddScoped<GetGameByIdUseCase>();
    builder.Services.AddScoped<CreateGameUseCase>();
    builder.Services.AddScoped<PurchaseGameUseCase>();
    builder.Services.AddScoped<ListUserGamesUseCase>();
    builder.Services.AddScoped<RegisterUserGameUseCase>();

    #endregion

    #region MASSTRANSIT

    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<PaymentProcessedEventConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitHost, "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.Message<OrderPlacedEvent>(x => x.SetEntityName("OrderPlacedEvent"));
            cfg.Message<PaymentProcessedEvent>(x => x.SetEntityName("PaymentProcessedEvent"));

            cfg.ReceiveEndpoint(paymentProcessedQueue, e =>
            {
                e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);
            });
        });
    });

    #endregion

    var app = builder.Build();

    app.UseHttpMetrics();

    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer>
            {
                new() { Url = $"{httpReq.Scheme}://{httpReq.Host}/catalog" }
            };
        });
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("swagger/v1/swagger.json", "FIAP.CATALOG API v1");
        c.RoutePrefix = string.Empty;
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapMetrics();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application stopped because of exception");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
