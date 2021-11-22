export function formatDate(date, withSecs){
    if(!date) {
        return "";
    }
    const finalDate = date.replace("T"," ").split(".")[0]

    return withSecs
            ? finalDate
            : finalDate.slice(0,-3);    
}

export function parseStatus(status) {
    switch(status) {
        case 1:
            return "new";
        case 2:
            return "validated";
        case 3:
            return "payment registered";
        case 4:
            return "paid";
        case 5:
            return "rejected";
        case 6:
            return "resigned";     
        case 7:
            return "canceled";                                                         
        default:
            return "undefined"
    }
}
