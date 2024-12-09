import EventList from "./EventList.jsx";
import useFetch from "./useFetch.jsx";

const Home = () => {
  //const { error, isPending, data: events } = useFetch('api/events');

  return (
    <div className="home">
      {/* { error && <div>{ error }</div> }
      { isPending && <div>Loading...</div> }
      { events && <EventList events={(events)} /> } */}
    </div>
  );
}
 
export default Home;
