import { connect } from 'react-redux';
import {bindActionCreators} from 'redux';
import getUsersList from '../../../actions/act_get_users_list';
import users from '../../../components/routes/protected/users';

const mapStateToProps = (state, ownProps) => 
{
    let actionType = ownProps.actionType;

    let activeAPI = state.APITracker.activeCalls.filter(ci => ci.actionType == actionType)[0];
    let failedAPI = state.APITracker.failedCalls.filter(ci => ci.actionType == actionType)[0];

    return(
    {
        usersList : state.Users.usersList,
    });
}

const mapDispatchToProps = (dispatch) => {
    return bindActionCreators(
      { 
        getUsersList : getUsersList,
      }, dispatch);
  }

const UsersContainer = connect(
  mapStateToProps,
  mapDispatchToProps
)(users)

export default UsersContainer