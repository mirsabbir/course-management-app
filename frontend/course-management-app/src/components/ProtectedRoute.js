import React, { useContext } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { AuthContext } from "../Contexts/AuthContext"; // Import the AuthContext
import CircularProgress from "@mui/material/CircularProgress"; // Import a loading spinner

const ProtectedRoute = () => {
  const { userName, loading } = useContext(AuthContext); // Check if the user is authenticated

  if (loading) {
    // Show a loading spinner while the authentication state is being initialized
    return <CircularProgress />;
  }

  // If authenticated, render the child components (the protected route)
  // If not authenticated, redirect to the 401 Unauthorized page
  console.log(userName); // Debugging
  return userName ? <Outlet /> : <Navigate to="/401" replace />;
};

export default ProtectedRoute;