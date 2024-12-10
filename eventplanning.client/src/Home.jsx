import EventList from "./EventList.jsx";
import useFetch from "./useFetch.jsx";

const Home = () => {
  const { data: events, isPending, error } = useFetch('api/events');
  return (
    <div className="home">
      { error && <div>{ error }</div> }
      { isPending && <div>Loading...</div> }
      { events && <EventList events={events} /> }
    </div>
  );
}
 
export default Home;
