using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all registered students.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching list of all students");
            var students = await _studentService.GetAllStudentsAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<StudentResponseDto>>.SuccessResponse(students, "Students retrieved successfully."));
        }

        /// <summary>
        /// Retrieves student details by unique ID.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching student details for ID: {StudentId}", id);
            var student = await _studentService.GetStudentByIdAsync(id, cancellationToken);

            if (student == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse($"Student with ID {id} was not found."));
            }

            return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(student, "Student details retrieved successfully."));
        }

        /// <summary>
        /// Registers a new student in the system.
        /// </summary>
        /// <param name="createDto">Student creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto createDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating student record for Name: {Name}, Email: {Email}", createDto.Name, createDto.Email);
            var student = await _studentService.CreateStudentAsync(createDto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = student.Id },
                ApiResponse<StudentResponseDto>.SuccessResponse(student, "Student added successfully."));
        }

        /// <summary>
        /// Updates an existing student record by ID.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="updateDto">Student update details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<StudentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto updateDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating student record for ID: {StudentId}", id);
            var updatedStudent = await _studentService.UpdateStudentAsync(id, updateDto, cancellationToken);

            return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(updatedStudent!, "Student record updated successfully."));
        }

        /// <summary>
        /// Removes a student record by ID.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting student record for ID: {StudentId}", id);
            await _studentService.DeleteStudentAsync(id, cancellationToken);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully."));
        }
    }
}
