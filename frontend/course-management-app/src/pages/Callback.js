import React, { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import axios from "axios";
import qs from "qs";

function Callback() {
  const navigate = useNavigate();
  const location = useLocation();

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

      console.log("Authorization Code:", code); // Debugging

      try {
        const response = await axios.post(
          "http://localhost:5161/connect/token",
          qs.stringify({
            grant_type: "authorization_code",
            code,
            redirect_uri: "http://localhost:3000/callback",
            client_id: "mvc",
            client_secret: "mvc_secret",
            code_verifier: codeVerifier, // Include the code_verifier here
          }),
          {
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
          }
        );

        const { access_token, refresh_token } = response.data;

        // Store tokens securely
        localStorage.setItem("access_token", access_token);
        localStorage.setItem("refresh_token", refresh_token);

        console.log("Access Token:", access_token); // Debugging

        navigate("/courses"); // Redirect after successful login
      } catch (error) {
        if (error.response) {
          console.error("Error Response Data:", error.response.data);
          console.error("Status Code:", error.response.status);
        } else if (error.request) {
          console.error("No Response Received:", error.request);
        } else {
          console.error("Request Error:", error.message);
        }

        navigate("/"); // Redirect to login on error
      }
    };

    exchangeCodeForToken();
  }, [location, navigate]);

  return <div>Loading...</div>;
}

export default Callback;

