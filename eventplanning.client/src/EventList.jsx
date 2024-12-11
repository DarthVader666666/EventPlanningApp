import { Link } from 'react-router-dom';

const EventList = ( events ) => {
  return (    
    <div className="event-list">
      { events.events.length == 0 ? <h1>Event List Is Empty</h1>
      : events.events.map((event, index) => (
        <div className="event-preview" key={index} >
          <Link to={`/${event.eventId}`}>
            <h2>{ event.title }</h2>
            <h3>Theme:</h3>
            <div>{ event.themeName }</div>
            <h3>Location:</h3>
            <div>{ event.location }</div>
            <h3>Date:</h3>
            <div>{ event.date }</div>
            <h3>Performers:</h3>
            <div>{ event.participants }</div>
          </Link>          
        </div>
      ))}      
    </div>
  );
}
 
export default EventList;