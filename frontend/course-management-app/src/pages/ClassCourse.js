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
import { LinkOff } from "@mui/icons-material";

function ClassCourse() {
  const { classId } = useParams(); // Access the classId parameter
  const navigate = useNavigate();
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [openUnenrollDialog, setOpenUnenrollDialog] = useState(false);
  const [courseToUnenroll, setCourseToUnenroll] = useState(null); // Renamed variable
  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const [selectedCourse, setSelectedCourse] = useState(null);
  const location = useLocation();
  const { className } = location.state || {};

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

  const fetchCourses = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token"); // Get the access token
      const response = await axios.get(
        `http://localhost:5181/api/classes/${classId}/courses`,
        {
          headers: {
            Authorization: `Bearer ${token}`, // Include the Bearer token
          },
        }
      );
      setCourses(response.data); // Set the courses data
      setLoading(false); // Stop loading
    } catch (error) {
      console.error("Error fetching courses:", error);
      handleApiError(error);
      setLoading(false); // Stop loading
    }
  };

  useEffect(() => {
    fetchCourses();
  }, [classId]);

  const handleUnenrollCourse = async () => {
    try {
      const token = localStorage.getItem("access_token");
      await axios.delete(
        `http://localhost:5181/api/courses/${courseToUnenroll}/classes/${classId}`, // Use courseToUnenroll in the URL
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      setSuccessMessage("Course unenrolled successfully!");
      fetchCourses(); // Refresh the list
      setOpenUnenrollDialog(false); // Close the unenroll dialog
    } catch (error) {
      console.error("Error unenrolling course:", error);
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
        `http://localhost:5181/api/courses/search?query=${query}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setSearchResults(response.data); // Set the search results
    } catch (error) {
      console.error("Error searching courses:", error);
      handleApiError(error);
    }
  };

  const handleEnrollCourse = async () => {
    if (!selectedCourse) {
      setErrorMessage("Please select a course to enroll.");
      return;
    }
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.post(
        `http://localhost:5181/api/courses/${selectedCourse.id}/classes`,
        {
          courseId: selectedCourse.id,
          classId: classId,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setSuccessMessage("Course enrolled successfully!");
      fetchCourses(); // Refresh the list
      setSelectedCourse(null); // Clear the selected course
      setSearchQuery(""); // Clear the search query
    } catch (error) {
      console.error("Error enrolling course:", error);
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
        onClick={() => navigate("/classes")}
        style={{ marginBottom: "10px" }}
      >
        Back to Classes
      </Button>
      <Typography variant="h4" gutterBottom textAlign="center">
        Manage Courses for Class: {className}
      </Typography>

      {loading ? (
        <CircularProgress />
      ) : (
        <>
          <TableContainer component={Paper} style={{ padding: "15px" }} elevation={5}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell><strong>Name</strong></TableCell>
                  <TableCell><strong>Description</strong></TableCell>
                  <TableCell><strong>Added At</strong></TableCell>
                  <TableCell><strong>Added By</strong></TableCell>
                  <TableCell><strong>Actions</strong></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {courses.map((course) => (
                  <TableRow key={course.id}>
                    <TableCell>{course.name}</TableCell>
                    <TableCell>{course.description}</TableCell>
                    <TableCell>{formatCreatedAt(course.assignedAt)}</TableCell>
                    <TableCell>{course.assignedBy}</TableCell>
                    <TableCell>
                      <IconButton
                        edge="end"
                        onClick={() => {
                          setCourseToUnenroll(course.id); // Set the course ID to unenroll
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
              Enroll a Course
            </Typography>
            <Autocomplete
              options={searchResults}
              getOptionLabel={(option) => `${option.name} (${option.description})`}
              onInputChange={(event, newValue) => {
                setSearchQuery(newValue);
                handleSearch(newValue);
              }}
              onChange={(event, newValue) => setSelectedCourse(newValue)}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Search courses by name or description"
                  variant="outlined"
                  fullWidth
                />
              )}
            />
            <Button
              variant="contained"
              color="primary"
              onClick={handleEnrollCourse}
              style={{ marginTop: "10px" }}
            >
              Enroll Course
            </Button>
          </Box>
        </>
      )}

      {/* Unenroll Confirmation Dialog */}
      <Dialog open={openUnenrollDialog} onClose={() => setOpenUnenrollDialog(false)}>
        <DialogTitle>Unenroll Course</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to unenroll this course?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenUnenrollDialog(false)} color="primary">
            Cancel
          </Button>
          <Button onClick={handleUnenrollCourse} color="error">
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

export default ClassCourse;