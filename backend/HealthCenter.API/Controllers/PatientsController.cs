using HealthCenter.Application.Services;
using HealthCenter.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HealthCenter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientService _patientService;

    public PatientsController(PatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetAll()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetById(Guid id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);
        if (patient == null)
            return NotFound();
        
        return Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<Patient>> Create(Patient patient)
    {
        var createdPatient = await _patientService.AddPatientAsync(patient);
        return CreatedAtAction(nameof(GetById), new { id = createdPatient.Id }, createdPatient);
    }
}
