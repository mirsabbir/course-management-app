import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
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
import StudentDashboard from "./pages/StudentDashboard";

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/callback" element={<Callback />} />
        <Route path="/courses" element={<Courses />} />
        <Route path="/classes" element={<Classes />} />
        <Route path="/students" element={<Students />} />
        <Route path="/courses/:courseId/students" element={<CourseStudent />} />
        <Route path="/classes/:classId/students" element={<ClassStudent />} />
        <Route path="/student" element={<ProtectedRoute><StudentDashboard /></ProtectedRoute>} />
      </Routes>
    </Router>
  );
}

export default App;