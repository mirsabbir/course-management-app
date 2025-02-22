import React from 'react';
import Banner from '../components/Banner';
import '../styles/homepage.css';

const HomePage = () => {
  return (
    <div className="homepage">
      <Banner />
      <div className="content">
        <h1>Welcome to the Course Management App</h1>
        <p>Manage courses, classes, and students with ease.</p>
      </div>
    </div>
  );
};

export default HomePage;