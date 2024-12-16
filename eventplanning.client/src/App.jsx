import Home from './Home.jsx';
import Title from './Title.jsx';
import LogIn from './Login.jsx';
import Create from './Create.jsx';
import Register from './Register.jsx';
import EventDetails from './EventDetails.jsx';
import Confirm from './Confirm.jsx';
import {
    BrowserRouter as Router,
    Route, Routes
  } from 'react-router-dom';

function App() {
    return (
        <Router>
            <div className="App">
                <Title/>
                <div className="content">
                    <Routes>
                        <Route path="/" element={<Home/>}></Route>
                        <Route path="/:eventId" element={<EventDetails/>}></Route>
                        <Route path="/login" element={<LogIn/>}></Route>
                        <Route path="/register" element={<Register/>}></Route>
                        <Route path="/create" element={<Create/>}></Route>
                        <Route path="/confirm/:status" element={<Confirm/>}></Route>
                    </Routes>
                </div>
            </div>    
        </Router>
  );
}

export default App;
