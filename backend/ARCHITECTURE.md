# HealthCenter - Patient Management Architecture

## SOLID Principles Implementation

### 1. Open/Closed Principle (OCP) ✅
**Open for extension, closed for modification**

- **Validators**: `IValidator<T>` interface allows adding new validators without modifying existing code
- **Mappers**: `IMapper<TSource, TDestination>` enables custom mapping strategies
- **Specifications**: `ISpecification<T>` pattern for extensible query logic
- **Event Handlers**: `IDomainEventHandler<TEvent>` for event-driven extensions
- **Result Pattern**: Standardized error handling without exceptions

**Example**: Add new validation rules by implementing `IValidator<T>`:
```csharp
public class AdvancedPatientValidator : IValidator<CreatePatientRequest>
{
    // New validation logic without touching existing validators
}
```

### 2. Liskov Substitution Principle (LSP) ✅
**Subtypes must be substitutable for their base types**

- **Repository Pattern**: `IPatientRepository` extends `IRepository<Patient>`, maintaining contract
- **Domain Entities**: Private constructors + factory methods prevent invalid state
- **Value Objects**: Immutable with validation in factory methods
- **Mappers**: Any `IMapper<Patient, PatientDto>` implementation is interchangeable

**Example**: Repository implementations are fully substitutable:
```csharp
IPatientRepository repo = new PatientRepository(); // In-memory
IPatientRepository repo = new EFPatientRepository(); // Entity Framework
IPatientRepository repo = new DapperPatientRepository(); // Dapper
// All work identically from consumer perspective
```

### 3. Dependency Inversion Principle (DIP) ✅
**Depend on abstractions, not concretions**

- **Service Layer**: Depends on `IPatientRepository`, `IMapper<T>`, `IValidator<T>`
- **Controller Layer**: Depends on `IPatientService` abstraction
- **Domain Layer**: Defines contracts (`IRepository<T>`) without implementation
- **Infrastructure**: Implements domain-defined interfaces

**Dependency Flow**:
```
API (Controllers) → Application (Services) → Domain (Entities)
                ↓                    ↓
         Infrastructure (Repositories)
```

## Architecture Layers

### Domain Layer (Core)
- **Entities**: `Patient` with encapsulated business logic
- **Value Objects**: `FullName`, `PhoneNumber` with validation
- **Events**: `PatientCreatedEvent` for domain events
- **Specifications**: Query logic encapsulation
- **Interfaces**: `IRepository<T>`, `IEntity`, `IDomainEvent`

### Application Layer
- **Services**: `PatientService` orchestrates business operations
- **DTOs**: `PatientDto`, `CreatePatientRequest`
- **Interfaces**: `IPatientService`, `IPatientRepository`
- **Mappers**: `PatientMapper` for entity-DTO conversion
- **Validators**: `CreatePatientRequestValidator`
- **Event Handlers**: `PatientCreatedEventHandler`

### Infrastructure Layer
- **Repositories**: `PatientRepository` implements data access
- **Persistence**: Database context (future: EF Core)

### API Layer
- **Controllers**: `PatientsController` handles HTTP requests
- **Middleware**: `ExceptionHandlingMiddleware` for global error handling
- **Extensions**: `ServiceCollectionExtensions` for DI configuration

## Key Patterns

### 1. Repository Pattern
Abstracts data access, enables testing and swapping implementations

### 2. Result Pattern
Type-safe error handling without exceptions:
```csharp
var result = await _service.AddPatientAsync(request);
if (!result.IsSuccess)
    return BadRequest(result.Error);
```

### 3. Factory Pattern
Domain entities created through factory methods ensuring validity:
```csharp
var patient = Patient.Create(fullName, phone);
```

### 4. Specification Pattern
Encapsulates query logic for reusability and testability

### 5. Domain Events
Decoupled event handling for cross-cutting concerns

## Extension Points

### Adding New Features
1. **New Entity**: Create in Domain layer, implement `Entity` base class
2. **New Repository**: Implement `IRepository<T>` in Infrastructure
3. **New Service**: Create interface in Application, implement with DI
4. **New Validator**: Implement `IValidator<T>`
5. **New Event Handler**: Implement `IDomainEventHandler<TEvent>`

### Future Enhancements
- ✅ Unit of Work for transaction management
- ✅ Specification pattern for complex queries
- ✅ Domain event dispatching
- 🔄 CQRS pattern (Command/Query separation)
- 🔄 MediatR for request/response handling
- 🔄 FluentValidation integration
- 🔄 AutoMapper integration
- 🔄 Entity Framework Core
- 🔄 Authentication & Authorization
- 🔄 API versioning
- 🔄 Rate limiting
- 🔄 Caching strategy

## Testing Strategy

### Unit Tests
- Domain entities and value objects
- Application services with mocked repositories
- Validators and mappers

### Integration Tests
- API endpoints with test database
- Repository implementations

### Architecture Tests
- Verify dependency rules
- Ensure SOLID compliance

## Benefits

✅ **Maintainability**: Clear separation of concerns
✅ **Testability**: All dependencies are abstractions
✅ **Extensibility**: New features without modifying existing code
✅ **Flexibility**: Easy to swap implementations
✅ **Scalability**: Clean architecture supports growth
✅ **Future-proof**: SOLID principles ensure long-term stability
