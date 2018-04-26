/*

*/

let defaultState = 
{
    usersList : [],
};

export default (state = defaultState, action) =>
{
    switch (action.type) 
    {
        case 'USERS_GET_USERS_LIST': 
            {
                let newState = {...state};

                newState.usersList = action.payLoad;
                
                return newState;
            }
        default:
            {
                return state;
            }
    }
}