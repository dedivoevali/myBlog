import { Link } from "react-router-dom"
import "../NotFoundPage/NotFoundPage.scss";
import { useTitle } from "../../hooks/use-title";

const NotFoundPage = () => {
    useTitle('404');
    return (<div className="not-found-page-wrapper">
        <h1>The existing URL was not found on this server.</h1>
        <Link to="/" className="not-found-page-button">Go home</Link>
    </div>)
}

export { NotFoundPage }