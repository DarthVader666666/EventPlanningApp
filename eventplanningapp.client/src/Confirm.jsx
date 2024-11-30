import { useNavigate, useParams } from "react-router";
import useFetch from "./useFetch.jsx";

const Confirm = () => {
  const { userId } = useParams();  
  const { eventId } = useParams();
  const serverBaseUrl = import.meta.env.MODE === 'development' ? import.meta.env.VITE_BASE_URL : '';
  const navigate = useNavigate();

  const email = sessionStorage.getItem('user_name');
  const { status } = useFetch(`${serverBaseUrl}/events/confirm/${userId}/${eventId}`, "GET");

  return (
    <div className="confirm">
        {
            status === 200 ?
            <article>
              <h2>Thank You! { email }</h2>
              <p>Confirmation approved!</p>
            </article> :
            <h2>Something went wrong :(</h2>
        }
        {<button onClick={() => navigate("/")}>Home</button>}
    </div>
  );
}

export default Confirm;