import React, { Component } from 'react';
import UsersHome from '../containers/ctr_usersHome';

class ModuleHome extends Component
{
    constructor(props)
    {
      super(props);
    }
 
    render()
    {
        return(<UsersHome />);
    }
}

export default ModuleHome;