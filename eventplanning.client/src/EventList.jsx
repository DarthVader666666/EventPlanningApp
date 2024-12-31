import { Link } from 'react-router-dom';

const EventList = ( events ) => {

  const token = sessionStorage.getItem("access_token");

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
      });
  }

  return (    
    <div className="event-list">
      { events.events.length == 0 ? <h1>Event List Is Empty</h1>
      : events.events.map((eventItem, index) => (
        <div className="event-preview" key={index} >
          <div style={{textAlign: 'right', height: '1rem'}}>
            <span onClick={() => onEventDelete(eventItem)} style={{fontSize: '2rem'}}>&times;</span>
          </div>
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