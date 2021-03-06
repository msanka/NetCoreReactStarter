import React, { Component } from 'react';
import Loadable from 'react-loadable';
//import { withReducer } from 'react-redux-dynamic-reducer';
//Store Import
import AppStore, { injectModuleReducer} from '../../store';

class Loading extends Component
{ 
    render()
    {
        let props = {...this.props};

        if (props.error) 
        {
            return <div>Error! { ' More Info : ' + props.error.message } </div>;
        } 
        else if (props.timedOut) 
        {
            return <div>Taking a long time...</div>;
        } 
        else if (props.pastDelay) 
        {
            return <div>Loading...</div>;
        } 
        else 
        {
            return null;
        }
    }
}

const loadableOptions =
{
    loading: Loading,
    delay: 200,
    timeout: 10,
};

export default 
{
    registeredRoutes:
    {
        public:
        [
            {
                name        : 'Home',
                path        : '/public/index',
                exact       : true,
                component   : Loadable({ loader: () => import('./public/home').then((resp) => {return resp.default}), ...loadableOptions })
            },
            {
                name        : 'About',
                path        : '/public/about',
                strict      : true,
                component   : Loadable({ loader: () => import('./public/about').then((resp) => {return resp.default}), ...loadableOptions })
            }
        ],
        protected:
        [
            {
                name        : 'Home',
                path        : '/protected/index',
                strict      : true,
                component   : Loadable({ loader: () => import('./protected/home').then((resp) => {return resp.default}), ...loadableOptions })
            },
            {
                name        : 'Blogs',
                path        : '/protected/blogs',
                strict      : true,
                component   : Loadable({ loader: () => import('../../modules/Blogs')
                                                       .then((resp) => 
                                                       { 
                                                           let store = AppStore;
                                                           injectModuleReducer(store, resp.moduleReducerName, resp.ModuleReducer); 
                                                           return resp.default; 
                                                        }), ...loadableOptions })
            }
        ],
    }
    
};