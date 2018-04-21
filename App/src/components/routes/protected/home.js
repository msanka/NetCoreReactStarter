import React, { Component } from 'react';
import AutoForm from 'react-auto-form';
import axios from 'axios';

class SecuredLandingPage extends Component
{
  render()
  {
    return(
      <div className="row"> 
          <div className="col-md-12">
            <div className="card border-primary">
              <div className="card-header text-white bg-primary">
                Welcome
              </div>
              <div className="card-body">
                <h5 className="card-title">You are in the home page under secured area</h5>
                <p className="card-text">You may signout using the logout option from the User Options</p>
              </div>
            </div>
          </div>
      </div>);
  }
}

export default SecuredLandingPage;