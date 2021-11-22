import React from 'react';
import Icon from './Icon';
import 'react-toastify/dist/ReactToastify.css';
import '../App.scss';

export default class QuestionModal extends React.Component {
  constructor(props) {
    super(props);
    this.onClose = this.onClose.bind(this);
    this.onConfirm = this.onConfirm.bind(this);
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
          <span className="Message">Are you sure you want to remove product <b>{model.name}</b> from your shop?</span>
          <div className="ButtonPanel">
            <button onClick={this.onConfirm}>                
              <Icon icon={"confirm"}/>
              <span className="Text">Confirm</span>
            </button>
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