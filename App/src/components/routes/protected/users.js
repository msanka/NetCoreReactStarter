import React, { Component } from 'react';
import AutoForm from 'react-auto-form';
import ApiWidget from '../../shared/ApiWidget';
import axios from 'axios';

class Users extends Component
{
    constructor(props)
    {
      super(props);

      this.onSubmit = this.onSubmit.bind(this);
    }
 
    onSubmit(event, data)
    {
      event.preventDefault();

      let apiConfig = ApiWidget.getRequestTemplate();

      apiConfig.url = __globals.PROXY_URI + '?url=$Config.jsonPlaceHolderBasePath$/users';
      apiConfig.method = 'get';
      apiConfig.headers = { 'userid' : '$Jwt.nameid$', 'givenName' : '$Jwt.given_name$' }

      let onSuccess = (data) =>
      {
        console.log(data);
      }

      this.refs.btnGetUsers.executeRequest(apiConfig, onSuccess);
    }

    render()
    {
        return(
          <div className="card border-primary">
            <div className="card-header text-white bg-primary">
              Users List
            </div>
            <div className="card-body">
              <h5 className="card-title">List is fetched from API</h5>
              <p className="card-text">Server is proxing the request</p>
              <AutoForm onSubmit={this.onSubmit} trimOnSubmit>
                <div className="form-group">
                  <div className="col-sm-12 controls">
                    <ApiWidget 
                        Component={() => {return(<button type="submit" className="btn btn-primary"> Get Users </button>)} }
                        ref='btnGetUsers'
                        />
                  </div>
                </div>
              </AutoForm>
            </div>
          </div>
        );
    }
}

export default Users;