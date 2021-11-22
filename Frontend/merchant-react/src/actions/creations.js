export function createOrderItem(model) {
    return {id: 0, productId: model.id, name: model.name, quantity: 1, price: model.price};
}

export function createEmptyOrder() {
    return {id: 0, date: new Date().toISOString(), userId: 0, totalPrice: 0.0, status: 1, items: []};
}

export function createOrder(order, model) {
    const newOrder = deepClone(order);
    newOrder.items.push(createOrderItem(model));

    return newOrder;
}

export function createNewProduct() {
    return {id: 0, name: undefined, quantity: undefined, price: undefined, isEnabled: true};
}

export function createEmptyProductState() {
    return {openedModal: "", selected: null, selectedId: null, showSpinner: false};
}

export function createNavigationState(route) {
    return {route: route, selected: null, selectedId: null, showSpinner: false};
}

export function createLogoutState() {
    return {route: "products", isLogged: false, user: null};
}

export function updateOrder(order, product) {
    const newOrder = deepClone(order);
    const existingProduct = newOrder.items.find(x => x.productId === product.id);
    if(existingProduct) {
        existingProduct.name = product.name;
        existingProduct.price = product.price;
        existingProduct.quantity += 1;
    } else {
        newOrder.items.push(createOrderItem(product));
    }

    return newOrder;
}

export function updateOrderItem(order, orderItem) {
    const newOrder = deepClone(order);
    const existingProduct = newOrder.items.find(x => x.id === orderItem.id);
    if(existingProduct) {
        existingProduct.name = orderItem.name;
        existingProduct.price = orderItem.price;
        existingProduct.quantity += orderItem.quantity;
    } else {
        newOrder.items.push({...orderItem});
    }

    return newOrder;
}

export function deleteOrderItem(order, orderItem) {
    const newOrder = deepClone(order);
    if(newOrder.items.some(x => x.id === orderItem.id)) {
        const index = newOrder.items.findIndex(x => x.id === orderItem.id);
        if (index !== -1) {
            newOrder.items.splice(index, 1);
          }
    } 

    return newOrder;
}

export function deepClone(model) {
    return JSON.parse(JSON.stringify(model));
}

export function createEmptyCartState(modified) {
    if(modified === true || modified === false) {
        return {openedModal: "", showSpinner: false, messages: [], modified: modified};
    }
    return {openedModal: "", showSpinner: false, messages: []};
}

export function createValidationModel(errors) {
    const messages = errors.split("\r\n");
    messages.pop();
    return {isValid: false, messages: messages};
}

export function createEmptyOrdersState(clearOrders) {
    if(clearOrders) {
        return {orders: [], openedModal: "", selected: null, selectedId: null, showSpinner: false};
    } else {
        return {openedModal: "", selected: null, selectedId: null, showSpinner: false};
    }
}

export function createEmptyOrderState(modified) {
    if(modified === true || modified === false) {
        return {openedModal: "", messages: [], modified: modified};
    }
    return {openedModal: "", messages: []};
}
