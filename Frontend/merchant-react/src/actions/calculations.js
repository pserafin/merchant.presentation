export function countItems(order) {
    var counter = 0;
    
    if(order && order.items) {
        order.items.forEach(x => {
            counter += x.quantity;
        });
    }

    return counter;
}

export function sumOrder(order) {
    var sum = 0.0;
    
    if(order && order.items) {
        order.items.forEach(x => {
            sum = fixRounding(x.quantity * x.price + sum);
        });
    }

    return sum;
}

export function sumOrders(orders) {
    var sum = 0.0;
    
    if(orders && orders.length > 0) {
        orders.forEach(x => {
            sum = fixRounding(x.totalPrice + sum);
        });
    }

    return sum;
}

export function paidPercentage(orders) {
    var percentage = 0;

    if(orders && orders.length > 0) {
        var sum = orders.filter(x => x.status === 4).length;
        percentage = fixRounding(sum / orders.length * 100);
    }

    return percentage;
}

export function fixRounding(value) {
    var power = Math.pow(10, 2);
    return Math.round(value * power) / power;
}