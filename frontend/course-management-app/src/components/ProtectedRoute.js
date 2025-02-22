import { Navigate } from "react-router-dom";

const ProtectedRoute = ({ children }) => {
  const { token } = "sdcfds";
  return token ? children : <Navigate to="/courses" />;
};

export default ProtectedRoute;
