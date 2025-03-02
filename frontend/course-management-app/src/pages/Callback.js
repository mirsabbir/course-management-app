import React, { useEffect, useContext } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import axios from "axios";
import qs from "qs";
import { AuthContext } from "../Contexts/AuthContext"; // Import the AuthContext

function Callback() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useContext(AuthContext); // Use AuthContext

  useEffect(() => {
    const exchangeCodeForToken = async () => {
      const queryParams = new URLSearchParams(location.search);
      const code = queryParams.get("code");
      const state = queryParams.get("state");

      // Retrieve the code_verifier from localStorage or state
      const codeVerifier = localStorage.getItem("code_verifier");

      if (!code || !state || !codeVerifier) {
        console.error("Missing code, state, or code_verifier in callback URL");
        navigate("/"); // Redirect to login
        return;
      }

      try {
        const response = await axios.post(
          "http://localhost:5161/connect/token",
          qs.stringify({
            grant_type: "authorization_code",
            code,
            redirect_uri: "http://localhost:3000/callback",
            client_id: "frontend-app",
            code_verifier: codeVerifier, // Include the code_verifier here
          }),
          {
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
          }
        );

        const { access_token } = response.data;

        // Call the login function from AuthContext to update the state
        login(access_token);

        navigate("/"); // Redirect after successful login
      } catch (error) {
        console.error("Error during token exchange:", error);
        navigate("/"); // Redirect to login on error
      }
    };

    exchangeCodeForToken();
  }, [location, navigate, login]);

  return <div>Loading...</div>;
}

export default Callback;