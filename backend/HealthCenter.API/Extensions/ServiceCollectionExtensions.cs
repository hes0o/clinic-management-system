using HealthCenter.Application.Common;
using HealthCenter.Application.Dtos;
using HealthCenter.Application.EventHandlers;
using HealthCenter.Application.Interfaces;
using HealthCenter.Application.Mappers;
using HealthCenter.Application.Services;
using HealthCenter.Application.Validators;
using HealthCenter.Domain.Entities;
using HealthCenter.Domain.Events;
using HealthCenter.Infrastructure.Repositories;

namespace HealthCenter.API.Extensions;

/// <summary>
/// Service collection extensions - OCP compliant for dependency registration
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IPatientService, PatientService>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IPatientRepository, PatientRepository>();
        
        return services;
    }

    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        // Mappers - OCP compliant, easily replaceable
        services.AddScoped<IMapper<Patient, PatientDto>, PatientMapper>();
        
        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // Validators - OCP compliant, easily extensible
        services.AddScoped<IValidator<CreatePatientRequest>, CreatePatientRequestValidator>();
        
        return services;
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        // Event handlers - OCP compliant for event-driven architecture
        services.AddScoped<IDomainEventHandler<PatientCreatedEvent>, PatientCreatedEventHandler>();
        
        return services;
    }
}
