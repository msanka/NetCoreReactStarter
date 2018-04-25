import { combineReducers } from 'redux';
import UsersReducer from './mdl_users_reducer';

const ModuleReducers = combineReducers(
    {
        Users      : UsersReducer
    });

export default ModuleReducers;