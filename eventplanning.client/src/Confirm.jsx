import { useNavigate, useSearchParams, useParams } from "react-router";

const Confirm = () => {
  //const [searchParams] = useSearchParams();
  const { status } = useParams();
  const navigate = useNavigate();
  const email = sessionStorage.getItem('user_name');

  return (
    <div className="confirm">
        {
            Number(status) == 200 ?
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