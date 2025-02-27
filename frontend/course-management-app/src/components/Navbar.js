import React, { useState } from "react";
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  CssBaseline,
  Box,
} from "@mui/material";
import { useNavigate, useLocation } from "react-router-dom";
import { Home, Book, Class, People } from "@mui/icons-material"; // Icons for menu options
import CryptoJS from "crypto-js";

function Navbar() {
  const [userName, setUserName] = useState(null);
  const navigate = useNavigate();
  const location = useLocation(); // Get current route location

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
    const clientId = "frontend-app";
    const redirectUrl = encodeURIComponent("http://localhost:3000/callback");
    const scope = encodeURIComponent("openid profile offline_access course.manage");
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

  // Sidebar menu items
  const menuItems = [
    { text: "Home", icon: <Home />, path: "/" },
    { text: "Courses", icon: <Book />, path: "/courses" },
    { text: "Classes", icon: <Class />, path: "/classes" },
    { text: "Students", icon: <People />, path: "/students" },
  ];

  return (
    <>
      <CssBaseline /> {/* Normalize CSS */}
      <AppBar position="sticky" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
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

      {/* Sidebar */}
      <Drawer
        variant="permanent"
        sx={{
          width: 240,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: { width: 240, boxSizing: "border-box" },
        }}
      >
        <Toolbar /> {/* Add space for the AppBar */}
        <Box sx={{ overflow: "auto" }}>
          <List>
            {menuItems.map((item) => (
              <ListItem
                button
                key={item.text}
                selected={location.pathname === item.path} // Highlight active menu item
                onClick={() => navigate(item.path)}
              >
                <ListItemIcon>{item.icon}</ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItem>
            ))}
          </List>
          <Divider />
        </Box>
      </Drawer>
    </>
  );
}

export default Navbar;