import React, { Component } from 'react';
import ReactDOM  from 'react-dom';
import { Provider } from 'react-redux';
import Notifications, {notify} from 'react-notify-toast';
import AppRouter from './components/router';
//Store Import
import AppStore from './store';
//Components Import - Start
import ErrorBoundary from './components/shared/errorBounary';
import Footer from './components/shared/footer';
import HelpCenter from './components/shared/helpCenter';
import ServiceWorker from './utilities/serviceworker';
//Components Import - End

class App extends Component
{
  constructor(props) 
  {
    super(props);
  }

  render()
  {
   return(
    <Provider store={AppStore}>
      <div>
          <Notifications />
          <ErrorBoundary>
            <AppRouter />
            <HelpCenter />
            <Footer />
          </ErrorBoundary>
      </div>
    </Provider>
   );
  }
}

try
{
  ReactDOM.render(<App />, document.getElementById('App') );
  ServiceWorker.registerServiceWorker();
  
}
catch(ex)
{
  console.log(ex);
}
