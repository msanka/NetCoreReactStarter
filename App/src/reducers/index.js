import { combineReducers } from 'redux';
import APITrackerReducer from './api_tracker_reducer';
import UsersModuleReducer from "./mdl_users_reducer";
                               
const AppReducers = (moduleReducers) =>
{
    return combineReducers(
    {
        APITracker : APITrackerReducer,
        Users      : UsersModuleReducer,
        ...moduleReducers
    });
}

export default AppReducers;