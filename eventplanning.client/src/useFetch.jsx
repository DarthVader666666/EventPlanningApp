import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const useFetch = (url, method, body) => {
  const [data, setData] = useState(null);
  const [isPending, setIsPending] = useState(true);
  const [error, setError] = useState(null);
  const [status, setStatus] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const token = sessionStorage.getItem("access_token");
    fetch(url, 
        { 
          method: method ? method : 'GET',
          body: body,
          headers:
          {
            "Accept": "application/json",
            "Authorization": "Bearer " + token
          }
        })
        .then(res => {
          setStatus(res.status);

          if(res.status === 401)
          {
            sessionStorage.clear();
            navigate("/login");
          }

          if (!res.ok) { // error coming back from server
            throw Error(res.statusText + ': could not fetch the data for that resource');
          }

          return res.json();
        })
        .then(data => {
          setIsPending(false);
          setData(data);
          setError(null);
        })
        .catch(err => {
          if (err.name === 'AbortError') {
            console.log('fetch aborted')
          }
          else {
            // auto catches network / connection error
            setIsPending(false);
            setError(err.message);
          }
        })
      },
   [url,method,body])

  return { data, isPending, error, status };
}

export default useFetch;