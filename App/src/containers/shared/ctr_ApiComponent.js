import { connect } from 'react-redux';
import {bindActionCreators} from 'redux';
import ApiComponent from '../../components/shared/ApiComponent';

const mapStateToProps = (state, ownProps) => 
{
    let actionType = ownProps.actionType;

    let activeAPI = state.APITracker.activeCalls.filter(ci => ci.actionType == actionType)[0];
    let failedAPI = state.APITracker.failedCalls.filter(ci => ci.actionType == actionType)[0];

    return(
    {
        hasError : (failedAPI != null),
        errorMessage : (failedAPI != null) ? failedAPI.error.message : null,
        isBusy : (activeAPI != null),
        Component : ownProps.Component,
    });
}

const ApiComponentContainer = connect(
  mapStateToProps,
)(ApiComponent)

export default ApiComponentContainer