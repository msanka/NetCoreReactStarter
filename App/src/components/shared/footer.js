import React, {Component} from 'react';

class Footer extends Component
{
    constructor(props)
    {
        super(props);
    }

    render()
    {
        return(
            <div className="pos-f-t">
                <nav className="navbar fixed-bottom navbar-dark bg-primary">
                    <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarToggleExternalContent" aria-controls="navbarToggleExternalContent" aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                    </button>
                    <footer>
                        <h4 className="text-white">Native API Proxy</h4>
                    </footer>
                </nav>
                <div className="collapse" id="navbarToggleExternalContent">
                    <hr/>
                    <div className="bg-dark p-4">
                        <h6 className="text-white"><strong>Context Info</strong></h6>
                        <span className="text-muted">Toggleable via the navbar brand.</span>
                    </div>
                </div> 
            </div>
        );
    }
};

export default Footer;