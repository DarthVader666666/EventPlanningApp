import { Link } from "react-router-dom";

const Navbar = ({name, role, closeSideBar}) => {
  return (
    <div onClick={closeSideBar} className="navbar">
      <Link to="/">Home</Link>
        {
          (role == 'Admin') &&
          <Link to="/create" className="create-button">New Event</Link>
        }
        <Link to="/login">{name ? name : "Log In"}</Link>
        <Link to="/register">Register</Link>
    </div>
  );
}
export default Navbar;