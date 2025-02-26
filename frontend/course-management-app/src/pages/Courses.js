import { useState, useEffect } from "react";
import { Container, Typography, Grid2, Paper, Button, Modal, Box, TextField, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";
import Sidebar from "../components/Sidebar";
import CourseList from "../components/CourseList";
import axios from "axios";

function Courses() {
  const [open, setOpen] = useState(false); // For Add Course Modal
  const [courseName, setCourseName] = useState("");
  const [courseDescription, setCourseDescription] = useState("");

  const [openDeleteDialog, setOpenDeleteDialog] = useState(false); // For Delete Confirmation Dialog
  const [courseToDelete, setCourseToDelete] = useState(null); // Store course to delete

  const [courses, setCourses] = useState([]); // To store fetched courses
  const [loading, setLoading] = useState(true); // Loading state for fetching courses

  const isStudent = false;
  const isStaff = true;

  // Open Add Course Modal
  const handleOpen = () => setOpen(true);

  // Close Add Course Modal
  const handleClose = () => setOpen(false);

  // Handle course form submission (mocking API call here)
  const handleAddCourse = async () => {
    try {
      const response = await axios.post("http://localhost:5181/api/courses", {
        name: courseName,
        description: courseDescription,
      });
      setCourseName("");
      setCourseDescription("");
      handleClose();
      fetchCourses(); // Re-fetch courses after adding a new one
    } catch (error) {
      console.error("Error adding course:", error);
    }
  };

  // Open Delete Confirmation Dialog
  const handleOpenDeleteDialog = (courseId) => {
    setCourseToDelete(courseId);
    setOpenDeleteDialog(true);
  };

  // Close Delete Confirmation Dialog
  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setCourseToDelete(null);
  };

  // Handle delete confirmation
  const handleDeleteCourse = async () => {
    try {
      await axios.delete(`/api/courses/${courseToDelete}`);
      fetchCourses(); // Re-fetch the course list after deletion
      handleCloseDeleteDialog(); // Close dialog after deletion
    } catch (error) {
      console.error("Error deleting course:", error);
    }
  };

  // Fetch courses from the backend
  const fetchCourses = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token"); // Retrieve the token
      const response = await axios.get("http://localhost:5181/api/courses", {
        headers: {
          Authorization: `Bearer ${token}`, // Add Bearer token
        },
      });
      setCourses(response.data);
    } catch (error) {
      console.error("Error fetching courses:", error);
    } finally {
      setLoading(false);
    }
  };
  

  useEffect(() => {
    fetchCourses(); // Fetch courses when component mounts
  }, []);

  return (
    <div style={{ display: "flex" }}>
      <Sidebar />
      <Container style={{ marginLeft: "20px", marginTop: "20px" }}>
        <Typography variant="h4" gutterBottom>
          {isStudent ? "My Courses" : "Staff Dashboard"}
        </Typography>
        <Grid2 container spacing={3}>
          <Grid2 item xs={12}>
            <Paper style={{ padding: "20px" }}>
              <Typography variant="h6" gutterBottom>
                {isStudent ? "Enrolled Courses" : "Manage Courses"}
              </Typography>
              {isStaff && (
                <Button variant="contained" color="primary" style={{ marginBottom: "10px" }} onClick={handleOpen}>
                  Add Course
                </Button>
              )}
              <CourseList 
                courses={courses} 
                isStaff={isStaff} 
                onDelete={handleOpenDeleteDialog} 
                loading={loading}
              />
            </Paper>
          </Grid2>
        </Grid2>
      </Container>

      {/* Add Course Modal */}
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="add-course-modal"
        aria-describedby="add-new-course-form"
      >
        <Box
          sx={{
            position: "absolute",
            top: "50%",
            left: "50%",
            transform: "translate(-50%, -50%)",
            width: 400,
            bgcolor: "background.paper",
            boxShadow: 24,
            p: 4,
          }}
        >
          <Typography variant="h6" component="h2" gutterBottom>
            Add New Course
          </Typography>
          <TextField
            label="Course Name"
            variant="outlined"
            fullWidth
            value={courseName}
            onChange={(e) => setCourseName(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <TextField
            label="Course Description"
            variant="outlined"
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
            onClick={handleAddCourse}
            fullWidth
          >
            Add Course
          </Button>
        </Box>
      </Modal>

      {/* Delete Confirmation Dialog */}
      <Dialog
        open={openDeleteDialog}
        onClose={handleCloseDeleteDialog}
      >
        <DialogTitle>Delete Course</DialogTitle>
        <DialogContent>
          <Typography variant="body1">
            Are you sure you want to delete this course?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDeleteDialog} color="primary">
            Cancel
          </Button>
          <Button onClick={handleDeleteCourse} color="primary">
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}

export default Courses;
