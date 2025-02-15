import React from 'react';
import Navbar from '../components/Navbar';
import Dashboard from '../components/Dashboard';
import '../styles/global.css';

const DashboardPage = () => {
  const userRole = 'staff'; // Replace with actual user role
  return (
    <div className="dashboard-page">
      <Navbar />
      <Dashboard userRole={userRole} />
    </div>
  );
};

export default DashboardPage;