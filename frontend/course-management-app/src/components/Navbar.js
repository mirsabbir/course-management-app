import React, { useContext, useState } from "react";
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
import { Menu as MenuIcon, Home, Book, Class, People, Close as CloseIcon, School } from "@mui/icons-material";
import { useNavigate, useLocation } from "react-router-dom";
import { AuthContext } from "../Contexts/AuthContext"; // Import the AuthContext

function Navbar() {
  const { userName, logout } = useContext(AuthContext); // Use AuthContext
  const [drawerOpen, setDrawerOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  // Sidebar menu items (only show if logged in)
  const menuItems = userName
    ? [
        { text: "Home", icon: <Home />, path: "/" },
        { text: "Courses", icon: <School />, path: "/courses" },
        { text: "Classes", icon: <Class />, path: "/classes" },
        { text: "Students", icon: <People />, path: "/students" },
      ]
    : [];

  return (
    <>
      <CssBaseline />
      <AppBar position="sticky">
        <Toolbar>
          {/* Hamburger Icon to Open Drawer (only show if logged in) */}
          {userName && (
            <IconButton edge="start" color="inherit" onClick={() => setDrawerOpen(true)} sx={{ mr: 2 }}>
              <MenuIcon />
            </IconButton>
          )}

          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            University Course Management
          </Typography>

          {userName ? (
            <>
              <Typography variant="body1" sx={{ mr: 2 }}>
                Welcome, {userName}
              </Typography>
              <Button color="inherit" onClick={logout}>
                Logout
              </Button>
            </>
          ) : (
            <Button color="inherit" onClick={() => navigate("/login")}>
              Login
            </Button>
          )}
        </Toolbar>
      </AppBar>

      {/* Closable Drawer (only show if logged in) */}
      {userName && (
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
                  key={item.text}
                  onClick={() => {
                    navigate(item.path);
                    setDrawerOpen(false); // Close drawer after navigation
                  }}
                  sx={{
                    cursor: "pointer",
                    backgroundColor: location.pathname === item.path ? "rgba(0, 0, 0, 0.1)" : "transparent",
                    "&:hover": { backgroundColor: "rgba(0, 0, 0, 0.2)" },
                  }}
                >
                  <ListItemIcon sx={{ color: location.pathname === item.path ? "primary.main" : "inherit" }}>
                    {item.icon}
                  </ListItemIcon>
                  <ListItemText
                    primary={item.text}
                    sx={{
                      color: location.pathname === item.path ? "primary.main" : "inherit",
                    }}
                  />
                </ListItem>
              ))}
            </List>
            <Divider />
          </Box>
        </Drawer>
      )}
    </>
  );
}

export default Navbar;