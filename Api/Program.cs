using System.Text;
using HcAgents.Common.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppDependencies(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(cfg =>
    {
        cfg.SwaggerEndpoint("/v1/swagger.json", "HCAgents API V1");
        cfg.RoutePrefix = "swagger";
        cfg.DocumentTitle = "HCAgents API - Swagger Docs";
    });
}

app.MapControllers();
app.UseAuthorization();

app.UseCors(cfg => cfg.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Run();
