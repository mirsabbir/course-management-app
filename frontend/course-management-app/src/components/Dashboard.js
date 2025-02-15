import React from 'react';

const Dashboard = ({ userRole }) => {
  return (
    <div className="dashboard">
      <h1>Dashboard</h1>
      {userRole === 'staff' && (
        <div>
          <h2>Staff Dashboard</h2>
          <p>Manage courses, classes, and students here.</p>
        </div>
      )}
      {userRole === 'student' && (
        <div>
          <h2>Student Dashboard</h2>
          <p>View your classes and courses here.</p>
        </div>
      )}
      {userRole === 'teacher' && (
        <div>
          <h2>Teacher Dashboard</h2>
          <p>Manage your classes and students here.</p>
        </div>
      )}
    </div>
  );
};

export default Dashboard;