import { combineReducers } from 'redux';
import UsersReducer from './mdl_users_reducer';

const moduleReducerName = 'BlogsModule';

const ModuleReducers = combineReducers(
    {
        Users      : UsersReducer
    });

export { ModuleReducers as default, moduleReducerName };