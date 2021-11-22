import React from 'react';
import 'react-toastify/dist/ReactToastify.css';
import '../App.scss';
import { validateProduct } from '../actions/verifications';
import Icon from './Icon';

export default class ProductModal extends React.Component {
  constructor(props) {
    super(props);
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
    this.onClose = this.onClose.bind(this);
    this.onSave = this.onSave.bind(this);
  };

  handleInputChange(event) {
    const model = {...this.props.model, [event.target.name]: event.target.value}
    this.props.onUpdate(model);
  };

  handleCheckboxChange(event) {
    const model = {...this.props.model, isEnabled: event.target.checked}
    this.props.onUpdate(model);
  };    

  onClose() {
    this.props.onModalClose(false);
  }

  onSave() {
    this.props.onModalClose(true);
  }

  render() {
    const model = this.props.model;

    return (
      <div className="OpenedModal">
        <button className="Close-button" onClick={this.onClose}><Icon icon={"close"}/></button>  
        <div className="App-form-login">            
          <span>Name:</span>
          <input className="App-input" type="text" required onChange={this.handleInputChange} value={model.name} name="name"></input>
          <span>Quantity:</span>
          <input className="App-input" type="text" required onChange={this.handleInputChange} value={model.quantity} name="quantity"></input>
          <span>Price:</span>
          <input className="App-input" type="text" required onChange={this.handleInputChange} value={model.price} name="price"></input>
          <span>For Sale:</span>
          <input className="App-input" type="checkbox" required onChange={this.handleCheckboxChange} checked={model.isEnabled} name="isEnabled"></input>
          <div className="ButtonPanel">
            <button onClick={this.onSave} disabled={!validateProduct(model)}>                
              <Icon icon={"save"}/>
              <span className="Text">Save</span>
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