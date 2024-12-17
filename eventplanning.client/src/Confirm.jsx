import { useNavigate, useSearchParams } from "react-router";
import { useState } from "react";

const Confirm = () => {
  const [searchParams] = useSearchParams();
  const [status, setState] = useState(400);
  const navigate = useNavigate();
  const email = sessionStorage.getItem('user_name');

  const handleConfirm = async () => {
    
    await fetch('api/authorization/login',
      {
          body: JSON.stringify({email: searchParams.get('email'), password: searchParams.get('password')} ),
          headers:
          {
              "Accept": "application/json",
              "Content-Type": "application/json",
          },
          method: "POST"
      })
      .then(async response => 
      {
        setState(response.status);
        const data = await response.json();
        sessionStorage.setItem("access_token", data.access_token);
        sessionStorage.setItem("user_name", data.user_name);
        sessionStorage.setItem("role", data.role);
      })
      
      navigate("/");
      navigate(0);
  }

  return (
    <div className="confirm">
        {
            searchParams.get('email') ?
            <article>
              <h2>Thank You! { email }</h2>
              <p>Confirmation approved!</p>
            </article> :
            <h2>Something went wrong :(</h2>
        }
        {<button onClick={handleConfirm}>Home</button>}
    </div>
  );
}

export default Confirm;