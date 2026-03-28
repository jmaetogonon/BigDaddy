using BigDaddy.Api.Extensions;
using BigDaddy.Api.Middleware;
using BigDaddy.Api.Services;
using BigDaddy.Application;
using BigDaddy.Identity.Extensions;
using BigDaddy.Persistence;
using BigDaddy.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

// ── Permission policies ───────────────────────────────────────────────────────
builder.Services.AddPermissionPolicies(
    // ── User module ────────────────────────────────────────────────────
    "users.list.view",
    "users.detail.view",
    "users.create",
    "users.edit",
    "users.delete",
    "users.activate",
    "users.deactivate",
    "users.lock",
    "users.unlock",
    "users.reset-password",
    "users.assign-roles",
    "users.assign-teams"
);

// ── Background services ───────────────────────────────────────────────────────
builder.Services.AddHostedService<TokenCleanupService>();

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "App API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(document =>
       new OpenApiSecurityRequirement
       {
           [new OpenApiSecuritySchemeReference("Bearer", document)] = []
       });
});

// ─────────────────────────────────────────────────────────────────────────────

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();         // 1. global error handler

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();                          // 2. validates JWT
app.UseMiddleware<TokenValidationMiddleware>();   // 3. checks token blacklist
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();               // apply pending migrations
    await DbSeeder.SeedAsync(db);                   // seed data
}

app.Run();