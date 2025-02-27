﻿using CourseManagement.Application.DTOs;
using CourseManagement.Application.DTOs.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface IStudentService
    {
        Task<(IEnumerable<StudentDTO> Students, int TotalCount)> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<StudentDTO> GetByIdAsync(Guid id);
        Task<StudentDTO> CreateAsync(CreateStudentDTO createStudentDTO);
        Task<StudentDTO> UpdateAsync(UpdateStudentDTO updateStudentDTO);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<StudentDTO>> SearchStudentsAsync(string searchTerm);
    }
}
