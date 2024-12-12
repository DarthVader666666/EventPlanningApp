import { Link } from "react-router-dom";

const Navbar = ({name, role}) => {
  return (
    <div className="navbar">
      <Link to="/">Home</Link>
        {
          (role == 'Admin' || role == 'User') &&
          <Link to="/create" className="create-button">New Event</Link>
        }
        <Link to="/login">{name ? name : "Log In"}</Link>
        <Link to="/register">Register</Link>
    </div>
  );
}
export default Navbar;