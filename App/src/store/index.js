import { compose, applyMiddleware } from 'redux';
import { createStore } from 'redux-dynamic-reducer';//Credit : https://www.npmjs.com/package/react-redux-dynamic-reducer
import thunkMiddleware from 'redux-thunk';
import promiseMiddleware from 'redux-promise';

import appReducers from "../reducers";

let initialState = {};
let enableReduxDevTools = true;

//Do not remove anything that is added to the store here. The middleware is required for the application to function correctly.
// Removing it could cause the app to end up with async actions that never complete. 

export default function() 
{
    var appStore = compose(
        applyMiddleware(thunkMiddleware, promiseMiddleware),
       enableReduxDevTools ? compose(window.devToolsExtension ? window.devToolsExtension() : f => f) : null)
       (createStore);

    var store = appStore(appReducers, initialState);

    return store;
}