using HealthCenter.Application.Dtos;
using HealthCenter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthCenter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientResponseDto>> Create([FromBody] CreatePatientRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _patientService.AddPatientAsync(request);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PatientResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PatientResponseDto>>> GetAll()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }
}
