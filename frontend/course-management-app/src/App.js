import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { Box } from "@mui/material";
import HomePage from "./pages/HomePage";
import Callback from "./pages/Callback";
import ProtectedRoute from "./components/ProtectedRoute";
import Navbar from "./components/Navbar";
import Courses from "./pages/Courses";
import Classes from "./pages/Classes";
import Students from "./pages/Students";
import CourseStudent from "./pages/CourseStudent";
import ClassStudent from "./pages/ClassStudent";
import ClassCourse from "./pages/ClassCourse";
import CourseClass from "./pages/CourseClass";
import Unauthorized from "./pages/Unauthorized";
import Classmates from "./pages/Classmates";
import { AuthProvider } from "./Contexts/AuthContext";
import Login from "./pages/Login";

function App() {
  return (
  <AuthProvider>
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<Login />} />
        <Route path="/callback" element={<Callback />} />
        <Route path="/401" element={<Unauthorized />} />
        <Route element={<ProtectedRoute/>}>
          <Route path="/courses" element={<Courses />} />
          <Route path="/classes" element={<Classes />} />
          <Route path="/students" element={<Students />} />
          <Route path="/courses/:courseId/students" element={<CourseStudent />} />
          <Route path="/classes/:classId/students" element={<ClassStudent />} />
          <Route path="/classes/:classId/courses" element={<ClassCourse />} />
          <Route path="/courses/:courseId/classes" element={<CourseClass />} />
          <Route path="/classes/:classId/classmates" element={<Classmates />} />
        </Route>
        {/* Fallback Route (e.g., for 404 Not Found) */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Router>
  </AuthProvider>
  );
}

export default App;