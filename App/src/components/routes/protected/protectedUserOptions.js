import React, { Component } from 'react';
import AutoForm from 'react-auto-form';
import axios from 'axios';

class ProtectedUserOptions extends Component
{
    constructor(props)
    {
      super(props);

      this.logoutUser = this.logoutUser.bind(this);
    }

    logoutUser()
    {
        axios.get('/api/security/logout')
        .then(function (response) 
        {
          window.location.href = response.data.redirectUrl;
        })
        .catch(function (error) 
        {
          console.log(error);
        });
    }

    render()
    {
        return(
          <form className="form-inline">
              <div className="collapse navbar-collapse" id="navbarNavDropdown">
                  <ul className="navbar-nav">
                      <li className="nav-item dropdown">
                          <a className="nav-link dropdown-toggle" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                              User Options
                          </a>
                          <div className="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">
                          <a className="dropdown-item" href="#">View Profile</a>
                          <a className="dropdown-item" href="#">Impersonate As</a>
                          <a className="dropdown-item" href="javascript:return void();" onClick={this.logoutUser}>Logout</a>
                          </div>
                      </li>
                  </ul>
              </div>
          </form>
        );
    }
}

export default ProtectedUserOptions;