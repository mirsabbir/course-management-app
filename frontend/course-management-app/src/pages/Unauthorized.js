import React from "react";
import { Container, Box, Typography, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";

function Unauthorized401() {
  const navigate = useNavigate();

  return (
    <Container>
      <Box
        display="flex"
        flexDirection="column"
        justifyContent="center"
        alignItems="center"
        height="100vh"
        textAlign="center"
      >
        <svg
          width="250"
          height="250"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <circle cx="12" cy="12" r="10" stroke="#FF9800" strokeWidth="2" fill="none" />
          <path
            d="M12 8v4m0 2h.01"
            stroke="#FF9800"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
        <Typography variant="h3" gutterBottom color="warning.main">
          401 - Access Denied
        </Typography>
        <Typography variant="h6" color="textSecondary" gutterBottom>
          You need proper authorization to view this page.
        </Typography>
        <Button
          variant="contained"
          color="warning"
          onClick={() => navigate("/")}
          sx={{ mt: 2 }}
        >
          Return Home
        </Button>
      </Box>
    </Container>
  );
}

export default Unauthorized401;