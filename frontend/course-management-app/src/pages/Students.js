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
import { Delete, Edit } from "@mui/icons-material";
import axios from "axios";

function Students() {
  const [open, setOpen] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [dateOfBirth, setDateOfBirth] = useState("");
  const [students, setStudents] = useState([]); // Initialize as an empty array
  const [loading, setLoading] = useState(true);
  const [studentToDelete, setStudentToDelete] = useState(null);
  const [editingStudent, setEditingStudent] = useState(null);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1); // Current page number
  const [pageSize, setPageSize] = useState(5); // Number of items per page
  const [totalPages, setTotalPages] = useState(1); // Total number of pages
  const [totalCount, setTotalCount] = useState(0); // Total number of items

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

  const fetchStudents = useCallback(async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.get("http://localhost:5181/api/students", {
        headers: { Authorization: `Bearer ${token}` },
        params: { pageNumber, pageSize }, // Add pagination query params
      });
      console.log("API Response:", response.data); // Log the API response
      setStudents(response.data.data || []); // Set students from the `data` key
      setPageNumber(response.data.pageNumber); // Update current page number
      setPageSize(response.data.pageSize); // Update page size
      setTotalCount(response.data.totalCount); // Update total count
      setTotalPages(response.data.totalPages); // Update total pages
    } catch (error) {
      handleApiError(error);
      console.error("Error fetching students:", error);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize]);

  useEffect(() => {
    fetchStudents();
  }, [fetchStudents]);

  const handleAddOrUpdateStudent = async () => {
    try {
      if (!fullName) {
        setErrorMessage("Full Name is required.");
        return; // Stop the function if validation fails
      }
      if (!email) {
        setErrorMessage("Email is required.");
        return; // Stop the function if validation fails
      }
      const token = localStorage.getItem("access_token");
      if (editingStudent) {
        await axios.put(
          `http://localhost:5181/api/students/${editingStudent.id}`,
          { id: editingStudent.id, fullName, email, dateOfBirth },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Student updated successfully!");
      } else {
        await axios.post(
          "http://localhost:5181/api/students",
          { fullName, email, dateOfBirth },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Student added successfully!");
      }
      fetchStudents();
      handleClose();
    } catch (error) {
      handleApiError(error);
      console.error("Error saving student:", error);
    }
  };

  const handleDeleteStudent = async () => {
    try {
      const token = localStorage.getItem("access_token");
      await axios.delete(`http://localhost:5181/api/students/${studentToDelete}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      setSuccessMessage("Student deleted successfully!");
      fetchStudents();
      handleCloseDeleteDialog();
    } catch (error) {
      handleApiError(error);
      console.error("Error deleting student:", error);
    }
  };

  const handleEditStudent = (student) => {
    setEditingStudent(student);
    setFullName(student.fullName);
    setEmail(student.email);
    setDateOfBirth(student.dateOfBirth);
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setEditingStudent(null);
    setFullName("");
    setEmail("");
    setDateOfBirth("");
  };

  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setStudentToDelete(null);
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
        Manage Students
      </Typography>
      <Button
        variant="contained"
        color="primary"
        onClick={() => setOpen(true)}
        style={{ marginBottom: "10px" }}
      >
        Add Student
      </Button>

      {loading ? (
        <CircularProgress />
      ) : (
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
                  <TableCell>{formatCreatedAt(student.dateOfBirth)}</TableCell>
                  <TableCell>{formatCreatedAt(student.createdAt)}</TableCell>
                  <TableCell>{student.createdBy}</TableCell>
                  <TableCell>
                    <IconButton edge="end" onClick={() => handleEditStudent(student)}>
                      <Edit color="primary" />
                    </IconButton>
                    <IconButton edge="end" onClick={() => {
                      setStudentToDelete(student.id);
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
            {editingStudent ? "Edit Student" : "Add New Student"}
          </Typography>
          <TextField
            label="Full Name"
            fullWidth
            value={fullName}
            required="true"
            onChange={(e) => setFullName(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <TextField
            label="Email"
            fullWidth
            value={email}
            required="true"
            disabled={editingStudent}
            onChange={(e) => setEmail(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <TextField
            label="Date of Birth"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={dateOfBirth}
            onChange={(e) => setDateOfBirth(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <Button
            variant="contained"
            color="primary"
            fullWidth
            onClick={handleAddOrUpdateStudent}
          >
            {editingStudent ? "Update Student" : "Add Student"}
          </Button>
        </Box>
      </Modal>

      <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
        <DialogTitle>Delete Student</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to delete this student?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDeleteDialog} color="primary">
            Cancel
          </Button>
          <Button onClick={handleDeleteStudent} color="error">
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

export default Students;