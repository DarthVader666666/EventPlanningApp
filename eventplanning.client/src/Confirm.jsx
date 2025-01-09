import { useNavigate, useSearchParams } from "react-router";

const Confirm = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const email = searchParams.get("user_name");

  const handleConfirm = async () => {    
    const token = searchParams.get("access_token");

    await fetch('api/authorization/login',
      {
          body: JSON.stringify({email: email, password: null} ),
          headers:
          {
              "Accept": "application/json",
              "Content-Type": "application/json",
              "Authorization": "Bearer " + token
          },
          method: "POST"
      })
      .then(async response => 
      {
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
            searchParams.get('success') === 'true' 
            ?
            <article>
              <h2>Thank You, { email }!</h2>
              <p>Confirmation approved!</p>
            </article> 
            :
            <h2>{searchParams.get('message')}</h2>
        }
        {<button onClick={handleConfirm}>Home</button>}
    </div>
  );
}

export default Confirm;