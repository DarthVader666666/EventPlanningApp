import Home from './Home.jsx';
import Navbar from './Navbar.jsx';
/*import Create from './Create.jsx';
import LogIn from './Login.jsx';
import Register from './Register.jsx';
import EventDetails from './EventDetails.jsx';
import Confirm from './Confirm.jsx'; */
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

function App() {
  const basePath =  (import.meta.env.MODE === 'development' ? '' : 'https://event-planning-app.azurewebsites.net');

  return (
    <Router>
      <div className="App">
        <Navbar />
        <div className="content">
          <Routes>
            <Route path="/" element={<Home/>}></Route>
{/*         <Route path={basePath + "/events/create"} element={<Create/>}></Route>
            <Route path={basePath + "/login/"} element={<LogIn/>}></Route>
            <Route path={basePath + "/register/"} element={<Register/>}></Route>
            <Route path={basePath + "/events/:eventId"} element={<EventDetails />}></Route>
            <Route path={basePath + "/confirm/:userId/:eventId"} element={<Confirm />}></Route> */}
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
