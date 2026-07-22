using Moq;
using StudentManagementAPI.Common.Exceptions;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;
using StudentManagementAPI.Repositories;
using StudentManagementAPI.Services;
using Xunit;

namespace StudentManagementAPI.Tests
{
    public class StudentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStudentRepository> _mockStudentRepo;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStudentRepo = new Mock<IStudentRepository>();
            _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepo.Object);

            _studentService = new StudentService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
        {
            // Arrange
            var mockStudents = new List<Student>
            {
                new Student { Id = 1, Name = "Rahul Sharma", Email = "rahul@example.com", Age = 22, Course = "CS", CreatedDate = DateTime.UtcNow },
                new Student { Id = 2, Name = "Priya Patel", Email = "priya@example.com", Age = 21, Course = "IT", CreatedDate = DateTime.UtcNow }
            };

            _mockStudentRepo.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockStudents);

            // Act
            var result = await _studentService.GetAllStudentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockStudentRepo.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ExistingId_ShouldReturnStudent()
        {
            // Arrange
            var student = new Student { Id = 1, Name = "Rahul Sharma", Email = "rahul@example.com", Age = 22, Course = "CS" };
            _mockStudentRepo.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(student);

            // Act
            var result = await _studentService.GetStudentByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Rahul Sharma", result.Name);
            Assert.Equal("rahul@example.com", result.Email);
        }

        [Fact]
        public async Task GetStudentByIdAsync_NonExistingId_ShouldReturnNull()
        {
            // Arrange
            _mockStudentRepo.Setup(repo => repo.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Student?)null);

            // Act
            var result = await _studentService.GetStudentByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateStudentAsync_ValidStudent_ShouldSaveAndReturnDto()
        {
            // Arrange
            var createDto = new CreateStudentDto
            {
                Name = "Aman Verma",
                Email = "aman@example.com",
                Age = 23,
                Course = "Software Engineering"
            };

            var createdStudent = new Student
            {
                Id = 10,
                Name = createDto.Name,
                Email = createDto.Email,
                Age = createDto.Age,
                Course = createDto.Course,
                CreatedDate = DateTime.UtcNow
            };

            _mockStudentRepo.Setup(repo => repo.EmailExistsAsync(createDto.Email, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockStudentRepo.Setup(repo => repo.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdStudent);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studentService.CreateStudentAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("Aman Verma", result.Name);
            _mockStudentRepo.Verify(repo => repo.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateStudentAsync_DuplicateEmail_ShouldThrowConflictException()
        {
            // Arrange
            var createDto = new CreateStudentDto
            {
                Name = "Aman Verma",
                Email = "existing@example.com",
                Age = 23,
                Course = "Software Engineering"
            };

            _mockStudentRepo.Setup(repo => repo.EmailExistsAsync(createDto.Email, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _studentService.CreateStudentAsync(createDto));
            _mockStudentRepo.Verify(repo => repo.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteStudentAsync_ExistingId_ShouldReturnTrue()
        {
            // Arrange
            _mockStudentRepo.Setup(repo => repo.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _studentService.DeleteStudentAsync(1);

            // Assert
            Assert.True(result);
            _mockStudentRepo.Verify(repo => repo.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentAsync_NonExistingId_ShouldThrowNotFoundException()
        {
            // Arrange
            _mockStudentRepo.Setup(repo => repo.DeleteAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _studentService.DeleteStudentAsync(999));
        }
    }
}
