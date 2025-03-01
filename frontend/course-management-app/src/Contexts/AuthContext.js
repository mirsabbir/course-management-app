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
        const currentTime = Math.floor(Date.now() / 1000); // Current time in UTC seconds

        // Check if the token is expired (using UTC time)
        if (decoded.exp && decoded.exp > currentTime) {
          setUserName(decoded.fullName); // Set the fullName from the JWT token
          setUserRole(decoded.role); // Set the user role
          setUserId(decoded.userId); // Set the user ID
        } else {
          // Token is expired, clear user details
          setUserName(null);
          setUserRole("");
          setUserId("");
          localStorage.removeItem("access_token"); // Optionally remove the expired token
        }
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
    const logoutUrl = "http://localhost:5161/Account/Logout";
    window.location.href = logoutUrl;
  };

  return (
    <AuthContext.Provider value={{ userName, userRole, userId, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export { AuthContext, AuthProvider };