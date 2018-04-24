import APIHelperUtilities from "../utilities/APIHelperUtilities";

export default () => 
{
    return (dispatch, getState) => 
    {
        let _config = APIHelperUtilities.getRequestTemplate();

        _config.url = __globals.PROXY_URI + '?url=$Config.jsonPlaceHolderBasePath$/users1234';
        _config.method = 'get';
        _config.headers = { 'userid' : '$Jwt.nameid$', 'givenName' : '$Jwt.given_name$' }

        let getUsersList = (_config) =>
        {
            return (
            {
                type : 'USERS_GET_USERS',
                payLoad : 
                {
                    request :
                    {
                        ensureLatest : true,
                        successType  : 'USERS_GET_USERS_LIST',
                        config       : _config
                    }
                }
            });
        };
        
        dispatch(getUsersList(_config));
    };
}