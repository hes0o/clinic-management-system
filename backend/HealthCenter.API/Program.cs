using HealthCenter.API.Extensions;
using HealthCenter.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "HealthCenter API - SOLID Compliant", Version = "v1" });
});

// Register dependencies using extension methods - OCP compliant
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddMappers();
builder.Services.AddValidators();
builder.Services.AddEventHandlers();

var app = builder.Build();

// Configure middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();