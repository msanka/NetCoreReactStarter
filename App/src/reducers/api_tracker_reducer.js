/*
activeCall:
{
    actionType : '',
    cancel : null,
}
failedCall:
{
    actionType : '',
    error : null,//Error Object
    response : null // raw response
    actionInfo : null // actual action
}
*/

let defaultState = 
{
    activeCalls : [],
    failedCalls : []
};

export default (state = defaultState, action) =>
{
    switch (action.type) 
    {
        case 'API_PUSH_ACTIVE_CALL': 
            {
                let newState = {...state};

                newState.activeCalls.push(action.payLoad);
                
                return newState;
            }
        case 'API_POP_ACTIVE_CALL': 
            {
                let newState = {...state};

                newState.activeCalls = newState.activeCalls.filter(ci => ci.actionType != action.payLoad.actionType);
                
                return newState;
            }
        case 'API_CLEAR_ERRORS': 
            {
                let newState = {...state};

                newState.failedCalls = newState.failedCalls.filter(ci => ci.actionType != action.payLoad.actionType);

                return newState;
            }
        case 'API_CALL_FAILED': 
            {
                let newState = {...state};

                newState.failedCalls.push(action.payLoad);

                return newState;
            }
        default:
            {
                return state;
            }
    }
}