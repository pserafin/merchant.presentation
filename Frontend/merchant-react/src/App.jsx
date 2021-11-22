import React from 'react';
import logo from './logo.svg';
import './App.scss';
import Login from './components/Login'
import TopBar from './components/TopBar'
import Products from './components/Products';
import Cart from './components/Cart';
import Orders from './components/Orders';
import { ToastContainer } from 'react-toastify';
import { toast } from 'react-toastify';
import { isInternalError, isForbidden, isAdmin, isCustomer} from './actions/verifications'
import { countItems } from './actions/calculations';

const axios = require('axios');
axios.defaults.withCredentials = true;


export default class App extends React.Component  {
  constructor(props) {
    super(props);
    this.state = {
        isLogged: null,
        user: null,
        cartItems: 0,
        modalOpen: false,
        order: null,
        route: "products"
    }
    this.setComponentState = this.setComponentState.bind(this);
    this.setLoginState = this.setLoginState.bind(this);
  };

  setComponentState(value) {
    this.setState(value);
  };

  setLoginState(value) {
    this.setState(value);
    if(value.isLogged) {
      this.getOpenOrder();
    }
  };

  getOpenOrder() {
    axios.get(process.env.REACT_APP_API_URL + 'order/current')
    .then(x => {
      this.setState({order: x.data, cartItems: countItems(x.data)});
    })
    .catch(error => {
      this.setState({order: null, cartItems: 0});
      if (isInternalError(error) || isForbidden(error)) {
        toast.error(error.response.data.message);
      }
    });        
}

  componentDidMount() {
    axios.get(process.env.REACT_APP_API_URL + 'account/get')
    .then(x => {
      this.setState({isLogged: true, user: {name: x.data.name, roles: x.data.roles}, order: null, cartItems: 0});
      this.getOpenOrder();
    })
    .catch(() => {
      this.setState({isLogged: false, user: null, order: null, cartItems: 0});
    });
  };

  render() {
    const model = this.state;

    return (
      <div className="App"> 
        <header className="App-header">
          {model.isLogged &&
            <div className="Min-width">
              <TopBar actionLogout={this.setLoginState} navigation={this.setComponentState} model={model}/>
              {model.route === "products" && 
                <Products action={this.setComponentState} model={model}/>
              }
              {model.route === "cart" && isCustomer(model) &&
                <Cart action={this.setComponentState} model={model}/>
              }
              {model.route === "orders" && isAdmin(model) &&
                <Orders action={this.setComponentState} model={model}/>
              }              
            </div>   
          }      
          {model.isLogged !== null && !model.isLogged &&
            <div>
              <img src={logo} className="App-logo" alt="logo" />
              <Login action={this.setLoginState}/>
            </div>
          }
        </header>
        <ToastContainer position="bottom-right" autoClose={2000} hideProgressBar={false} newestOnTop={false} closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover className="DarkToast" />
      </div>
    );
  }
}
