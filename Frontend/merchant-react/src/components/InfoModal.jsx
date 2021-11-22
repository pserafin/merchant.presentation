import React from 'react';
import Icon from './Icon';
import 'react-toastify/dist/ReactToastify.css';
import '../App.scss';

export default class InfoModal extends React.Component {
  constructor(props) {
    super(props);
    this.onClose = this.onClose.bind(this);
  };

  onClose() {
    this.props.onModalClose(false);
  }

  onConfirm() {
    this.props.onModalClose(true);
  }

  render() {
    const model = this.props.model;

    return (
      <div className="OpenedModal">
        <button className="Close-button" onClick={this.onClose}>✖</button>  
        <div className="App-form-login">            
          <ul className="Message">
            {model.map((item, i) => {     
              return (
                <li key={i}>{item}</li>
                );
              })
            }                
          </ul>
          <div className="ButtonPanel">
            <button onClick={this.onClose}>
              <Icon icon={"close"}/>
              <span className="Text">Close</span>
            </button>
          </div>              
        </div>
      </div>
    );
  };
}