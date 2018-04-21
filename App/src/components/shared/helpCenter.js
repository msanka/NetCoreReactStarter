import React, { Component } from 'react';

class HelpCenter extends Component
{
    constructor(props)
    {
        super(props);
    }

    render()
    {
        //https://www.iconfinder.com/icons/118806/browser_help_icon#size=32
        let localStyle = 
        {
            'marginTop': '125px',
            'position': 'fixed',
            'top': '0',
            'right' : '0',
            'zIndex': '10000',
            'borderRadius' : '5px 0px 0px 5px',
            'backgroundColor' : 'rgb(196, 235, 248)',
            'boxShadow': '5px 5px 5px #888888',
            'borderStyle': 'solid',
            'borderWidth': '1px 0px 1px 1px',
            'cursor': 'help'
        }
        return(
            <div>
                <div style={localStyle}> 
                    <img alt="help" src={'data:image/png;base64,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'} />
                </div>
            </div>
        );
    }
}

export default HelpCenter;