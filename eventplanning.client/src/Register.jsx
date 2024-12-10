import { useState } from "react";
import { useNavigate } from "react-router";

const Register = () => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    
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
            
            await fetch('api/authorization/register/',
                {
                    body: JSON.stringify({ email, firstName, lastName, password }),
                    headers:
                    {
                      "Content-Type": "application/json"
                    },
                    method: "POST"
                })
            .then(response => response.json())
            .then(data => 
                {
                    sessionStorage.setItem("access_token", data.access_token);
                    sessionStorage.setItem("user_name", data.user_name);
                });
        
        navigate("/");
    };

    return (
        <div className="login">
            {
                <form onSubmit={handleSubmit}>
                    <label>Email</label>
                    <input required type="text" value={email} onChange={(e) => setEmail(e.target.value)}></input>
                    <label>First Name</label>
                    <input required type="text" value={firstName} onChange={(e) => setFirstName(e.target.value)}></input>
                    <label>Last Name</label>
                    <input required type="text" value={lastName} onChange={(e) => setLastName(e.target.value)}></input>
                    <label>Password</label>
                    <input required type="password" value={password} onChange={(e) => setPassword(e.target.value)}></input>
                    <label>Confirm Password</label>
                    <input required type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)}></input>
                    <button type="submit">Register</button>
                </form>
            }
        </div>
      );
}
 
export default Register;