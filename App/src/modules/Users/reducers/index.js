/*
activeCall:
{
    id : '',
    instanceId : '',
    cancel : null,
    config : null,
}
*/

let defaultState = 
{
    activeCalls : [],
};

export default (state = defaultState, action) =>
{
    switch (action.type) 
    {
        case 'INITIALIZE_KNOWN_COMPONENTS': 
            {
                console.log('Action INITIALIZE_KNOWN_COMPONENTS processed on Known Components Reducer');
                if (action.payLoad == null)
                {
                    console.log('Known Components Registry loaded with empty content.');
                }
                return state = action.payLoad;
            }
        default:
            {
                return state;
            }
    }
}