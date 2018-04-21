import React, { Component } from 'react';
import AutoForm from 'react-auto-form';
import axios from 'axios';
import BootstrapAlert from '../../shared/bootstrapAlert';

class Login extends Component
{
    constructor(props)
    {
      super(props);

      this.state=
      {
        hasError : false,
        errorMessage : null
      }

      this.onSubmit = this.onSubmit.bind(this);
    }
    
    onSubmit(event, data)
    {
      event.preventDefault();

      this.setState({ hasError : false, errorMessage : null });

      axios.post('/api/security/authenticate',data)
      .then((response) =>
      {
          if (response.status == "200")
          {
            let redirectUrl = response.data.redirectUrl;
            window.location.href = redirectUrl;
          }
          else
          {
            this.setState({ hasError : true, errorMessage : response.data.message });  
          }
      })
      .catch((error) =>
      {
          this.setState({ hasError : true, errorMessage : error.response.data.message });
      });
    }

    render()
    {
        return(
          <div className="card border-primary">
            <div className="card-header text-white bg-primary">
              Login
            </div>
            <div className="card-body">
              <h5 className="card-title">Login to view secured content</h5>
              <p className="card-text">Security is implemented using JWT</p>
              <AutoForm onSubmit={this.onSubmit} trimOnSubmit>
                <BootstrapAlert type='danger' showAlert={this.state.hasError} message={this.state.errorMessage} />
                <div className="input-group">
                  <span className="input-group-addon"><i className="glyphicon glyphicon-user"></i></span>
                  <input id="Username" type="text" className="form-control" name="Username" placeholder="username or email" />
                </div>
                <br />
                <div className="input-group">
                  <span className="input-group-addon"><i className="glyphicon glyphicon-lock"></i></span>
                  <input id="Password" type="password" className="form-control" name="Password" placeholder="password" />
                </div>
                <hr />
                <div className="form-group">
                  <div className="col-sm-12 controls">
                    <button type="submit" className="btn btn-primary"> Login </button>
                  </div>
                </div>
              </AutoForm>
            </div>
          </div>
        );
    }
}


class Home extends Component
{
  render(){
  return(
    <div className="row"> 
        <div className="col-md-3">
          <Login />
        </div>
        <div className="col-md-9" />
    </div>);
  }
}

export default Home;