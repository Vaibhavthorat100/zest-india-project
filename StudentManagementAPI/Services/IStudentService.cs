using StudentManagementAPI.DTOs;

namespace StudentManagementAPI.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default);
        Task<StudentResponseDto?> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto createDto, CancellationToken cancellationToken = default);
        Task<StudentResponseDto?> UpdateStudentAsync(int id, UpdateStudentDto updateDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken = default);
    }
}
