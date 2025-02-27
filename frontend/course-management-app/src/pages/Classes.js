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

function Classes() {
  const [open, setOpen] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [className, setClassName] = useState("");
  const [classDescription, setClassDescription] = useState("");
  const [classes, setClasses] = useState([]); // Initialize as an empty array
  const [loading, setLoading] = useState(true);
  const [classToDelete, setClassToDelete] = useState(null);
  const [editingClass, setEditingClass] = useState(null);
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

  const fetchClasses = useCallback(async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.get("http://localhost:5181/api/classes", {
        headers: { Authorization: `Bearer ${token}` },
        params: { pageNumber, pageSize }, // Add pagination query params
      });
      console.log("API Response:", response.data); // Log the API response
      setClasses(response.data.data || []); // Set classes from the `data` key
      setPageNumber(response.data.pageNumber); // Update current page number
      setPageSize(response.data.pageSize); // Update page size
      setTotalCount(response.data.totalCount); // Update total count
      setTotalPages(response.data.totalPages); // Update total pages
    } catch (error) {
      handleApiError(error);
      console.error("Error fetching classes:", error);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize]);

  useEffect(() => {
    fetchClasses();
  }, [fetchClasses]);

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
    setPageNumber(newPage); // Update page number when pagination is clicked
  };

  return (
    <Container style={{ marginTop: "20px" }}>
      <Typography variant="h4" gutterBottom textAlign="center">
        Manage Classes
      </Typography>
      <Button
        variant="contained"
        color="primary"
        onClick={() => setOpen(true)}
        style={{ marginBottom: "10px" }}
      >
        Add Class
      </Button>

      {loading ? (
        <CircularProgress />
      ) : (
        <>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Name</strong></TableCell>
                <TableCell><strong>Description</strong></TableCell>
                <TableCell><strong>Created At</strong></TableCell>
                <TableCell><strong>Created By</strong></TableCell>
                <TableCell><strong>Actions</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {classes.map((cls) => (
                <TableRow key={cls.id}>
                  <TableCell>{cls.name}</TableCell>
                  <TableCell>{cls.description}</TableCell>
                  <TableCell>{cls.createdAt}</TableCell>
                  <TableCell>{cls.createdBy}</TableCell>
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
        </>
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
            {editingClass ? "Edit Class" : "Add New Class"}
          </Typography>
          <TextField
            label="Class Name"
            fullWidth
            value={className}
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