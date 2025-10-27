using IzhevskEventRadar.Api.BuilderExtensions;
using IzhevskEventRadar.DataBase.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddServiceRegister(builder.Configuration);
builder.Services.AddHangfireWithServer(builder.Configuration);
builder.Services.AddGrpc();

builder.Services.AddCors(options =>
{
    options.AddPolicy("RequestPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:51054");
        policy.AllowCredentials();
        policy.WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS");
        policy.AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks();
builder.AddDbContext();

var app = builder.Build();

app.MapHealthChecks("/healthz").RequireHost("localhost");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EventRadarDbContext>>();
    using var context = db.CreateDbContext();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("RequestPolicy");

app.UseGrpc();

app.UseEndpoints();

app.UseHangfireSecureDashboard();
app.InitializeRecurringJobs(app.Configuration);

app.Run();
