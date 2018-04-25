import { connect } from 'react-redux';
import {bindActionCreators} from 'redux';
import getUsersList from '../actions/act_get_users_list';
import UsersHome from '../components/usersHome';

const mapStateToProps = (state, ownProps) => 
{
    let currentModuleReducer = state.moduleReducers.UserModule;
    let actionType = ownProps.actionType;

    let activeAPI = state.APITracker.activeCalls.filter(ci => ci.actionType == actionType)[0];
    let failedAPI = state.APITracker.failedCalls.filter(ci => ci.actionType == actionType)[0];

    return(
    {
        usersList : currentModuleReducer.Users.usersList,
    });
}

const mapDispatchToProps = (dispatch) => {
    return bindActionCreators(
      { 
        getUsersList : getUsersList,
      }, dispatch);
  }

const UsersHomeContainer = connect(
  mapStateToProps,
  mapDispatchToProps
)(UsersHome)

export default UsersHomeContainer