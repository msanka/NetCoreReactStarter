import axios from 'axios';
import Qs from 'qs';

export default (id, instanceId, config, onSuccess, onFailed, onNotifyProgress=null, alwaysNew=true) => 
{
    console.log('Register New Layout Action Received');

    return (dispatch, getState) => 
    {
        const { APIHelperRegistry } = getState();
        
        if ( alwaysNew == true)
        {
            let onGoingActiveCall = APIHelperRegistry.activeCalls.filter(ac => ac.id == id && ac.instanceId == instanceId)[0];

            if (onGoingActiveCall != null)
            {
                onGoingActiveCall.cancel();
            }
        }
        
        let cancelTokenSource = axios.CancelToken.source();

        let updateWidgetSettings = (config, cancelTokenSource) =>
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
                cancelToken : cancelTokenSource.token,
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
        
        let updatedConfig = this.updateWidgetSettings(config, cancelTokenSource);
        
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


    };
}