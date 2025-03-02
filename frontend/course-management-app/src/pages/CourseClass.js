import React, { useEffect, useState, useContext } from "react";
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
import { AuthContext } from "../Contexts/AuthContext"; // Import the AuthContext

function CourseClass() {
  const { courseId } = useParams(); // Access the courseId parameter
  const navigate = useNavigate();
  const [classes, setClasses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [openUnenrollDialog, setOpenUnenrollDialog] = useState(false);
  const [classToUnenroll, setClassToUnenroll] = useState(null); // Renamed variable
  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const [selectedClass, setSelectedClass] = useState(null);
  const location = useLocation();
  const { courseName } = location.state || {};

    // Use AuthContext to get userRole and userId
    const { userRole, userId } = useContext(AuthContext);

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

  const fetchClasses = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token"); // Get the access token
      const response = await axios.get(
        `http://localhost:5181/api/courses/${courseId}/classes`,
        {
          headers: {
            Authorization: `Bearer ${token}`, // Include the Bearer token
          },
        }
      );
      setClasses(response.data); // Set the classes data
      setLoading(false); // Stop loading
    } catch (error) {
      console.error("Error fetching classes:", error);
      handleApiError(error);
      setLoading(false); // Stop loading
    }
  };

  useEffect(() => {
    fetchClasses();
  }, [courseId]);

  const handleUnenrollClass = async () => {
    try {
      const token = localStorage.getItem("access_token");
      await axios.delete(
        `http://localhost:5181/api/courses/${courseId}/classes/${classToUnenroll}`, // Use classToUnenroll in the URL
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      setSuccessMessage("Class unenrolled successfully!");
      fetchClasses(); // Refresh the list
      setOpenUnenrollDialog(false); // Close the unenroll dialog
    } catch (error) {
      console.error("Error unenrolling class:", error);
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
        `http://localhost:5181/api/classes/search?query=${query}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setSearchResults(response.data); // Set the search results
    } catch (error) {
      console.error("Error searching classes:", error);
      handleApiError(error);
    }
  };

  const handleEnrollClass = async () => {
    if (!selectedClass) {
      setErrorMessage("Please select a class to enroll.");
      return;
    }
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.post(
        `http://localhost:5181/api/courses/${courseId}/classes`,
        {
          classId: selectedClass.id,
          courseId: courseId,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      setSuccessMessage("Class enrolled successfully!");
      fetchClasses(); // Refresh the list
      setSelectedClass(null); // Clear the selected class
      setSearchQuery(""); // Clear the search query
    } catch (error) {
      console.error("Error enrolling class:", error);
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
      
      {userRole === "Staff" && 
      <Typography variant="h4" gutterBottom textAlign="center">
        Manage Classes for Course: {courseName}
      </Typography>}

      {userRole === "Student" && 
      <Typography variant="h4" gutterBottom textAlign="center">
        View Classes for Course: {courseName}
      </Typography>}

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
                  {userRole === "Staff" && <TableCell><strong>Added By</strong></TableCell>}
                  {userRole === "Staff" && <TableCell><strong>Actions</strong></TableCell>}
                </TableRow>
              </TableHead>
              <TableBody>
                {classes.map((classItem) => (
                  <TableRow key={classItem.id}>
                    <TableCell>{classItem.name}</TableCell>
                    <TableCell>{classItem.description}</TableCell>
                    <TableCell>{formatCreatedAt(classItem.assignedAt)}</TableCell>
                    {userRole === "Staff" && <TableCell>{classItem.assignedBy}</TableCell>}
                    {userRole === "Staff" && <TableCell>
                      <IconButton
                        edge="end"
                        onClick={() => {
                          setClassToUnenroll(classItem.id); // Set the class ID to unenroll
                          setOpenUnenrollDialog(true); // Open the confirmation dialog
                        }}
                      >
                        <LinkOff color="error" />
                      </IconButton>
                    </TableCell>
                    }
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>

          {/* Search and Enroll Section */}
          { userRole === "Staff" &&
          <Box mt={4}>
            <Typography variant="h6" gutterBottom>
              Enroll a Class
            </Typography>
            <Autocomplete
              options={searchResults}
              getOptionLabel={(option) => `${option.name} (${option.description})`}
              onInputChange={(event, newValue) => {
                setSearchQuery(newValue);
                handleSearch(newValue);
              }}
              onChange={(event, newValue) => setSelectedClass(newValue)}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Search classes by name or description"
                  variant="outlined"
                  fullWidth
                />
              )}
            />
            <Button
              variant="contained"
              color="primary"
              onClick={handleEnrollClass}
              style={{ marginTop: "10px" }}
            >
              Enroll Class
            </Button>
          </Box>
          }
        </>
      )}

      
      {/* Unenroll Confirmation Dialog */}
      <Dialog open={openUnenrollDialog} onClose={() => setOpenUnenrollDialog(false)}>
        <DialogTitle>Unenroll Class</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to unenroll this class?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenUnenrollDialog(false)} color="primary">
            Cancel
          </Button>
          <Button onClick={handleUnenrollClass} color="error">
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

export default CourseClass;