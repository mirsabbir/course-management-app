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

  const fetchClasses = useCallback(async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token");
      const response = await axios.get("http://localhost:5181/api/classes", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setClasses(response.data);
    } catch (error) {
      handleApiError(error);
      console.error("Error fetching classes:", error);
    } finally {
      setLoading(false);
    }
  }, []);

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
        setSuccessMessage("Class updated successfully!"); // Success message for update
      } else {
        await axios.post(
          "http://localhost:5181/api/classes",
          { name: className, description: classDescription },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setSuccessMessage("Class added successfully!"); // Success message for add
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
      setSuccessMessage("Class deleted successfully!"); // Success message for delete
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
        <List>
          {classes.map((cls) => (
            <ListItem key={cls.id} secondaryAction={
              <>
                <IconButton edge="end" onClick={() => handleEditClass(cls)}>
                  <Edit color="primary" />
                </IconButton>
                <IconButton edge="end" onClick={() => {
                  setClassToDelete(cls.id);
                  setOpenDeleteDialog(true);
                }}>
                  <Delete color="error" />
                </IconButton>
              </>
            }>
              <ListItemText
                primary={cls.name}
                secondary={cls.description}
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