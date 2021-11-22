import React from 'react';
import '../App.scss'
import Icon from './Icon'
import { toast } from 'react-toastify';
import { isCustomer, isCurrentModule, isAdmin } from '../actions/verifications';
import { createNavigationState, createLogoutState } from '../actions/creations';
import 'react-toastify/dist/ReactToastify.css';

const axios = require('axios');

export default class TopBar extends React.Component {
  constructor(props) {
    super(props);
    this.handleLogoutClick = this.handleLogoutClick.bind(this);
    this.handleCartClick = this.handleCartClick.bind(this);
    this.handleHomeClick = this.handleHomeClick.bind(this);
    this.handleOrdersClick = this.handleOrdersClick.bind(this);
  };

  handleCartClick() {
    this.props.navigation(createNavigationState("cart"));
  };

  handleHomeClick() {
    this.props.navigation(createNavigationState("products"));
  };

  handleOrdersClick() {
    this.props.navigation(createNavigationState("orders"));
  }

  handleLogoutClick() {
    axios.post(process.env.REACT_APP_API_URL + 'account/logout', null, 
    {
      credentials: 'include'
    })
    .then(() => {
      toast.success("Farwell...");
      this.props.actionLogout(createLogoutState());
    })
    .catch(() =>  {
      toast.success("Farwell...");
      this.props.actionLogout(createLogoutState());
    })
  }

  render() {
    const model = this.props.model;

    return (
      <div className="Top-bar">
        <div className="Top-bar-left">
          <span>Merchant-App</span>
        </div>
        <div className="Top-bar-right">
          <button className={isCurrentModule(model, 'products') ? 'Button-icon Icon-left Selected' : 'Button-icon Icon-left'} title="Home" onClick={this.handleHomeClick}>
            <Icon icon={"products"} />
          </button>
          {isAdmin(model) && 
            <button className={isCurrentModule(model, 'orders') ? 'Button-icon Icon-left Selected' : 'Button-icon Icon-left'} title="Orders" onClick={this.handleOrdersClick}>
              <Icon icon={"orders"} className={isCurrentModule(model, 'orders') ? 'Selected' : ''}/>
            </button>
          }                
          {isCustomer(model) && 
            <button className={isCurrentModule(model, 'cart') ? 'Button-icon Icon-left Selected' : 'Button-icon Icon-left'} title="Your cart" onClick={this.handleCartClick}>
              <Icon icon={"cart"} badge={model.cartItems} className={isCurrentModule(model, 'cart') ? 'Selected' : ''}/>
            </button>
          }
          <button className="Button-icon" title="Log out" onClick={this.handleLogoutClick}>
            <Icon icon={"logout"}/>
          </button>
        </div>
      </div>
    );
  };
}