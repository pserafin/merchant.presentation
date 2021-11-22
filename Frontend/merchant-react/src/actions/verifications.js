
export function isInternalError(error) {
    return error.response && error.response.data && error.response.data.statusCode === 500
}

export function isForbidden(error) {
    return error.response && error.response.data && error.response.data.statusCode === 403
}

export function isNotAccepted(error) {
    return error.response && error.response.data && error.response.data.statusCode === 406
}

export function validateProduct(model) {
    if(!model ||
      (!model.name || model.name.length < 1) ||
      ((!model.quantity && model.quantity !==0) || isNaN(Number(model.quantity)) || parseInt(model.quantity, 10) < 0) ||
      ((!model.price && model.price !==0) || isNaN(Number(model.price)) || parseFloat(model.price) < 0) ||
      (model.isEnabled !== false && model.isEnabled !== true)) {
        return false;
    }
    return true;
}

export function isLogged(model) {
    return model && model.isLogged;
}

export function isCustomer(model) {
    return isLogged(model) && model.user.roles.includes('Customer');
}

export function isAdmin(model) {
    return isLogged(model) && model.user.roles.includes('Administrator');
}

export function isCurrentModule(props, href) {
    if(!props || !props.route) {
        return false
    }
    return props.route === href;
}