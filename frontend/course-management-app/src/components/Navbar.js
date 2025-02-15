import React from 'react';
import { Link } from 'react-router-dom';

const Navbar = () => {
  const isLoggedIn = false; // Replace with actual login state
  return (
    <nav className="navbar">
      <div className="navbar-brand">Course Management</div>
      <div className="navbar-actions">
        {isLoggedIn ? (
          <Link to="/dashboard" className="btn">Dashboard</Link>
        ) : (
          <Link to="/login" className="btn">Login</Link>
        )}
      </div>
    </nav>
  );
};

export default Navbar;