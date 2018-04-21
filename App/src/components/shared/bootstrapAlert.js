import React , { Component } from 'react';

class BootstrapAlert extends Component
{
    constructor(props)
    {
        super(props);
        this.state=
        {
            showAlert   : false,
            message     : null,
            type        : 'success' 
        }
    }

    componentWillReceiveProps(nextProps)
    {
        this.setState({...nextProps});
    }

    render()
    {
        if (!this.state.showAlert)
            return null;

        return(
            <div className={"alert alert-" + this.state.type } role="alert">
                {this.state.message}
            </div>
        );
    }
}

export default BootstrapAlert;