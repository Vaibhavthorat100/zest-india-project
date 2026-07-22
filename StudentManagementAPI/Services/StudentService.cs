using StudentManagementAPI.Common.Exceptions;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Repositories;

namespace StudentManagementAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default)
        {
            var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);
            return students.Select(MapToResponseDto);
        }

        public async Task<StudentResponseDto?> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
            return student == null ? null : MapToResponseDto(student);
        }

        public async Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto createDto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Students.EmailExistsAsync(createDto.Email, cancellationToken: cancellationToken))
            {
                throw new ConflictException($"A student with email '{createDto.Email}' already exists.");
            }

            var student = new Student
            {
                Name = createDto.Name.Trim(),
                Email = createDto.Email.Trim(),
                Age = createDto.Age,
                Course = createDto.Course.Trim(),
                CreatedDate = DateTime.UtcNow
            };

            var createdStudent = await _unitOfWork.Students.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToResponseDto(createdStudent);
        }

        public async Task<StudentResponseDto?> UpdateStudentAsync(int id, UpdateStudentDto updateDto, CancellationToken cancellationToken = default)
        {
            var existingStudent = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
            if (existingStudent == null)
            {
                throw new NotFoundException("Student", id);
            }

            if (await _unitOfWork.Students.EmailExistsAsync(updateDto.Email, excludeId: id, cancellationToken: cancellationToken))
            {
                throw new ConflictException($"A student with email '{updateDto.Email}' already exists.");
            }

            existingStudent.Name = updateDto.Name.Trim();
            existingStudent.Email = updateDto.Email.Trim();
            existingStudent.Age = updateDto.Age;
            existingStudent.Course = updateDto.Course.Trim();

            await _unitOfWork.Students.UpdateAsync(existingStudent, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToResponseDto(existingStudent);
        }

        public async Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken = default)
        {
            var deleted = await _unitOfWork.Students.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                throw new NotFoundException("Student", id);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static StudentResponseDto MapToResponseDto(Student student)
        {
            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Age = student.Age,
                Course = student.Course,
                CreatedDate = student.CreatedDate
            };
        }
    }
}
