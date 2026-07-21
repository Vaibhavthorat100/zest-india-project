using StudentManagementAPI.Common.Security;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var defaultAdmin = new ApplicationUser
                {
                    Username = "admin",
                    Email = "admin@zestindia.com",
                    PasswordHash = PasswordHasher.HashPassword("Admin@123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(defaultAdmin);
            }

            if (!context.Students.Any())
            {
                var initialStudents = new List<Student>
                {
                    new Student
                    {
                        Name = "Rahul Sharma",
                        Email = "rahul.sharma@example.com",
                        Age = 22,
                        Course = "Computer Science",
                        CreatedDate = DateTime.UtcNow.AddDays(-10)
                    },
                    new Student
                    {
                        Name = "Priya Patel",
                        Email = "priya.patel@example.com",
                        Age = 21,
                        Course = "Information Technology",
                        CreatedDate = DateTime.UtcNow.AddDays(-8)
                    },
                    new Student
                    {
                        Name = "Aman Verma",
                        Email = "aman.verma@example.com",
                        Age = 23,
                        Course = "Software Engineering",
                        CreatedDate = DateTime.UtcNow.AddDays(-5)
                    },
                    new Student
                    {
                        Name = "Sneha Gupta",
                        Email = "sneha.gupta@example.com",
                        Age = 20,
                        Course = "Data Science",
                        CreatedDate = DateTime.UtcNow.AddDays(-2)
                    }
                };

                context.Students.AddRange(initialStudents);
            }

            context.SaveChanges();
        }
    }
}
