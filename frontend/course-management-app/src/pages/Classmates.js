import { useState, useEffect, useCallback } from "react";
import {
  TableContainer,
  Paper,
  Container,
  Typography,
  CircularProgress,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  Snackbar,
  Alert,
  Pagination,
  Box,
} from "@mui/material";
import axios from "axios";
import { useLocation } from "react-router-dom"; // Import useLocation to access route state

function Classmates() {
  const [classmates, setClassmates] = useState([]); // Initialize as an empty array
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [pageNumber, setPageNumber] = useState(1); // Current page number
  const [pageSize, setPageSize] = useState(5); // Number of items per page
  const [totalPages, setTotalPages] = useState(1); // Total number of pages
  const [totalCount, setTotalCount] = useState(0); // Total number of items

  // Use useLocation to access the state passed via navigate
  const location = useLocation();
  const { studentId, classId } = location.state || {}; // Destructure studentId and classId from state

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

  const fetchClassmates = useCallback(async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem("access_token");
      
      const response = await axios.get(
        `http://localhost:5181/api/students/${studentId}/classes/${classId}/classmates`,
        {
          headers: { Authorization: `Bearer ${token}` },
          params: { pageNumber, pageSize }, // Add pagination query params
        }
      );
      console.log("API Response:", response.data); // Log the API response
      setClassmates(response.data || []); // Set classmates from the `data` key
      setPageNumber(response.data.pageNumber); // Update current page number
      setPageSize(response.data.pageSize); // Update page size
      setTotalCount(response.data.totalCount); // Update total count
      setTotalPages(response.data.totalPages); // Update total pages
    } catch (error) {
      handleApiError(error);
      console.error("Error fetching classmates:", error);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize, studentId, classId]);

  useEffect(() => {
    console.log(studentId);
    console.log(classId);
    if (studentId && classId) {
      fetchClassmates();
    }
  }, [fetchClassmates, studentId, classId]);

  const handlePageChange = (event, newPage) => {
    setPageNumber(newPage); // Update page number when pagination is clicked
  };

  return (
    <Container style={{ marginTop: "20px" }}>
      <Typography variant="h4" gutterBottom textAlign="center">
        Classmates
      </Typography>

      {loading ? (
        <CircularProgress />
      ) : (
        <TableContainer component={Paper} style={{ padding: "15px" }} elevation={5}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Full Name</strong></TableCell>
                <TableCell><strong>Email</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {classmates.map((classmate) => (
                <TableRow key={classmate.id}>
                  <TableCell>{classmate.fullName}</TableCell>
                  <TableCell>{classmate.email}</TableCell>
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
    </Container>
  );
}

export default Classmates;