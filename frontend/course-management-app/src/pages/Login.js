import React from "react";
import CryptoJS from "crypto-js";
import { Container, Button, Box, Typography } from "@mui/material";

function Login() {
  const handleLogin = () => {
    const generateRandomString = (length) => {
      const charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
      let randomString = "";
      const randomValues = new Uint8Array(length);
      window.crypto.getRandomValues(randomValues);
      randomValues.forEach((value) => {
        randomString += charset[value % charset.length];
      });
      return randomString;
    };

    const generateCodeChallenge = (codeVerifier) => {
      return CryptoJS.SHA256(codeVerifier)
        .toString(CryptoJS.enc.Base64)
        .replace(/\+/g, "-")
        .replace(/\//g, "_")
        .replace(/=/g, "");
    };

    const codeVerifier = generateRandomString(64);
    const codeChallenge = generateCodeChallenge(codeVerifier);
    localStorage.setItem("code_verifier", codeVerifier);

    const authEndpoint = "http://localhost:5161/connect/authorize";
    const clientId = "frontend-app";
    const redirectUrl = encodeURIComponent("http://localhost:3000/callback");
    const scope = encodeURIComponent("openid profile offline_access course.manage");
    const state = crypto.randomUUID();

    const authUrl = `${authEndpoint}?response_type=code&client_id=${clientId}&redirect_uri=${redirectUrl}&scope=${scope}&state=${state}&code_challenge=${codeChallenge}&code_challenge_method=S256`;
    
    window.location.href = authUrl;
  };

  return (
    <Container>
      <Box
        display="flex"
        flexDirection="column"
        justifyContent="center"
        alignItems="center"
        height="100vh"
      >
        <Typography variant="h4" gutterBottom>
          Welcome to the Course Management System
        </Typography>
        <Typography variant="h5" gutterBottom>
          Please login with authorization server
        </Typography>
        <Button
          variant="contained"
          color="primary"
          size="large"
          onClick={handleLogin}
          sx={{
            padding: "12px 24px",
            borderRadius: "8px",
            textTransform: "none",
            fontSize: "18px",
            boxShadow: "0px 4px 10px rgba(0, 0, 0, 0.2)",
            "&:hover": {
              backgroundColor: "#1565c0",
            },
          }}
        >
          Login
        </Button>
      </Box>
    </Container>
  );
}

export default Login;
