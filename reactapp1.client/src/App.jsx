import Home from './Home.jsx';
import Navbar from './Navbar.jsx';
import LogIn from './Login.jsx';
//import Create from './Create.jsx';
//import Register from './Register.jsx';
//import EventDetails from './EventDetails.jsx';
//import Confirm from './Confirm.jsx';
import {
    BrowserRouter as Router,
    Route, Routes
  } from 'react-router-dom';

function App() {
    return (
        /*
        <Router>
            <div className="App">
                <Navbar/>
                <div className="content">
                    <Home/>
                </div>
            </div>    
        </Router>
        */

<Router>
<div className="App">
    <Navbar/>
    <div className="content">
        <Routes>
            <Route path="/" element={<Home/>}></Route>
            <Route path="/authorization/login" element={<LogIn/>}></Route>
        </Routes>
    </div>
</div>    
</Router> 
  );
}

export default App;
