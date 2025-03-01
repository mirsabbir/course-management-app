import React, { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

const AuthContext = createContext();

const AuthProvider = ({ children }) => {
  const [userName, setUserName] = useState(null);
  const [userRole, setUserRole] = useState("");
  const [userId, setUserId] = useState("");
  const [loading, setLoading] = useState(true); // Add a loading state

  // Check if the user is authenticated on app load or refresh
  useEffect(() => {
    const token = localStorage.getItem("access_token");
    if (token) {
      try {
        const decoded = jwtDecode(token);
        console.log(decoded); // Debugging
        setUserName(decoded.fullName); // Set the fullName from the JWT token
        setUserRole(decoded.role); // Set the user role
        setUserId(decoded.userId); // Set the user ID
      } catch (error) {
        console.error("Error decoding JWT token:", error);
        setUserName(null);
        setUserRole("");
        setUserId("");
      }
    }
    setLoading(false); // Mark initialization as complete
  }, []);

  // Login function to set the access token and update the user details
  const login = (token) => {
    localStorage.setItem("access_token", token);
    const decoded = jwtDecode(token);
    setUserName(decoded.fullName);
    setUserRole(decoded.role);
    setUserId(decoded.userId);
  };

  // Logout function to clear the access token and reset the user details
  const logout = () => {
    localStorage.removeItem("access_token");
    setUserName(null);
    setUserRole("");
    setUserId("");
  };

  return (
    <AuthContext.Provider value={{ userName, userRole, userId, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export { AuthContext, AuthProvider };