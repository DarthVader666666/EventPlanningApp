import Home from './Home.jsx';
import Navbar from './Navbar.jsx';
import useFetch from "./useFetch.jsx";
//import { button } from "react-router-dom";
import { BrowserRouter } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
/*import Create from './Create.jsx';
import LogIn from './Login.jsx';
import Register from './Register.jsx';
import EventDetails from './EventDetails.jsx';
import Confirm from './Confirm.jsx'; */
//import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

function App() {
  //const basePath =  (import.meta.env.MODE === 'development' ? '' : 'https://event-planning-app.azurewebsites.net');
  const { error, isPending, data: events } = useFetch('events');
//   const jwtToken = sessionStorage.getItem("access_token");

//   if(jwtToken)
//   {
//     const token = jwtDecode(jwtToken);
    
//     if (token.exp * 1000 < new Date().getTime()) {
//       sessionStorage.clear();
//     }
//   }

  const name = sessionStorage.getItem("user_name");

  return (    
    <div className="home">
        <nav className="navbar">
            <h1>The Best Event Planning App</h1>
            <h3>{import.meta.env.MODE}</h3>
            <h3>{import.meta.env.url}</h3>
                <div className="links">
                    <button to="/">Home</button>
                    {
                      name &&
                      <button className="create-button">New Event</button>
                    }
                    <button>{name ? name : "Log In"}</button>
                    <button>Register</button>
                </div>
        </nav>
        { error && <div>{ error }</div> }
        { isPending && <div>Loading...</div> }
        {/* { events && <EventList events={(events)} /> } */}
    </div>
    
  );
}

export default App;
