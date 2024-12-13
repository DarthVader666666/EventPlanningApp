import Navbar from "./Navbar.jsx"
import { jwtDecode } from "jwt-decode";
import {slide as Menu} from "react-burger-menu"

const Title = () => {
  const jwtToken = sessionStorage.getItem("access_token");

  if(jwtToken)
  {
    const token = jwtToken || jwtDecode(jwtToken);
    
    if (token.exp * 1000 < new Date().getTime()) {
      sessionStorage.clear();
    }
  }

  const name = sessionStorage.getItem("user_name");
  const role = sessionStorage.getItem("role");

  return (
    <div className="title">
      <div>
        <h1 style={{padding: '2px'}}>The Best Event Planning App</h1>
        <h3 style={{padding: '3px'}}>{import.meta.env.MODE}</h3>
      </div>

      <Menu noOverlay right width={'70%'} customBurgerIcon={<img src='public/menu_burger_icon.svg'/>}>
        <Navbar name={name} role={role}></Navbar>
      </Menu>

      <div className="visible-navbar">
        <Navbar name={name} role={role}></Navbar>
      </div>

    </div>
  );
}
export default Title;