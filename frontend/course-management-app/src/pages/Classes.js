import React, { useState, useEffect, useCallback, useContext } from "react";
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
import { Delete, Edit, Groups2, People, PeopleAlt, School } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { AuthContext } from "../Contexts/AuthContext"; // Adjust the import path

function Classes() {
  const [open, setOpen] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [className, setClassName] = useState("");
  const [classDescription, setClassDescription] = useState("");
  const [classes, setClasses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [classToDelete, setClassToDelete] = useState(null);
  const [editingClass, setEditingClass] = useState(null);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(7);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [studentId, setStudentId] = useState("");
  const navigate = useNavigate();

  // Use AuthContext to get user role and ID
  const { userRole, userId } = useContext(AuthContext);

  // Fetch student information if the user is a student
  useEffect(() => {
    if (userRole === "Student") {
      const fetchStudentInfo = async () => {
        try {
          const token = localStorage.getItem("access_token");
          const response = await axios.get("http://localhost:5181/api/students/me", {
            headers: { Authorization: `Bearer ${token}` },
          });
          setStudentId(response.data.id);
        } catch (error) {
          handleApiError(error);
          console.error("Error fetching student info:", error);
        }
      };
      fetchStudentInfo();
    }
  }, [userRole]);

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

  const fetchClasses = useCallback(async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token");
      let apiUrl;
      if (userRole === "Student") {
        apiUrl = `http://localhost:5181/api/students/${studentId}/classes`;
      } else {
        apiUrl = "http://localhost:5181/api/classes";
      }
      const response = await axios.get(apiUrl, {
        headers: { Authorization: `Bearer ${token}` },
        params: { pageNumber, pageSize },
      });
      console.log("API Response:", response.data);
      if(userRole === "Student"){
        setClasses(response.data || []);
      } else{
        setClasses(response.data.data || []);
      }
      setPageNumber(response.data.pageNumber);
      setPageSize(response.data.pageSize);
      setTotalCount(response.data.totalCount);
      setTotalPages(response.data.totalPages);
    } catch (error) {
      handleApiError(error);
      console.error("Error fetching classes:", error);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize, userRole, studentId]);

  useEffect(() => {
    if (userRole && (userRole !== "Student" || studentId)) {
      fetchClasses();
    }
  }, [fetchClasses, userRole, studentId]);

  const handleAddOrUpdateClass = async () => {
    try {
      const token = localStorage.getItem("access_token");
      if (editingClass) {
        await axios.put(
          `http://localhost:5181/api/classes/${editingClass.id}`,
          { id: editingClass.id, name: className, description: classDescription },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Class updated successfully!");
      } else {
        await axios.post(
          "http://localhost:5181/api/classes",
          { name: className, description: classDescription },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Class added successfully!");
      }
      fetchClasses();
      handleClose();
    } catch (error) {
      handleApiError(error);
      console.error("Error saving class:", error);
    }
  };

  const handleDeleteClass = async () => {
    try {
      const token = localStorage.getItem("access_token");
      await axios.delete(`http://localhost:5181/api/classes/${classToDelete}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      setSuccessMessage("Class deleted successfully!");
      fetchClasses();
      handleCloseDeleteDialog();
    } catch (error) {
      handleApiError(error);
      console.error("Error deleting class:", error);
    }
  };

  const handleEditClass = (cls) => {
    setEditingClass(cls);
    setClassName(cls.name);
    setClassDescription(cls.description);
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setEditingClass(null);
    setClassName("");
    setClassDescription("");
  };

  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setClassToDelete(null);
  };

  const handlePageChange = (event, newPage) => {
    setPageNumber(newPage);
  };

  const formatCreatedAt = (createdAt) => {
    if (!createdAt) return "N/A";
    const date = new Date(createdAt);
    return date.toLocaleString();
  };

  return (
    <Container style={{ marginTop: "20px" }}>
      <Typography variant="h4" gutterBottom textAlign="center">
        {userRole === "Student" ? "View All Classes" : "Manage Classes"}
      </Typography>
      {userRole === "Staff" && (
        <Button
          variant="contained"
          color="primary"
          onClick={() => setOpen(true)}
          style={{ marginBottom: "10px" }}
        >
          Add Class
        </Button>
      )}

      {loading ? (
        <CircularProgress />
      ) : (
        <TableContainer component={Paper} style={{ padding: "15px" }} elevation={5}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Name</strong></TableCell>
                <TableCell><strong>Description</strong></TableCell>
                {userRole === "Student" && <TableCell><strong>Course Name</strong></TableCell>}
                {userRole === "Staff" && <TableCell><strong>Created At</strong></TableCell>}
                {userRole === "Staff" && <TableCell><strong>Created By</strong></TableCell>}
                {userRole === "Staff" && <TableCell><strong>Manage Students</strong></TableCell>}
                {userRole === "Staff" && <TableCell><strong>Manage Courses</strong></TableCell>}
                {userRole === "Staff" && <TableCell><strong>Actions</strong></TableCell>}
                {userRole === "Student" && <TableCell><strong>Classmates</strong></TableCell>}
              </TableRow>
            </TableHead>
            <TableBody>
              {classes.map((cls) => (
                <TableRow key={cls.id}>
                  <TableCell>{cls.name}</TableCell>
                  <TableCell>{cls.description}</TableCell>
                  {userRole === "Student" && <TableCell>{cls.courseName}</TableCell>}
                  {userRole === "Staff" && <TableCell>{formatCreatedAt(cls.createdAt)}</TableCell>}
                  {userRole === "Staff" && <TableCell>{cls.createdBy}</TableCell>}
                  {userRole === "Staff" && (
                    <TableCell>
                      <IconButton onClick={() => 
                                  navigate(`/classes/${cls.id}/students`, {
                                    state: { className: cls.name },
                                  })}>
                        <People color="primary"></People>
                      </IconButton>
                    </TableCell>
                  )}
                  {userRole === "Staff" && (
                    <TableCell>
                      <IconButton onClick={() => 
                                  navigate(`/classes/${cls.id}/courses`, {
                                    state: { className: cls.name },
                                  })}>
                        <School color="primary"></School>
                      </IconButton>
                    </TableCell>
                  )}
                  {userRole === "Staff" && (
                    <TableCell>
                      <IconButton edge="end" onClick={() => handleEditClass(cls)}>
                        <Edit color="primary" />
                      </IconButton>
                      <IconButton edge="end" onClick={() => {
                        setClassToDelete(cls.id);
                        setOpenDeleteDialog(true);
                      }}>
                        <Delete color="error" />
                      </IconButton>
                    </TableCell>
                  )}
                  {userRole === "Student" && (
                    <TableCell>
                      <IconButton edge="end" onClick={() =>
                                  navigate(`/classes/${cls.id}/classmates`, {
                                    state: { className: cls.name, studentId: studentId, classId: cls.id },
                                  })}>
                        <Groups2 color="primary"></Groups2>
                      </IconButton>
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>

          {/* Pagination Controls */}
          <Box display="flex" justifyContent="center" mt={2}>
            <Pagination
              count={totalPages}
              page={pageNumber}
              onChange={handlePageChange}
              color="primary"
            />
          </Box>
          </TableContainer>
      )}

      {userRole === "Staff" && (
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
              {editingClass ? "Edit Class" : "Add New Class"}
            </Typography>
            <TextField
              label="Class Name"
              fullWidth
              value={className}
              required="true"
              onChange={(e) => setClassName(e.target.value)}
              style={{ marginBottom: "10px" }}
            />
            <TextField
              label="Class Description"
              fullWidth
              multiline
              rows={4}
              value={classDescription}
              onChange={(e) => setClassDescription(e.target.value)}
              style={{ marginBottom: "10px" }}
            />
            <Button
              variant="contained"
              color="primary"
              fullWidth
              onClick={handleAddOrUpdateClass}
            >
              {editingClass ? "Update Class" : "Add Class"}
            </Button>
          </Box>
        </Modal>
      )}

      {userRole === "Staff" && (
        <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
          <DialogTitle>Delete Class</DialogTitle>
          <DialogContent>
            <Typography>Are you sure you want to delete this class?</Typography>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDeleteDialog} color="primary">
              Cancel
            </Button>
            <Button onClick={handleDeleteClass} color="error">
              Delete
            </Button>
          </DialogActions>
        </Dialog>
      )}

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

export default Classes;