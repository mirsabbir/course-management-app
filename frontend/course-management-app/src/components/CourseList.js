// CourseList.js
import React from "react";
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Button } from "@mui/material";

const CourseList = ({ isStaff }) => {
  const courses = [
    { id: 1, name: "Math 101", description: "Basic Mathematics" },
    { id: 2, name: "Physics 101", description: "Fundamentals of Physics" },
  ];

  return (
    <TableContainer component={Paper}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Course ID</TableCell>
            <TableCell>Course Name</TableCell>
            <TableCell>Description</TableCell>
            <TableCell>Description 2</TableCell>
            {isStaff && <TableCell>Actions</TableCell>}
          </TableRow>
        </TableHead>
        <TableBody>
          {courses.map((course) => (
            <TableRow key={course.id}>
              <TableCell>{course.id}</TableCell>
              <TableCell>{course.name}</TableCell>
              <TableCell>{course.description}</TableCell>
              <TableCell>{course.description}</TableCell>
              {isStaff && (
                <TableCell>
                  <Button variant="contained" color="primary" size="small" style={{ marginRight: "5px" }}>
                    Edit
                  </Button>
                  <Button variant="contained" color="secondary" size="small">
                    Delete
                  </Button>
                </TableCell>
              )}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default CourseList;
