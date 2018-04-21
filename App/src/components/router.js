import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch, Link } from 'react-router-dom';
import appRoutes from './routes';
import ProtectedUserOptions from './routes/protected/protectedUserOptions';

class AppRouter extends Component 
{
    constructor(props)
    {
        super(props);
        
        let _routes = appRoutes.registeredRoutes.public;
        let _isProtectedRoute = false;

        if (window.location.pathname.toLowerCase().startsWith('/protected'))
        {
            _routes = appRoutes.registeredRoutes.protected;
            _isProtectedRoute = true;
        }

        this.state =
        {
            routes : _routes,
            isProtectedRoute : _isProtectedRoute
        };
    }

    buildLinks(routes)
    {
        return this.state.routes.map((routeInfo,index) =>
        {
            return (<li key={index} className="nav-item"> <Link className="nav-link" to={routeInfo.path}> {routeInfo.name} </Link>  </li>);
        });
    }

    buildRoutes(routes)
    {
        return this.state.routes.map((routeInfo,index) =>
        {
            return (<Route key={index} {...routeInfo} />);
        });
    }

    renderLogedOnProfile()
    {
        if (!this.state.isProtectedRoute)
            return null;

        return(<ProtectedUserOptions />);
    }

    render()
    {
        let localStyle =
        {
            borderStyle : 'solid',
            borderWidth : '1px',
            borderColor : 'blue',
            margin      : '5px',
            padding     : '10px'
        };

        return (
                <Router>
                    <div>
                        <nav className="navbar navbar-expand-lg navbar-dark bg-primary justify-content-between">
                            <a className="navbar-brand" href="#">Native API Proxy</a>
                            <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarColor01" aria-controls="navbarColor01" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                            </button>
                            <div className="collapse navbar-collapse" id="navbarColor01">
                                <ul className="navbar-nav mr-auto">
                                    {this.buildLinks()}
                                </ul>
                            </div>
                            {this.renderLogedOnProfile()}
                        </nav>
                        <div style={localStyle}>
                            {this.buildRoutes()}
                        </div>
                    </div>
                </Router>
            );
    }
};

export default AppRouter;
