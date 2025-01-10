import { useState } from "react";
import { useNavigate } from "react-router";

const Register = () => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [sending, setSending] = useState(false);
    
    const navigate = useNavigate();
    
    const handleSubmit = async (e) =>
    {
        e.preventDefault();

        if(password !== confirmPassword)
        {
            alert("Passwords do not match");
            return;
        }

        sessionStorage.clear();

        setSending(true);
            
        const response = await fetch('api/authorization/register/',
            {
                body: JSON.stringify({ email, firstName, lastName, password }),
                headers:
                {
                  "Content-Type": "application/json"
                },
                method: "POST"
            });

            setSending(false);

            if(response.status === 200)
            {
              const body = await response.json();
            
              if(body.message == 'Email sent')
              {
                alert('Registration link sent. Please, check your email!');
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
        
        navigate("/");
        navigate(0);
    };

    return (
        <div className="login">
            {
                sending && (
                  <div>
                    <h3>Sending confirmation email <div className="loading">↻</div></h3><span>{email}</span>
                  </div>
                )
            }
            {
                <form onSubmit={handleSubmit}>
                    <h1>Register</h1>
                    <label>Email</label>
                    <input required type="email" value={email} onChange={(e) => setEmail(e.target.value)}></input>
                    <label>First Name</label>
                    <input required type="text" value={firstName} onChange={(e) => setFirstName(e.target.value)}></input>
                    <label>Last Name</label>
                    <input required type="text" value={lastName} onChange={(e) => setLastName(e.target.value)}></input>
                    <label>Password</label>
                    <input required type="password" value={password} onChange={(e) => setPassword(e.target.value)}></input>
                    <label>Confirm Password</label>
                    <input required type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)}></input>
                    {
                        !sending && (<button type="submit">Register</button>)
                    }
                </form>
            }
        </div>
      );
}
 
export default Register;