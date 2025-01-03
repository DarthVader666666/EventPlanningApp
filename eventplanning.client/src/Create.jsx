import { useState } from "react";
import useFetch from "./useFetch.jsx";
import { useNavigate } from "react-router";

const Create = () => {
  const [title, setTitle] = useState('');
  const [themeId, setThemeId] = useState('');
  const [subThemeId, setSubThemeId] = useState('');
  const [date, setDate] = useState('');
  const [location, setLocation] = useState('');
  const { data: themes } = useFetch('api/themes');
  const [subThemes, setSubThemes] = useState();
  const [participants, setParticipants] = useState([]);
  const [dressCode, setDressCode] = useState('false');
  const [amountOfVacantPlaces, setAmountOfVacantPlaces] = useState(0);
  const [status, setStatus] = useState(200);
  const [isPending, setIsPending] = useState(false);
  const navigate = useNavigate();

  const token = sessionStorage.getItem("access_token");

  const handleSubmit = (e) => 
  {
    e.preventDefault();
    const newEvent = {title, themeId, subThemeId, date, dressCode, participants, location, amountOfVacantPlaces};
    setIsPending(true);
    
    fetch('api/events/create/', 
      {
        method: "POST",
        headers: 
        {
          "Content-Type": "application/json",
          "Authorization": "Bearer " + token
        },
        body: JSON.stringify(newEvent)
      }).then(response => {
        setStatus(response.status);
        setIsPending(false);
      });

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
    <div className="create">
      <h2>Create a New Event</h2>
      <form id="form" onSubmit={handleSubmit}>
        <label>Title</label>
        <input type="text" required value={title} onChange={(e) => setTitle(e.target.value)}></input>

        <label>Date</label>
        <input type="datetime-local" required value={date} onChange={(e) => setDate(e.target.value)}></input>

        <label>Location</label>
        <input type="text" required value={location} onChange={(e) => setLocation(e.target.value)}></input>

        <div className="dress-code">
          <span>Dress code</span>
          <input type="checkbox" value={dressCode} onChange={(e) => setDressCode(e.target.value)}></input>
        </div>

        <label>Vacant Places</label>
          <input type="number" value={amountOfVacantPlaces} onChange={(e) => setAmountOfVacantPlaces(e.target.value)}></input>

        <label>Theme</label>
        {
            themes &&
            <select value={themeId} onChange={(e) => onThemeChange(e.target.value)} >
                <option value=''></option>
                { themes.map((x, index) => 
                    { 
                        return <option value={x.themeId} key={index}>{x.themeName}</option>
                    })}
            </select>            
        }

        {
            subThemes &&
            <label>Sub Theme</label>
        }
        {            
            subThemes &&
            <select value={subThemeId} onChange={(e) => setSubThemeId(e.target.value)} >
                <option value=''></option>
                { subThemes.map((x, index) => 
                    { 
                        return <option value={x.subThemeId} key={index}>{x.subThemeName}</option>
                    })}
            </select>  
        }

        {
            (participants.length > 0) &&
            <label>Performers</label>
        }
        <div>
          {
            participants.map((participant, index) =>
              { 
                return <input value={participant} onChange={(e) => 
                    setParticipants((p) => p.map((x, i) => i === index ? e.target.value : x)) } 
                    key={index} type="text" ></input>
              })
          }
          {
            (participants.length > 0) &&
            <button className="add-participant" type="button" onClick={() => setParticipants([...participants, ''])}>Add</button>            
          }
        </div>
        <button form="form" disabled={isPending}>{isPending ? "Loading..." : "Create Event"}</button>
      </form>
    </div>
  );

  function onThemeChange(themeId)
  {  
    if(themeId !== '')
    {
        setThemeId(themeId);
        setSubThemes(themes.find(x => x.themeId === Number(themeId)).subThemes);
        setParticipants(['']);
    }
    else
    {
        setThemeId(null);
        setSubThemes(null);
        setParticipants([]);
    }
  }
}
 
export default Create;