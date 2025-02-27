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
  IconButton,
} from "@mui/material";
import { Menu as MenuIcon, Home, Book, Class, People, Close as CloseIcon } from "@mui/icons-material";
import { useNavigate, useLocation } from "react-router-dom";
import CryptoJS from "crypto-js";

function Navbar() {
  const [userName, setUserName] = useState(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

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

  // Handle login redirect (PKCE Flow)
  const handleLogin = () => {
    const codeVerifier = generateRandomString(64);
    const codeChallenge = generateCodeChallenge(codeVerifier);
    localStorage.setItem("code_verifier", codeVerifier);

    const authEndpoint = "http://localhost:5161/connect/authorize";
    const clientId = "frontend-app";
    const redirectUrl = encodeURIComponent("http://localhost:3000/callback");
    const scope = encodeURIComponent("openid profile offline_access course.manage");
    const state = "some_random_state_value";

    const authUrl = `${authEndpoint}?response_type=code&client_id=${clientId}&redirect_uri=${redirectUrl}&scope=${scope}&state=${state}&code_challenge=${codeChallenge}&code_challenge_method=S256`;
    
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
      <CssBaseline />
      <AppBar position="sticky">
        <Toolbar>
          {/* Hamburger Icon to Open Drawer */}
          <IconButton edge="start" color="inherit" onClick={() => setDrawerOpen(true)} sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>

          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            University Course Management
          </Typography>

          {userName ? (
            <>
              <Typography variant="body1" sx={{ mr: 2 }}>
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

      {/* Closable Drawer */}
      <Drawer
        anchor="left"
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
      >
        <Box sx={{ width: 240 }}>
          {/* Close Button */}
          <Box sx={{ display: "flex", justifyContent: "flex-end", p: 1 }}>
            <IconButton onClick={() => setDrawerOpen(false)}>
              <CloseIcon />
            </IconButton>
          </Box>

          <List>
            {menuItems.map((item) => (
              <ListItem
                button
                key={item.text}
                selected={location.pathname === item.path}
                onClick={() => {
                  navigate(item.path);
                  setDrawerOpen(false); // Close drawer after navigation
                }}
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
