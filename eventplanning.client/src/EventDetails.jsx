import { useNavigate, useParams } from "react-router";
import useFetch from "./useFetch.jsx";
import { useState } from "react";

const EventDetails = () => {
  const { eventId } = useParams();
  const { isPending: pending, data: event } = useFetch('api/events/' + eventId);
  const [sending, setSending] = useState(false);
  const [message, setMessage] = useState('');
  const navigate = useNavigate();
  const email = sessionStorage.getItem('user_name');

  const handleParticipate = async () =>
  {
    const token = sessionStorage.getItem('access_token');

    if(email === null) {
        navigate("/login");
        return;
    }

    setSending(true);

    const response = await fetch('api/events/participate/', 
    {
      method: "POST",
      headers: 
        {
          "Content-Type": "application/json",
          "Authorization": "Bearer " + token
        },
      body: JSON.stringify({ eventId, email })
    }).then(response => response);

    setSending(false);

    if(response.status === 200)
    {
      const body = await response.json();
      setMessage(body.message);
      
      if(body.message == 'Email sent')
      {
        alert('Confirmation link sent. Please, check your email!');
        navigate("/");
        navigate(0);
      }
    }
    else
    {
      alert('Something went wrong!');
      navigate("/");
      navigate(0);
    }
  }

  return (
    <div className="event-details">
      {
        sending && (
          <div>
            <h3>Sending confirmation email <div className="loading">↻</div></h3><span>{email}</span>
          </div>
        )
      }

      {
        message == '' || message == 'Email sent' ? 
        (
          event && Number(event.amountOfVacantPlaces) > 0 ? 
          (
            <div>
              <h2>{ event.title }</h2>
                <h3>Theme:</h3>
                <div>{ event.themeName }</div>
                <h3>Sub theme:</h3>
                <div>{ event.subThemeName }</div>
                <h3>Location:</h3>
                <div>{ event.location }</div>
                <h3>Date:</h3>
                <div>{ event.date }</div>
                <h3>Performers:</h3>
                <div>{ event.participants }</div>
                <h3>Vacant places:</h3>
                <div>{ event.amountOfVacantPlaces }</div>
                { <button onClick={() => handleParticipate()}>Participate</button> }
            </div>
          ) :
          (
            !pending && <h1>Sorry, but event capacity is full : (</h1>
          )
        ) : 
        <div>
          <h1>{message}</h1>
          <button onClick={() => { setMessage(''); navigate("/");}}>Home</button>
        </div>
      }
    </div>
  );
}

export default EventDetails;