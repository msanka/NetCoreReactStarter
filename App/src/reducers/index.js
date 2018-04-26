import { combineReducers } from 'redux';
import APITrackerReducer from './api_tracker_reducer';
                               
const AppReducers = (moduleReducers) =>
{
    return combineReducers(
    {
        APITracker : APITrackerReducer,
        ...moduleReducers
    });
}

export default AppReducers;