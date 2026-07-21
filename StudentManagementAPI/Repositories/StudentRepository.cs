using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Students.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<Student> AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            student.CreatedDate = DateTime.UtcNow;
            await _context.Students.AddAsync(student, cancellationToken);
            return student;
        }

        public Task<Student> UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            _context.Students.Update(student);
            return Task.FromResult(student);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await GetByIdAsync(id, cancellationToken);
            if (student == null) return false;

            _context.Students.Remove(student);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Students.AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Students.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }
            return await query.AnyAsync(s => s.Email.ToLower() == email.ToLower(), cancellationToken);
        }
    }
}
