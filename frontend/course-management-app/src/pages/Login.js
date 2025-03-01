import React, {useEffect, useState} from "react";
import CryptoJS from "crypto-js";
import { Container, Button, Box, Typography } from "@mui/material";



function Login() {

  useEffect(() => {
    const timer = setTimeout(() => {
      handleLogin();
    }, 500);
  
    return () => clearTimeout(timer); // Cleanup the timer on component unmount
  }, []);

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
        Redirecting to authorization server...
        </Typography>
      </Box>
    </Container>
    
  );
}

export default Login;
