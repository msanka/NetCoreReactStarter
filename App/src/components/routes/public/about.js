import React, { Component } from 'react';

class About extends Component
{
  render()
  {
    let divProducer = (len) =>
    {
      return Array.from({length: len}, () => Math.floor(Math.random() * len)).map((num,index) =>
      {
        return (<div key={index}> {index}. Number at this index [{num}] </div>);
      });
    };

    return(
      <div> 
          About 
          {divProducer(40)}
      </div>
    );
  }
}

export default About;