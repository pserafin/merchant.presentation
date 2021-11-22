import React from 'react';
import { toast } from 'react-toastify';
import Icon from './Icon'
import InfoModal from './InfoModal';
import 'react-toastify/dist/ReactToastify.css';
import '../App.scss';
import { createEmptyOrder, updateOrderItem, deleteOrderItem, deepClone, createEmptyOrderState, createValidationModel } from '../actions/creations';
import { isInternalError, isForbidden, isNotAccepted } from '../actions/verifications'
import { countItems, sumOrder } from '../actions/calculations';
import { formatDate, parseStatus } from '../actions/formattings';

const axios = require('axios');
const orderStatuses = [1,2,3,4,5,6,7];

export default class OrderModal extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
        orderModel: createEmptyOrder(),
        modified: false,
        openedModal: "",
        messages: [],
    }
    this.onInfoModalClose = this.onInfoModalClose.bind(this);
    this.onClose = this.onClose.bind(this);
    this.onSave = this.onSave.bind(this);
    this.handleStatusChange = this.handleStatusChange.bind(this); 
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
    this.setState({orderModel: deepClone(this.props.model), modified: false});
  };

  onClose() {
    this.props.onModalClose(false);
  }  

  onSave() {
    this.setState({showSpinner: true});
    axios.put(process.env.REACT_APP_API_URL + 'order/update', this.state.orderModel)
    .then(x => {
      this.setState(createEmptyOrderState());
      this.props.onModalClose(true);
      toast.success("Your order has updated");
    })
    .catch(error => {
      this.setState(createEmptyOrderState());
      if (isInternalError(error) || isForbidden(error)) {
        toast.error(error.response.data.message);
      } else if (isNotAccepted(error)) {
        const errors = createValidationModel(error.response.data.message);
        this.showValidationMessage(errors);
      }
    });  
  }


  showValidationMessage(data) {
    if(!data) {
        this.setState(createEmptyOrderState());
        return;
    } else if(data.isValid) {
        this.setState(createEmptyOrderState());
        toast.success("Order is valid");
        return;
  }

  this.setState({openedModal: "info", messages: data.messages});
    toast.warning("Order did not pass validation");
  }

  onInfoModalClose() {
    this.setState(createEmptyOrderState());    
  }

  handleStatusChange(event){
      const order = deepClone(this.props.model)
      order.status = parseInt(event.target.value, 10);
      this.setState({orderModel: order, modified: true});  
  }

  render() {
    const orderModel = this.state.orderModel;
    const hideModalContainer = this.state.openedModal === "" || this.state.openedModal === "_";
    const saveDisabled = this.state.openedModal !== "" || !this.state.modified;

    return (
    <div className="OpenedModal">
      <button className="Close-button" onClick={this.onClose}><Icon icon={"close"}/></button>    
      <div className={hideModalContainer ? "Modal Fixed Hidden" : "Modal Fixed Visible"}>
        {this.state.openedModal === "info" &&
          <InfoModal model={this.state.messages} onModalClose={this.onInfoModalClose}/>
        }    
      </div>  
      <div className={this.state.openedModal !== "" ? "Blured" : ""}>
        <h1>Manage order</h1>
        <div className="Table Fixed">
          <div className="No-border Action-panel Info">
            <div className="Info-item">
                <div>Order date</div>
                <div><b>{formatDate(orderModel.date)}</b></div>
            </div>
            <div className="Info-item">
              <div>User</div>
              <div><b>{orderModel.userName}</b></div>  
            </div> 
            <div className="Info-item">
              <div>Status</div>
              <div>
                <select className="App-select" onChange={this.handleStatusChange} value={orderModel.status}>
                  {orderStatuses.map(i => <option key={i} value={i}>{parseStatus(i)}</option>) 
                  }
                </select>   
              </div>
            </div>                         
          </div>            
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
          <div className="ButtonPanel">
            <button onClick={this.onSave} disabled={saveDisabled}>                
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
    </div>
    );
  };
}