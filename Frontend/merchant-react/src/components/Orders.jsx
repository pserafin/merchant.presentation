import React from 'react';
import { toast } from 'react-toastify';
import Icon from './Icon'
import OrderModal from './OrderModal';
import { SpinnerCircular } from 'spinners-react';
import 'react-toastify/dist/ReactToastify.css';
import '../App.scss';
import { isInternalError, isForbidden, isAdmin } from '../actions/verifications'
import { createEmptyOrdersState, deepClone } from '../actions/creations';
import { formatDate, parseStatus } from '../actions/formattings';
import { sumOrders, paidPercentage } from '../actions/calculations';

const axios = require('axios');

export default class Orders extends React.Component {
  _isMounted = false;
  constructor(props) {
    super(props);
    this.state = {
        orders: [],
        selected: null,
        selectedId: null,
        openedModal: "",
        messages: [],
        showSpinner: false
    }
    this.onOrderModalClose = this.onOrderModalClose.bind(this);
  };

  componentDidMount() {
    this._isMounted = true;
    this.getOrders();
  };

  componentWillUnmount() {
    this._isMounted = true;
  }

  getOrders(message) {
    this.setState({showSpinner: true});
    axios.get(process.env.REACT_APP_API_URL + 'order/list')
    .then(x => {
      if(!this._isMounted) { 
          return; 
      }
      this.setState({orders: x.data, openedModal: "", selected: null, selectedId: null, showSpinner: false});
      if(message) {
        toast.success(message);
      }
    })
    .catch(error => {
      if(this._isMounted) { 
        this.setState(createEmptyOrdersState(true));
      }
      if (isInternalError(error) || isForbidden(error)) {
        toast.error(error.response.data.message);
      }
    });       
  }

  handleEditClick(item) {
    const model = deepClone(item);
    this.setState({openedModal: "order", selected: model, selectedId: model.id});
  }

  onOrderModalClose(saved) {
    if(saved) {
      this.getOrders();
    } else {
        this.setState(createEmptyOrdersState(false));
    }
  }

  render() {
    const model = this.props.model;
    const hideModalContainer = this.state.openedModal === "" || this.state.openedModal === "_";

    return (
      <div>
      <div className={this.state.showSpinner ? "SpinnerModal Visible" : "SpinnerModal Hidden"}>
        <SpinnerCircular enabled={this.state.showSpinner} thickness={200}/>                
      </div>  
      <div className={hideModalContainer ? "Modal Hidden" : "Modal Visible"}>
        {this.state.openedModal === "order" &&
          <OrderModal model={this.state.selected} onModalClose={this.onOrderModalClose}/>
        } 
      </div>  
      <div className={this.state.openedModal !== "" ? "Blured" : ""}>
        <h1>Orders history</h1>
        <div className="Table">
          <div className="Scrollable"> 
          <div className="Table-row Table-header">
            <div className="Small-col">No.</div>
            <div>Date</div>
            <div className="Center">User</div>
            <div>Price</div>
            <div>Status</div>
            <div className="Actions">                       
              {isAdmin(model) && 
                <button className="Button-icon Empty"></button>
              }                      
            </div>              
          </div>
          {this.state.orders.map((item, i) => { return (
            <div key={item.id} className="Table-row">
              <div className="Small-col">{i+1}.</div>
              <div>{formatDate(item.date)}</div>
              <div className="Center">{item.userName}</div>
              <div className="Bold">{item.totalPrice} €</div>
              <div>{parseStatus(item.status)}</div>
                <div className="Actions">                     
                  {isAdmin(model) && 
                    <div>
                      <button className="Button-icon" title="Manage" onClick={this.handleEditClick.bind(this, item)} disabled={this.state.openedModal !== ""}>
                        <Icon icon={"edit"}/>
                      </button>
                    </div>
                  }
                </div>
            </div>
            ); })
          }
          </div>
          <div className="Table-row Table-footer">
            <div className="Small-col"></div>
            <div></div>
            <div></div>
            <div>{sumOrders(this.state.orders)} €</div>
            <div>paid: {paidPercentage(this.state.orders)} %</div>           
            <div className="Actions">
            <button className="Button-icon Empty"></button>      
            </div>              
          </div>        
        </div>      
      </div>
    </div>
    );
  };
}