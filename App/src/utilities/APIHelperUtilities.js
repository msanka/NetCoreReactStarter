import axios from 'axios';
import Qs from 'qs';

const CancelToken = axios.CancelToken;

class APIHelperUtilities
{
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
            responseEncoding: 'utf8' // default
        });
    }

    static executeRequest(config, onSuccess, onFailed, onNotifyProgress=null)
    {
        let updateWidgetSettings = (config) =>
        {
            let widgetSettings =
            {
                transformRequest: null,
                transformResponse: null,
                timeout: 3000,
                withCredentials: false, // default
                adapter: null,
                auth: null,
                paramsSerializer: function(params) 
                {
                    return Qs.stringify(params, {arrayFormat: 'brackets'})
                },
                // `onUploadProgress` allows handling of progress events for uploads
                // Do whatever you want with the native progress event
                onUploadProgress: function (progressEvent) 
                {
                    if(onNotifyProgress != null) 
                    {
                        onNotifyProgress('upload', progressEvent);
                    }
                },
                // `onDownloadProgress` allows handling of progress events for downloads
                // Do whatever you want with the native progress event
                onDownloadProgress: function (progressEvent) 
                {
                    if(onNotifyProgress != null) 
                    {
                        onNotifyProgress('download', progressEvent);
                    }
                },
                params : config.queryParams,
                data   : config.body
            };

            let updatedConfig = {...config, ...widgetSettings};
            return updatedConfig;
        }
        
        if (onNotifyProgress != null)
        {
            config.onNotifyProgress = onNotifyProgress;
        }
        
        let updatedConfig = this.updateWidgetSettings(config);
        
        axios.request(updatedConfig)
        .then((response) =>
        {
            if (response.status == "200")
            {
                onSuccess(response.data);
            }
            else
            {
                onFailed(new Error('Request failed with a non 200 response'), response);
            }
        })
        .catch((error) =>
        {
            onFailed(error, response);
        });
    }
}

export default APIHelperUtilities;