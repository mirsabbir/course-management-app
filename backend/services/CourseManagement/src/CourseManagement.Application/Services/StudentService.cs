using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseManagement.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserService _userService;

        public StudentService(
            IStudentRepository studentRepository,
            IUserService userService)
        {
            _studentRepository = studentRepository;
            _userService = userService;
        }

        public async Task<StudentDTO> CreateAsync(CreateStudentDTO createStudentDTO)
        {
            var userId = await _userService.CreateUserAsync(new DTOs.CreateUserDTO
            {
                Email = createStudentDTO.Email,
                FullName = createStudentDTO.FullName,
            });

            var newStudent = new Student
            {
                Id = Guid.NewGuid(),
                FullName = createStudentDTO.FullName,
                Email = createStudentDTO.Email,
                DateOfBirth = createStudentDTO.DateOfBirth,
                UserId = Guid.Parse(userId)
            };

            await _studentRepository.AddStudentAsync(newStudent);

            return new StudentDTO
            {
                Id = newStudent.Id,
                Email = newStudent.Email,
                DateOfBirth = newStudent.DateOfBirth,
                FullName = newStudent.FullName,
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            await _studentRepository.DeleteStudentAsync(id);
        }

        public async Task<IEnumerable<StudentDTO>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllStudentsAsync();

            return students.Select(s => new StudentDTO
            {
                Id = s.Id,
                Email = s.Email,
                FullName = s.FullName,
                DateOfBirth = s.DateOfBirth,
            });
        }

        public async Task<StudentDTO> GetByIdAsync(Guid id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            return new StudentDTO
            {
                Id = student.Id,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email
            };
        }

        public async Task<StudentDTO> UpdateAsync(UpdateStudentDTO updateStudentDTO)
        {
            var student = await _studentRepository.GetStudentByIdAsync(updateStudentDTO.Id);

            if (student == null)
                throw new KeyNotFoundException("Student not found.");

            student.FullName = updateStudentDTO.FullName;
            student.DateOfBirth = updateStudentDTO.DateOfBirth;

            await _studentRepository.UpdateStudentAsync(student);

            return new StudentDTO
            {
                Id = student.Id,
                DateOfBirth = student.DateOfBirth,
                FullName = student.FullName,
                Email = student.Email
            };
        }
    }
}
