import React from 'react';
import { toast } from 'react-toastify';
import Icon from './Icon'
import ProductModal from './ProductModal';
import QuestionModal from './QuestionModal';
import { SpinnerCircular } from 'spinners-react';
import 'react-toastify/dist/ReactToastify.css';
import '../App.scss';
import { isInternalError, isForbidden, isAdmin, isCustomer } from '../actions/verifications'
import { createNewProduct, createOrder, createEmptyProductState, updateOrder } from '../actions/creations';
import { countItems } from '../actions/calculations';

const axios = require('axios');

export default class Products extends React.Component {
  _isMounted = false;
  constructor(props) {
    super(props);
    this.state = {
        products: [],
        openedModal: "",
        selected: null,
        selectedId: null,
        showSpinner: false
    }
    this.handleAddClick = this.handleAddClick.bind(this);
    this.onSelectedUpdate = this.onSelectedUpdate.bind(this);
    this.onProductModalClose = this.onProductModalClose.bind(this);
    this.onQuestionModalClose = this.onQuestionModalClose.bind(this);
  };

  componentDidMount() {
    this._isMounted = true;
    this.getProducts();
  };

  componentWillUnmount() {
    this._isMounted = true;
  }

  onSelectedUpdate(updated) {
    this.setState({selected: updated});
  };

  handleAddClick() {
    const model = createNewProduct();
    this.setState({openedModal: "product", selected: model, selectedId: model.id});
  };

  handleBuyClick(item) {
      const order = this.props.model.order;
      const model = {...item};
      this.setState({openedModal: "_", selected: model, selectedId: model.id, showSpinner: true});

      if(order.id === 0) {
          this.addOrder(order, model);
      } else {
          this.patchOrder(order, model);
      }
  };

  handleEditClick(item) {
      const model = {...item};
      this.setState({openedModal: "product", selected: model, selectedId: model.id});
  };

  handleDeleteClick(item) {
      const model = {...item};
      this.setState({openedModal: "delete", selected: model, selectedId: model.id});
  };

  onProductModalClose(save) {
      if(save) {
          const model = {
              ...this.state.selected, 
              price: parseFloat(this.state.selected.price),
              quantity: parseInt(this.state.selected.quantity, 10)
          };
          this.setState({openedModal: "_", showSpinner: true});
          if(this.state.selectedId === 0) {
              this.addProduct(model);
          } else {
              this.updateProduct(model);
          }
      } else {
          this.setState(createEmptyProductState());
      }        
  }

  onQuestionModalClose(confirm) {
      if(confirm) {
          const model = {...this.state.selected};
          this.setState({openedModal: "_", showSpinner: true});
          this.deleteProduct(model);
      } else {
          this.setState(createEmptyProductState());
      }
  }

  getProducts(message) {
    this.setState({showSpinner: true});
    axios.get(process.env.REACT_APP_API_URL + 'product/list')
    .then(x => {
      if(!this._isMounted) { return; }
      this.setState({products: x.data, openedModal: "", selected: null, selectedId: null, showSpinner: false});
      if(message) {
        toast.success(message);
      }
    })
    .catch(error => {
      if(!this._isMounted) { 
        this.setState(createEmptyProductState());
      }
      if (isInternalError(error) || isForbidden(error)) {
        toast.error(error.response.data.message);
      }
    });        
  }

  addProduct(model) {
    axios.post(process.env.REACT_APP_API_URL + 'product/add', model)
    .then(response => {
        this.getProducts("Product " + model.name + "has been added");

    })
    .catch(error =>  {
        this.setState(createEmptyProductState());
        if (isInternalError(error) || isForbidden(error)) {
            toast.error(error.response.data.message);
        }
    })
  }

  updateProduct(model) {
    axios.put(process.env.REACT_APP_API_URL + 'product/update', model)
    .then(response => {
        this.getProducts("Product " + model.name + "has been modified");

    })
    .catch(error =>  {
        this.setState(createEmptyProductState());
        if (isInternalError(error) || isForbidden(error)) {
            toast.error(error.response.data.message);
        }
    })
  }

  deleteProduct(model) {
    axios.delete(process.env.REACT_APP_API_URL + 'product/delete/' + model.id)
    .then(response => {
        this.getProducts("Product " + model.name + " has been removed");

    })
    .catch(error =>  {
        this.setState(createEmptyProductState());
        if (isInternalError(error) || isForbidden(error)) {
            toast.error(error.response.data.message);
        }
    })
  }

  addOrder(order, model) {
    const newOrder = createOrder(order, model);

    axios.post(process.env.REACT_APP_API_URL + 'order/add', newOrder)
    .then(response => {
        this.setState(createEmptyProductState());
        this.props.action({order: response.data, cartItems: countItems(response.data)});
        this.getProducts(model.name + " added to your cart")
    })
    .catch(error =>  {
        this.setState(createEmptyProductState());
        if (isInternalError(error) || isForbidden(error)) {
            toast.error(error.response.data.message);
        }
    });
  }

  patchOrder(order, model) {
    const newOrder = updateOrder(order, model);
    
    axios.put(process.env.REACT_APP_API_URL + 'order/update', newOrder)
    .then(response => {
        this.setState(createEmptyProductState());
        this.props.action({order: response.data, cartItems: countItems(response.data)})
        this.getProducts(model.name + " added to your cart")
    })
    .catch(error =>  {
        this.setState(createEmptyProductState());
        if (isInternalError(error) || isForbidden(error)) {
            toast.error(error.response.data.message);
        }
    });
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
          {this.state.openedModal === "product" &&
            <ProductModal model={this.state.selected} onUpdate={this.onSelectedUpdate} onModalClose={this.onProductModalClose}/>
          }
          {this.state.openedModal === "delete" &&
            <QuestionModal model={this.state.selected} onModalClose={this.onQuestionModalClose}/>
          }    
        </div>  
        <div className={this.state.openedModal !== "" ? "Blured" : ""}>
          <h1>Available products</h1>
          <div className="Table">
            {isAdmin(model) && 
              <div className="Table-row No-border Action-panel">
                <button className="Button-action" title="Add product" onClick={this.handleAddClick} disabled={this.state.openedModal !== ""}>
                  <Icon icon={"add"}/>
                  <span className="Text">Add product</span>
                </button>
              </div>
            }
            <div className="Scrollable"> 
            <div className="Table-row Table-header">
              <div>Name</div>
              <div>Quantity</div>
              <div>Price</div>
              {isAdmin(model) && 
                <div>For Sale</div>
              }
              <div className="Actions">
                {isCustomer(model) && 
                  <button className="Button-icon Empty"></button>
                }                            
                {isAdmin(model) && 
                  <div>
                    <button className="Button-icon Empty"></button>
                    <button className="Button-icon Empty"></button>
                  </div>
                }                      
              </div>              
            </div>
            {this.state.products.map((item, i) => {     
              return (
                <div key={item.id} className="Table-row">
                    <div>{item.name}</div>
                    <div>{item.quantity}</div>
                    <div>{item.price} €</div>
                    {isAdmin(model) && <div>{item.isEnabled 
                        ? <span>✔</span> 
                        : <span>❌</span> 
                    }
                    </div>}
                    <div className="Actions">
                      {isCustomer(model) && item.quantity > 0 && model.order && 
                          <button className="Button-icon" title="Buy" onClick={this.handleBuyClick.bind(this, item)}>
                            <Icon icon={"cartAdd"}/>
                          </button>
                      }                            
                      {isAdmin(model) && 
                        <div>
                          <button className="Button-icon" title="Edit" onClick={this.handleEditClick.bind(this, item)} disabled={this.state.openedModal !== ""}>
                            <Icon icon={"edit"}/>
                          </button>
                          <button className="Button-icon" title="Delete" onClick={this.handleDeleteClick.bind(this, item)} disabled={this.state.openedModal !== ""}>
                            <Icon icon={"delete"}/>
                          </button>
                        </div>
                      }
                    </div>
                </div>
                );
              })
            }
          </div>      
          </div>
        </div>
      </div>
    );
  };
}