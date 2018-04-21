import React, { Component } from 'react';
import ReactDOM  from 'react-dom';
import Notifications, {notify} from 'react-notify-toast';
import AppRouter from './components/router';

//Components Import - Start
import ErrorBoundary from './components/shared/errorBounary';
import Footer from './components/shared/footer';
import HelpCenter from './components/shared/helpCenter';
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
      <div>
          <Notifications />
          <ErrorBoundary>
            <AppRouter />
            <HelpCenter />
            <Footer />
          </ErrorBoundary>
      </div>
   );
  }
}

try
{
  ReactDOM.render(<App />, document.getElementById('App') );
}
catch(ex)
{
  console.log(ex);
}
