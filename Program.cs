using Microsoft.EntityFrameworkCore;
using FIAP.Catalog.Data;
using FIAP.Catalog.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FIAP.Messages;
using FIAP.Catalog.Consumers;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("PROGRAM NOVA DO CATALOG");
Console.WriteLine($"RABBITMQ_HOST={Environment.GetEnvironmentVariable("RABBITMQ_HOST")}");

var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq-service";

var paymentProcessedQueue = Environment.GetEnvironmentVariable("PAYMENT_PROCESSED_QUEUE") ?? "PaymentProcessedEvent";

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "CloudGames.Users";

var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "CloudGames.Users.Client";

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "SUPER_SECRET_KEY_123456789_123456789";

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

builder.Services.AddControllers();

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

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite("Data Source=catalog.db"));

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP.CATALOG API v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();