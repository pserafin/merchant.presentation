import React from 'react';
import '../App.scss'
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const axios = require('axios');

export default class Login extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      login: '',
      password: ''
    }
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleButtonClick = this.handleButtonClick.bind(this);
  };

  handleInputChange(event) {
    this.setState({[event.target.name]: event.target.value});
  };

  handleButtonClick() {
    const model = {name: this.state.login, password: this.state.password};

    axios.post(process.env.REACT_APP_API_URL + 'account/login', model, 
    {
      credentials: 'include'
    })
    .then(response => {
      toast.success("Welocome back " + response.data.name);
      this.props.action({isLogged: true, user: {name: response.data.name, roles: response.data.roles}, order: null});
    })
    .catch(error =>  {
      const message = error.response && error.response.status === 401 
        ? 'The username or password is incorrect' 
        : 'Login operation failed. Please try again later';
        toast.error(message);
        this.props.action({isLogged: false, user: null, order: null});
    })
  }

  render() {
  return (
    <div className="App-form-login">            
      <span>Username:</span>
      <input className="App-input" type="text" required name="login" onChange={this.handleInputChange} ></input>
      <span>Password:</span>
      <input className="App-input" type="password" required name="password" onChange={this.handleInputChange} ></input>
      <button className="App-button" onClick={this.handleButtonClick} disabled={!this.state.login || !this.state.password}>Login</button>
    </div>
    );
  };
}