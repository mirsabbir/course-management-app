import { useState, useEffect, useCallback } from "react";
import {
  TableContainer,
  Paper,
  Container,
  Typography,
  Button,
  Modal,
  Box,
  TextField,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  CircularProgress,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  IconButton,
  Snackbar,
  Alert,
  Pagination,
} from "@mui/material";
import { Book, Delete, Edit, People } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import axios from "axios";

function Courses() {
  const [open, setOpen] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [courseName, setCourseName] = useState("");
  const [courseDescription, setCourseDescription] = useState("");
  const [courses, setCourses] = useState([]); // Initialize as an empty array
  const [loading, setLoading] = useState(true);
  const [courseToDelete, setCourseToDelete] = useState(null);
  const [editingCourse, setEditingCourse] = useState(null);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1); // Current page number
  const [pageSize, setPageSize] = useState(5); // Number of items per page
  const [totalPages, setTotalPages] = useState(1); // Total number of pages
  const [totalCount, setTotalCount] = useState(0); // Total number of items
  const navigate = useNavigate();

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

  const fetchCourses = useCallback(async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.get("http://localhost:5181/api/courses", {
        headers: { Authorization: `Bearer ${token}` },
        params: { pageNumber, pageSize }, // Add pagination query params
      });
      console.log("API Response:", response.data); // Log the API response
      setCourses(response.data.data || []); // Set courses from the `data` key
      setPageNumber(response.data.pageNumber); // Update current page number
      setPageSize(response.data.pageSize); // Update page size
      setTotalCount(response.data.totalCount); // Update total count
      setTotalPages(response.data.totalPages); // Update total pages
    } catch (error) {
      handleApiError(error);
      console.error("Error fetching courses:", error);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize]);

  useEffect(() => {
    fetchCourses();
  }, [fetchCourses]);

  const handleAddOrUpdateCourse = async () => {
    try {
      const token = localStorage.getItem("access_token");
      if (editingCourse) {
        await axios.put(
          `http://localhost:5181/api/courses/${editingCourse.id}`,
          { id: editingCourse.id, name: courseName, description: courseDescription },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Course updated successfully!");
      } else {
        await axios.post(
          "http://localhost:5181/api/courses",
          { name: courseName, description: courseDescription },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Course added successfully!");
      }
      fetchCourses();
      handleClose();
    } catch (error) {
      handleApiError(error);
      console.error("Error saving course:", error);
    }
  };

  const handleDeleteCourse = async () => {
    try {
      const token = localStorage.getItem("access_token");
      await axios.delete(`http://localhost:5181/api/courses/${courseToDelete}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      setSuccessMessage("Course deleted successfully!");
      fetchCourses();
      handleCloseDeleteDialog();
    } catch (error) {
      handleApiError(error);
      console.error("Error deleting course:", error);
    }
  };

  const handleEditCourse = (course) => {
    setEditingCourse(course);
    setCourseName(course.name);
    setCourseDescription(course.description);
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setEditingCourse(null);
    setCourseName("");
    setCourseDescription("");
  };

  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setCourseToDelete(null);
  };

  const handlePageChange = (event, newPage) => {
    setPageNumber(newPage); // Update page number when pagination is clicked
  };

  // Function to format createdAt in local time zone
  const formatCreatedAt = (createdAt) => {
    if (!createdAt) return "N/A"; // Handle undefined or null values
    const date = new Date(createdAt);
    return date.toLocaleString(); // Convert to local time zone
  };

  return (
    <Container style={{ marginTop: "20px" }}>
      <Typography variant="h4" gutterBottom textAlign="center">
        Manage Courses
      </Typography>
      <Button
        variant="contained"
        color="primary"
        onClick={() => setOpen(true)}
        style={{ marginBottom: "10px" }}
      >
        Add Course
      </Button>

      {loading ? (
        <CircularProgress />
      ) : (
        <TableContainer component={Paper} style={{ padding: "15px" }} elevation={5}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Name</strong></TableCell>
                <TableCell><strong>Description</strong></TableCell>
                <TableCell><strong>Created At</strong></TableCell>
                <TableCell><strong>Created By</strong></TableCell>
                <TableCell><strong>Manage Students</strong></TableCell>
                <TableCell><strong>Manage Classes</strong></TableCell>
                <TableCell><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {courses.map((course) => (
                <TableRow key={course.id}>
                  <TableCell>{course.name}</TableCell>
                  <TableCell>{course.description}</TableCell>
                  <TableCell>{formatCreatedAt(course.createdAt)}</TableCell>
                  <TableCell>{course.createdBy}</TableCell>
                  <TableCell>
                    <IconButton onClick={() => 
                                navigate(`/courses/${course.id}/students`, {
                                  state: { courseName: course.name },
                                })}>
                      <People color="primary"></People>
                    </IconButton>
                  </TableCell>
                  <TableCell>
                    <IconButton onClick={() => 
                                navigate(`/courses/${course.id}/classes`, {
                                  state: { courseName: course.name },
                                })}>
                      <Book color="primary"></Book>
                    </IconButton>
                  </TableCell>
                  <TableCell>
                    <IconButton edge="end" onClick={() => handleEditCourse(course)}>
                      <Edit color="primary" />
                    </IconButton>
                    <IconButton edge="end" onClick={() => {
                      setCourseToDelete(course.id);
                      setOpenDeleteDialog(true);
                    }}>
                      <Delete color="error" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>

          {/* Pagination Controls */}
          <Box display="flex" justifyContent="center" mt={2}>
            <Pagination
              count={totalPages} // Total number of pages
              page={pageNumber} // Current page number
              onChange={handlePageChange} // Handle page change
              color="primary"
            />
          </Box>
          </TableContainer>
      )}

      <Modal open={open} onClose={handleClose}>
        <Box sx={{
          position: "absolute",
          top: "50%",
          left: "50%",
          transform: "translate(-50%, -50%)",
          width: 400,
          bgcolor: "background.paper",
          boxShadow: 24,
          p: 4,
          borderRadius: "10px"
        }}>
          <Typography variant="h6" gutterBottom>
            {editingCourse ? "Edit Course" : "Add New Course"}
          </Typography>
          <TextField
            label="Course Name"
            fullWidth
            value={courseName}
            required="true"
            onChange={(e) => setCourseName(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <TextField
            label="Course Description"
            fullWidth
            multiline
            rows={4}
            value={courseDescription}
            onChange={(e) => setCourseDescription(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <Button
            variant="contained"
            color="primary"
            fullWidth
            onClick={handleAddOrUpdateCourse}
          >
            {editingCourse ? "Update Course" : "Add Course"}
          </Button>
        </Box>
      </Modal>

      <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
        <DialogTitle>Delete Course</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to delete this course?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDeleteDialog} color="primary">
            Cancel
          </Button>
          <Button onClick={handleDeleteCourse} color="error">
            Delete
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

export default Courses;