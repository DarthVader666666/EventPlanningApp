import { Link } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

const Navbar = () => {
  const jwtToken = sessionStorage.getItem("access_token");

  if(jwtToken)
  {
    const token = jwtToken || jwtDecode(jwtToken);
    
    if (token.exp * 1000 < new Date().getTime()) {
      sessionStorage.clear();
    }
  }

  const name = sessionStorage.getItem("user_name");

  return (
    <nav className="navbar">
      <h1>The Best Event Planning App</h1>
      <h3>{import.meta.env.MODE}</h3>
      <h3>{import.meta.env.url}</h3>
      <div className="links">
        <Link to="/events">Home</Link>
        {
          name &&
          <Link to="events/create" className="create-button">New Event</Link>
        }
        <Link to="/login">{name ? name : "Log In"}</Link>
        <Link to="/register/">Register</Link>
      </div>
    </nav>
  );
}
 
export default Navbar;