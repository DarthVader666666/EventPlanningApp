import Navbar from "./Navbar.jsx"
import { useState } from "react";
import { jwtDecode } from "jwt-decode";
import {slide as Menu} from "react-burger-menu"

const Title = () => {
  const jwtToken = sessionStorage.getItem("access_token");
  const [isOpen, setOpen] = useState(false);

  const handleIsOpen = () => {
    setOpen(!isOpen)
  }

  const closeSideBar = () => {
    setOpen(false)
  }

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

      <Menu isOpen={isOpen}
        onOpen={handleIsOpen}
        onClose={handleIsOpen} 
        noOverlay right width={'70%'} 
        customBurgerIcon={<img src='/menu-burger-icon_2.svg'/>}>
        <Navbar closeSideBar={closeSideBar} name={name} role={role}></Navbar>
      </Menu>

      <div className="visible-navbar">
        <Navbar name={name} role={role}></Navbar>
      </div>

    </div>
  );
}
export default Title;