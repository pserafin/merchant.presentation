import React from 'react';
import { toast } from 'react-toastify';
import Icon from './Icon'
import InfoModal from './InfoModal';
import { SpinnerCircular } from 'spinners-react';
import 'react-toastify/dist/ReactToastify.css';
import '../App.scss';
import { createEmptyOrder, updateOrderItem, deleteOrderItem, deepClone, createEmptyCartState, createValidationModel } from '../actions/creations';
import { isInternalError, isForbidden, isNotAccepted } from '../actions/verifications'
import { countItems, sumOrder } from '../actions/calculations';

const axios = require('axios');

export default class Cart extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
        orderModel: createEmptyOrder(),
        modified: false,
        openedModal: "",
        messages: [],
        showSpinner: false
    }
    this.onInfoModalClose = this.onInfoModalClose.bind(this);
  };

  handleDecreaseClick(orderItem) {
    const item = {...orderItem, quantity: -1};
    const order = updateOrderItem(this.state.orderModel, item);
    this.setState({orderModel: order, modified: true});
  }

  handleIncreaseClick(orderItem) {
    const item = {...orderItem, quantity: 1};
    const order = updateOrderItem(this.state.orderModel, item);
    this.setState({orderModel: order, modified: true});
  }

  handleDeleteClick(orderItem) {
    const order = deleteOrderItem(this.state.orderModel, orderItem);
    this.setState({orderModel: order, modified: true});    
  }

  componentDidMount() {
    this.setState({orderModel: deepClone(this.props.model.order), modified: false});
  };

  handleSaveClick() {
    this.setState({showSpinner: true});
    axios.put(process.env.REACT_APP_API_URL + 'order/update', this.state.orderModel)
    .then(x => {
      this.setState(createEmptyCartState());
      this.props.action({order : x.data, cartItems: countItems(x.data) })
      toast.success("Your order has updated");
    })
    .catch(error => {
      this.setState(createEmptyCartState());
      if (isInternalError(error) || isForbidden(error)) {
        toast.error(error.response.data.message);
      } else if (isNotAccepted(error)) {
        const errors = createValidationModel(error.response.data.message);
        this.showValidationMessage(errors);
      }

    });  
  }

  handleValidateClick() {
    this.setState({showSpinner: true});
    axios.post(process.env.REACT_APP_API_URL + 'order/validate', this.state.orderModel)
    .then(x => {
      this.showValidationMessage(x.data);
    })
    .catch(error => {
      this.setState(createEmptyCartState());
      if (isInternalError(error) || isForbidden(error)) {
        toast.error(error.response.data.message);
      }
    });  
  }

  showValidationMessage(data) {
    if(!data) {
      this.setState(createEmptyCartState());
      return;
    } else if(data.isValid) {
      this.setState(createEmptyCartState());
      toast.success("Your order is valid");
      return;
    }

    this.setState({openedModal: "info", showSpinner: false, messages: data.messages});
    toast.warning("Your order did not pass validation");
  }

  onInfoModalClose() {
    this.setState(createEmptyCartState());    
  }

  render() {
    const orderModel = this.state.orderModel;
    const hideModalContainer = this.state.openedModal === "" || this.state.openedModal === "_";
    const saveDisabled = this.state.openedModal !== "" || !this.state.modified;
    const verifyDisabled = this.state.openedModal !== "" || !orderModel.items || orderModel.items.length === 0;

    return (
      <div>
        <div className={this.state.showSpinner ? "SpinnerModal Visible" : "SpinnerModal Hidden"}>
          <SpinnerCircular enabled={this.state.showSpinner} thickness={200}/>                
        </div>  
        <div className={hideModalContainer ? "Modal Hidden" : "Modal Visible"}>
          {this.state.openedModal === "info" &&
            <InfoModal model={this.state.messages} onModalClose={this.onInfoModalClose}/>
          }    
        </div>  
        <div className={this.state.openedModal !== "" ? "Blured" : ""}>
          <h1>Your current order</h1>
          <div className="Table">
            <div className="Table-row No-border Action-panel">
              <button className="Button-action" title="Save" onClick={this.handleSaveClick.bind(this)} disabled={saveDisabled}>
                <Icon icon={"save"}/>
                <span className="Text">Save</span>
              </button>
              <button className="Button-action" title="Validate" onClick={this.handleValidateClick.bind(this)} disabled={verifyDisabled}>
                <Icon icon={"add"}/>
                <span className="Text">Validate</span>
              </button>              
            </div>
            <div className="Scrollable"> 
            <div className="Table-row Table-header">
              <div className="Small-col">No.</div>
              <div>Name</div>
              <div>Price</div>
              <div>Quantity</div>           
              <div className="Actions">
                <button className="Button-icon Empty"></button>
                <button className="Button-icon Empty"></button>             
              </div>              
            </div>
            {orderModel.items.map((item, i) => { return (
              <div key={item.id} className="Table-row">
                <div className="Small-col">{i+1}.</div>
                <div>{item.name}</div>                
                <div>{item.price} €</div>
                <div>{item.quantity}</div>
                <div className="Actions">
                  <div>
                    <button className="Button-icon" title="Decrease" onClick={this.handleDecreaseClick.bind(this, item)} disabled={this.state.openedModal !== "" || item.quantity <= 1}>
                      <Icon icon={"remove"}/>
                    </button>                      
                    <button className="Button-icon" title="Increase" onClick={this.handleIncreaseClick.bind(this, item)} disabled={this.state.openedModal !== ""}>
                      <Icon icon={"add"}/>
                    </button>
                    <button className="Button-icon" title="Delete" onClick={this.handleDeleteClick.bind(this, item)} disabled={this.state.openedModal !== ""}>
                      <Icon icon={"delete"}/>
                    </button>
                  </div>
                </div>
              </div>
              ); })
            }
            </div>
            <div className="Table-row Table-footer">
              <div className="Small-col"></div>
              <div></div>
              <div>{sumOrder(orderModel)} €</div>
              <div>{countItems(orderModel)}</div>           
              <div className="Actions">
                <button className="Button-icon Empty"></button>
                <button className="Button-icon Empty"></button>
                <button className="Button-icon Empty"></button>         
              </div>              
            </div>
          </div>      
        </div>
      </div>
    );
  };
}