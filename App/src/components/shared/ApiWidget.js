import React , { Component } from 'react';
import axios from 'axios';
import Qs from 'qs';
import {notify} from 'react-notify-toast';
import img_LoadingGear from '../../images/loadingGear.gif';
import img_Error from '../../images/error.gif';
import { SSL_OP_NETSCAPE_DEMO_CIPHER_CHANGE_BUG } from 'constants';

class APIWidget extends Component
{
    constructor(props)
    {
        super(props);
        this.state=
        {
            isBusy      : false,
            hasError    : false,
            errorMessage : null,
        }
    }

    static getRequestTemplate()
    {
        //Credit : https://github.com/axios/axios
        return (
        {
            url: null,
            method: 'get', // default
            baseURL: '',
            headers: null,
            queryParams: {},
            body: {},
            responseType: 'json', // default
            responseEncoding: 'utf8', // default
            cancelToken: null
        });
    }

    updateWidgetSettings(config)
    {
        let widgetSettings =
        {
            transformRequest: null,
            transformResponse: null,
            timeout: 2000,
            withCredentials: false, // default
            adapter: null,
            auth: null,
            paramsSerializer: function(params) 
            {
              return Qs.stringify(params, {arrayFormat: 'brackets'})
            },
            // `onUploadProgress` allows handling of progress events for uploads
            onUploadProgress: function (progressEvent) 
            {
                // Do whatever you want with the native progress event
            },
              // `onDownloadProgress` allows handling of progress events for downloads
            onDownloadProgress: function (progressEvent) 
            {
                // Do whatever you want with the native progress event
            },
            params : config.queryParams,
            data   : config.body
        };

        let updatedConfig = {...config, ...widgetSettings};
        return updatedConfig;
    }

    updateExecutionState(updatedProps)
    {
        this.setState({...updatedProps});
        if (updatedProps.hasOwnProperty('hasError') && updatedProps.hasError == true)
        {
            notify.show(updatedProps.errorMessage, "error");
        }
    }

    executeRequest(config, onSuccess)
    {
        this.setState({isBusy : true, hasError : false});
        let updatedConfig = this.updateWidgetSettings(config);
        
        axios.request(updatedConfig)
        .then((response) =>
        {
            if (response.status == "200")
            {
                this.updateExecutionState({isBusy : false});
                onSuccess(response.data);
            }
            else
            {
                this.updateExecutionState({isBusy : false,  hasError : true, errorMessage : response.data.message});
            }
        })
        .catch((error) =>
        {
            let message = error.message;
            this.updateExecutionState({isBusy : false,  hasError : true, errorMessage : message });
        });
    }

    componentWillReceiveProps(nextProps)
    {
        this.setState({...nextProps});
    }

    getImageProperties()
    {
        return (
        {
            src : this.state.hasError ? img_Error : img_LoadingGear,
            title : this.state.hasError ? this.state.errorMessage : null,
            alt : this.state.hasError ? 'Error' : 'Loading'
        });
    }

    render()
    {
        let DynamicComponent = this.props.Component;

        if (!this.state.isBusy && !this.state.hasError)
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

export default APIWidget;