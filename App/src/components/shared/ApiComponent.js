import React , { Component } from 'react';
import {notify} from 'react-notify-toast';

class APIComponent extends Component
{

    constructor(props)
    {
        super(props);
    }

    componentWillReceiveProps(nextProps)
    {
        
    }

    getImageProperties()
    {
        return (
        {
            src : __globals.SPA_IMAGES_ROOT + (this.props.hasError ? '/error.gif' : '/loadingGear.gif'),
            title : this.props.hasError ? this.props.errorMessage : null,
            alt : this.props.hasError ? 'Error' : 'Loading'
        });
    }

    render()
    {
        let DynamicComponent = this.props.Component;

        if (!this.props.isBusy && !this.props.hasError)
            return <DynamicComponent />;

        let imageProps = this.getImageProperties();

        return(
            <div className="d-inline-flex">
                <div><DynamicComponent /></div>
                <div>
                    <img {...imageProps} style={{ height : '30px', width : '30px', marginLeft : '5px' }} />
                </div>
            </div>
        );
    }
}

export default APIComponent;