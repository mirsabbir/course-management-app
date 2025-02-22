import React, { useState } from "react";
import { AppBar, Toolbar, Typography, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";
import CryptoJS from "crypto-js";

function Navbar() {
  const [userName, setUserName] = useState(null);
  const navigate = useNavigate();

  // Generate a random string for the code verifier
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

  // Generate the code challenge from the code verifier
  const generateCodeChallenge = (codeVerifier) => {
    return CryptoJS.SHA256(codeVerifier)
      .toString(CryptoJS.enc.Base64)
      .replace(/\+/g, "-")
      .replace(/\//g, "_")
      .replace(/=/g, "");
  };

  // Handle login redirect
  const handleLogin = () => {
    const codeVerifier = generateRandomString(64);
    const codeChallenge = generateCodeChallenge(codeVerifier);

    // Store the code verifier in localStorage
    localStorage.setItem("code_verifier", codeVerifier);

    // Redirect to the OAuth provider's authorization endpoint
    const authEndpoint = "http://localhost:5161/connect/authorize";
    const clientId = "mvc";
    const redirectUrl = encodeURIComponent("http://localhost:3000/callback");
    const scope = encodeURIComponent("openid profile api1");
    const state = "some_random_state_value"; // For CSRF protection

    const authUrl = `${authEndpoint}?response_type=code&client_id=${clientId}&redirect_uri=${redirectUrl}&scope=${scope}&state=${state}&code_challenge=${codeChallenge}&code_challenge_method=S256`;

    // Redirect the user to the OAuth provider
    window.location.href = authUrl;
  };

  // Handle logout
  const handleLogout = () => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    localStorage.removeItem("code_verifier");
    setUserName(null);
    navigate("/");
  };


  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" style={{ flexGrow: 1 }}>
          University Course Management
        </Typography>
        {userName ? (
          <>
            <Typography variant="body1" style={{ marginRight: "20px" }}>
              Welcome, {userName}
            </Typography>
            <Button color="inherit" onClick={handleLogout}>
              Logout
            </Button>
          </>
        ) : (
          <Button color="inherit" onClick={handleLogin}>
            Login
          </Button>
        )}
      </Toolbar>
    </AppBar>
  );
}

export default Navbar;