import React from "react";
import { Container, Typography, Grid2, Paper } from "@mui/material";
import Sidebar from "../components/Sidebar";

function StudentDashboard() {
  return (
    <div style={{ display: "flex" }}>
      <Sidebar />
      <Container style={{ marginLeft: "250px", marginTop: "20px" }}>
        <Typography variant="h4" gutterBottom>
          Student Dashboard
        </Typography>
        <Grid2 container spacing={3}>
          <Grid2 item xs={12}>
            <Paper style={{ padding: "20px" }}>
              <Typography variant="h6" gutterBottom>
                My Courses
              </Typography>
              {/* Add course list here */}
            </Paper>
          </Grid2>
        </Grid2>
      </Container>
    </div>
  );
}

export default StudentDashboard;