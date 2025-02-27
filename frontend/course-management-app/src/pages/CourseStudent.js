import React, { useEffect, useState } from "react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import axios from "axios";
import {
  Container,
  Typography,
  Button,
  TableContainer,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  Paper,
  CircularProgress,
  IconButton,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Snackbar,
  Alert,
  TextField,
  Autocomplete,
  Box,
} from "@mui/material";
import { People, Edit, Delete, LinkOff } from "@mui/icons-material";

function CourseStudent() {
  const { courseId } = useParams(); // Access the courseId parameter
  const navigate = useNavigate();
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [openUnenrollDialog, setOpenUnenrollDialog] = useState(false);
  const [studentToUnenroll, setStudentToUnenroll] = useState(null); // Renamed variable
  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const [selectedStudent, setSelectedStudent] = useState(null);
  const location = useLocation();
  const { courseName } = location.state || {};

  const handleApiError = (error) => {
    if (error.response) {
      const { data, status } = error.response;

      if (data.errors) {
        const messages = Object.values(data.errors).flat().join("\n");
        setErrorMessage(messages);
      } else if (data.message) {
        setErrorMessage(data.message);
      } else {
        setErrorMessage(`Error: ${status}`);
      }
    } else {
      setErrorMessage("An unexpected error occurred.");
    }
  };

  const fetchStudents = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token"); // Get the access token
      const response = await axios.get(
        `http://localhost:5181/api/courses/${courseId}/students`,
        {
          headers: {
            Authorization: `Bearer ${token}`, // Include the Bearer token
          },
        }
      );
      setStudents(response.data); // Set the students data
      setLoading(false); // Stop loading
    } catch (error) {
      console.error("Error fetching students:", error);
      handleApiError(error);
      setLoading(false); // Stop loading
    }
  };

  useEffect(() => {
    fetchStudents();
  }, [courseId]);

  const handleUnenrollStudent = async () => {
    try {
      const token = localStorage.getItem("access_token");
      await axios.delete(
        `http://localhost:5181/api/courses/${courseId}/students/${studentToUnenroll}`, // Use studentToUnenroll in the URL
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      setSuccessMessage("Student unenrolled successfully!");
      fetchStudents(); // Refresh the list
      setOpenUnenrollDialog(false); // Close the unenroll dialog
    } catch (error) {
      console.error("Error unenrolling student:", error);
      handleApiError(error);
    }
  };

  const handleSearch = async (query) => {
    if (!query) {
      setSearchResults([]);
      return;
    }
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.get(
        `http://localhost:5181/api/students/search?query=${query}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setSearchResults(response.data); // Set the search results
    } catch (error) {
      console.error("Error searching students:", error);
      handleApiError(error);
    }
  };

  const handleEnrollStudent = async () => {
    if (!selectedStudent) {
      setErrorMessage("Please select a student to enroll.");
      return;
    }
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.post(
        `http://localhost:5181/api/courses/${courseId}/students`,
        {
          studentId: selectedStudent.id,
          courseId: courseId,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setSuccessMessage("Student enrolled successfully!");
      fetchStudents(); // Refresh the list
      setSelectedStudent(null); // Clear the selected student
      setSearchQuery(""); // Clear the search query
    } catch (error) {
      console.error("Error enrolling student:", error);
      handleApiError(error);
    }
  };

  const formatCreatedAt = (createdAt) => {
    if (!createdAt) return "N/A";
    const date = new Date(createdAt);
    return date.toLocaleString(); // Convert to local time zone
  };

  return (
    <Container style={{ marginTop: "20px" }}>
      <Button
        variant="contained"
        color="primary"
        onClick={() => navigate("/courses")}
        style={{ marginBottom: "10px" }}
      >
        Back to Courses
      </Button>
      <Typography variant="h4" gutterBottom textAlign="center">
        Manage Students for Course Name: {courseName}
      </Typography>

      {loading ? (
        <CircularProgress />
      ) : (
        <>
          <TableContainer component={Paper} style={{ padding: "15px" }} elevation={5}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell><strong>Full Name</strong></TableCell>
                  <TableCell><strong>Email</strong></TableCell>
                  <TableCell><strong>Date of Birth</strong></TableCell>
                  <TableCell><strong>Created At</strong></TableCell>
                  <TableCell><strong>Created By</strong></TableCell>
                  <TableCell><strong>Actions</strong></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {students.map((student) => (
                  <TableRow key={student.id}>
                    <TableCell>{student.fullName}</TableCell>
                    <TableCell>{student.email}</TableCell>
                    <TableCell>{student.dateOfBirth}</TableCell>
                    <TableCell>{formatCreatedAt(student.createdAt)}</TableCell>
                    <TableCell>{student.createdBy}</TableCell>
                    <TableCell>
                      <IconButton
                        edge="end"
                        onClick={() => {
                          setStudentToUnenroll(student.id); // Set the student ID to unenroll
                          setOpenUnenrollDialog(true); // Open the confirmation dialog
                        }}
                      >
                        <LinkOff color="error" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>

          {/* Search and Enroll Section */}
          <Box mt={4}>
            <Typography variant="h6" gutterBottom>
              Enroll a Student
            </Typography>
            <Autocomplete
              options={searchResults}
              getOptionLabel={(option) => `${option.fullName} (${option.email})`}
              onInputChange={(event, newValue) => {
                setSearchQuery(newValue);
                handleSearch(newValue);
              }}
              onChange={(event, newValue) => setSelectedStudent(newValue)}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Search students by name or email"
                  variant="outlined"
                  fullWidth
                />
              )}
            />
            <Button
              variant="contained"
              color="primary"
              onClick={handleEnrollStudent}
              style={{ marginTop: "10px" }}
            >
              Enroll Student
            </Button>
          </Box>
        </>
      )}

      {/* Unenroll Confirmation Dialog */}
      <Dialog open={openUnenrollDialog} onClose={() => setOpenUnenrollDialog(false)}>
        <DialogTitle>Unenroll Student</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to unenroll this student?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenUnenrollDialog(false)} color="primary">
            Cancel
          </Button>
          <Button onClick={handleUnenrollStudent} color="error">
            Unenroll
          </Button>
        </DialogActions>
      </Dialog>

      {/* Error Snackbar */}
      <Snackbar
        open={!!errorMessage}
        autoHideDuration={6000}
        onClose={() => setErrorMessage("")}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert severity="error" onClose={() => setErrorMessage("")}>
          {errorMessage}
        </Alert>
      </Snackbar>

      {/* Success Snackbar */}
      <Snackbar
        open={!!successMessage}
        autoHideDuration={6000}
        onClose={() => setSuccessMessage("")}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert severity="success" onClose={() => setSuccessMessage("")}>
          {successMessage}
        </Alert>
      </Snackbar>
    </Container>
  );
}

export default CourseStudent;