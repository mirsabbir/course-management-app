import { useState, useEffect, useCallback } from "react";
import {
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
  List,
  ListItem,
  ListItemText,
  IconButton,
  Snackbar,
  Alert,
} from "@mui/material";
import { Delete, Edit } from "@mui/icons-material";
import axios from "axios";
import { format } from "date-fns"; // For formatting dates

function Students() {
  const [open, setOpen] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [dateOfBirth, setDateOfBirth] = useState("");
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [studentToDelete, setStudentToDelete] = useState(null);
  const [editingStudent, setEditingStudent] = useState(null);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState(""); // New state for success messages

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
      });
      setStudents(response.data);
    } catch (error) {
      handleApiError(error);
      console.error("Error fetching students:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchStudents();
  }, [fetchStudents]);

  const handleAddOrUpdateStudent = async () => {
    try {
      const token = localStorage.getItem("access_token");
      if (editingStudent) {
        await axios.put(
          `http://localhost:5181/api/students/${editingStudent.id}`,
          { id: editingStudent.id, fullName, email, dateOfBirth },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Student updated successfully!"); // Success message for update
      } else {
        await axios.post(
          "http://localhost:5181/api/students",
          { fullName, email, dateOfBirth },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Student added successfully!"); // Success message for add
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
      setSuccessMessage("Student deleted successfully!"); // Success message for delete
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
    setDateOfBirth(format(new Date(student.dateOfBirth), "yyyy-MM-dd")); // Format date for input
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
        <List>
          {students.map((student) => (
            <ListItem key={student.id} secondaryAction={
              <>
                <IconButton edge="end" onClick={() => handleEditStudent(student)}>
                  <Edit color="primary" />
                </IconButton>
                <IconButton edge="end" onClick={() => {
                  setStudentToDelete(student.id);
                  setOpenDeleteDialog(true);
                }}>
                  <Delete color="error" />
                </IconButton>
              </>
            }>
              <ListItemText
                primary={student.fullName}
                secondary={
                  <>
                    <Typography variant="body2" color="text.secondary">
                      Email: {student.email}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Date of Birth: {format(new Date(student.dateOfBirth), "dd/MM/yyyy")}
                    </Typography>
                  </>
                }
              />
            </ListItem>
          ))}
        </List>
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
            onChange={(e) => setFullName(e.target.value)}
            style={{ marginBottom: "10px" }}
          />
          <TextField
            label="Email"
            fullWidth
            value={email}
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