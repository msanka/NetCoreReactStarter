import React, { Component } from 'react';
import AutoForm from 'react-auto-form';
import ApiComponent from '../../../components/shared/ApiComponent';


class UsersHome extends Component
{
    constructor(props)
    {
      super(props);

      this.onSubmit = this.onSubmit.bind(this);
    }
 
    onSubmit(event, data)
    {
      event.preventDefault();
      if (this.props.getUsersList != null)
      {
          this.props.getUsersList();
      }
    }

    renderUsersList()
    {
      if ((this.props.usersList == null) || (this.props.usersList.length <=0))
    return (<div> Users not available </div>);
      
      return this.props.usersList.map((user) => {
        return (<div> {user.name} </div>)
      });
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
                    <ApiComponent
                        actionType = 'USERS_GET_USERS' 
                        Component={() => {return(<button type="submit" className="btn btn-primary"> Get Users </button>)} }
                        ref='btnGetUsers'
                        />
                  </div>
                </div>
              </AutoForm>
              <hr/>
              {this.renderUsersList()}
            </div>
          </div>
        );
    }
}

export default UsersHome;