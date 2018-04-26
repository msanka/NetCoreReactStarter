import axios from 'axios';
import Qs from 'qs';

const CancelToken = axios.CancelToken;

const makeAPIRequest = store => next => action => 
{
    //console.log('dispatching', action)

    if ((action.payLoad != null) && (action.payLoad.request != null))
    {

        if (action.payLoad.request.ensureLatest != null && action.payLoad.request.ensureLatest == true)
        {
            const { APITracker } = store.getState();
            let pendingCall = APITracker.activeCalls.filter(ci => ci.actionType != action.type)[0];

            if (pendingCall != null)
            {
                pendingCall.cancel();
                store.dispatch(dispatcherAction('API_POP_ACTIVE_CALL', {actionType : action.type}));
            }
        }

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
                onUploadProgress: null,
                // `onDownloadProgress` allows handling of progress events for downloads
                // Do whatever you want with the native progress event
                onDownloadProgress: null,
                cancelToken : null,
                params : config.queryParams,
                data   : config.body
            };

            let updatedConfig = {...config, ...widgetSettings};
            return updatedConfig;
        }
        
        let updatedConfig = updateWidgetSettings(action.payLoad.request.config);

        let sourceToken = CancelToken.source();
        updatedConfig.cancelToken = sourceToken.token;

        let dispatcherAction = (_type, _payLoad) =>
        {
            return(
                {
                    type    : _type,
                    payLoad : _payLoad
                });
        }

        store.dispatch(dispatcherAction('API_CLEAR_ERRORS', {actionType : action.type}));
        store.dispatch(dispatcherAction('API_PUSH_ACTIVE_CALL', {actionType : action.type, cancel : sourceToken }));
       
        return axios.request(updatedConfig)
        .then((response) =>
        {
            store.dispatch(dispatcherAction('API_POP_ACTIVE_CALL', {actionType : action.type}));

            if (response.status == "200")
            {
                next(store.dispatch(dispatcherAction(action.payLoad.request.successType, response.data)));
            }
            else
            {
                let failedPayload = {
                    actionType : action.type,
                    error : new Error('Request failed with a non 200 response. Received Response code "${response.status}"'),
                    errorResponse : response,
                    actionInfo : action
                }
                next(store.dispatch(dispatcherAction('API_CALL_FAILED', failedPayload)));
            }
        })
        .catch((_error) =>
        {
            store.dispatch(dispatcherAction('API_POP_ACTIVE_CALL', {actionType : action.type}));

            let failedPayload = {
                actionType : action.type,
                error : _error,
                errorResponse : _error.response,
                actionInfo : action
            }
            next(store.dispatch(dispatcherAction('API_CALL_FAILED', failedPayload)));
        });
    }
    else
    {
        return next(action);
    }
}

export default makeAPIRequest;
