import React from "react";
import { List, ListItem, ListItemIcon, ListItemText, Divider } from "@mui/material";
import { Link } from "react-router-dom";
import SchoolIcon from '@mui/icons-material/School';
import PeopleIcon from "@mui/icons-material/People";
import BookIcon from "@mui/icons-material/Book";

function Sidebar() {
  return (
    <div style={{ width: "250px", height: "100vh", backgroundColor: "#f5f5f5" }}>
      <List>
        <ListItem button component={Link} to="/courses">
          <ListItemIcon>
            <SchoolIcon />
          </ListItemIcon>
          <ListItemText primary="Courses" />
        </ListItem>
        <ListItem button component={Link} to="/classes">
          <ListItemIcon>
            <BookIcon />
          </ListItemIcon>
          <ListItemText primary="Classes" />
        </ListItem>
        <ListItem button component={Link} to="/Students">
          <ListItemIcon>
            <PeopleIcon />
          </ListItemIcon>
          <ListItemText primary="Students" />
        </ListItem>
      </List>
      <Divider />
    </div>
  );
}

export default Sidebar;