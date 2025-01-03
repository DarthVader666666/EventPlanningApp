import { Link } from 'react-router-dom';
import { useNavigate } from "react-router";
import { useState } from 'react';

const EventList = ( events ) => {

  const token = sessionStorage.getItem("access_token");
  const role = sessionStorage.getItem("role");
  const navigate = useNavigate();
  const [status, setStatus] = useState(200);

  const onEventDelete = function (eventItem) {
    fetch('api/events/remove', 
      {
        method: 'DELETE',
        body: JSON.stringify(eventItem),
        headers:
        {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "Authorization": "Bearer " + token
        },
      }).then(response => setStatus(response.status));

      if(status === 200) {
        navigate("/");
        navigate(0);
      }

      if(status === 401) {
        sessionStorage.clear();
        navigate("/login");
        navigate(0);
      }
  }

  return (    
    <div className="event-list">
      { events.events.length == 0 ? <h1>Event List Is Empty</h1>
      : events.events.map((eventItem, index) => (
        <div className="event-preview" key={index} >
          {
            role === "Admin" 
            ? <div style={{textAlign: 'end', height: '1rem'}}>
                <span onClick={() => onEventDelete(eventItem)}>&times;</span>
              </div>
            : <div></div>            
          }
          
          <div>
            <Link to={`/${eventItem.eventId}`}>
              <h2>{ eventItem.title }</h2>
              <h3>Theme:</h3>
              <div>{ eventItem.themeName }</div>
              <h3>Location:</h3>
              <div>{ eventItem.location }</div>
              <h3>Date:</h3>
              <div>{ eventItem.date }</div>
              <h3>Performers:</h3>
              <div>{ eventItem.participants }</div>
            </Link>          
          </div>          
        </div>
      ))}      
    </div>
  );
}
 
export default EventList;