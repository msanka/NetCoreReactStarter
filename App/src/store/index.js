import { createStore, compose, applyMiddleware } from 'redux';
//import { createStore } from 'redux-dynamic-reducer';//Credit : https://www.npmjs.com/package/react-redux-dynamic-reducer
import thunkMiddleware from 'redux-thunk';
import promiseMiddleware from 'redux-promise';
import axiosAPIMiddleware from '../utilities/middleware_axios';

import appReducers from "../reducers";

let initialState = {};
let enableReduxDevTools = true;

//Do not remove anything that is added to the store here. The middleware is required for the application to function correctly.
// Removing it could cause the app to end up with async actions that never complete. 

export default (function() 
{
    var appStore = compose(
        applyMiddleware(thunkMiddleware, promiseMiddleware, axiosAPIMiddleware),
       enableReduxDevTools ? compose(window.devToolsExtension ? window.devToolsExtension() : f => f) : null)
       (createStore);

    const store = appStore(appReducers(), initialState);
    store.moduleReducers = {};
    return store;
})();


//Credit : https://stackoverflow.com/questions/32968016/how-to-dynamically-load-reducers-for-code-splitting-in-a-redux-application
export function injectModuleReducer(store, name, asyncReducer) {
    store.moduleReducers[name] = asyncReducer;
    store.replaceReducer(appReducers(store.moduleReducers));
  }