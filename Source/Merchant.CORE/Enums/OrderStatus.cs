namespace Merchant.CORE.Enums
{
    public enum OrderStatus
    {
        New = 1,
        //When order pass validation
        Validated = 2,
        //When payment is registered with Direct or scheduled with DirectLink
        PaymentRegistered = 3,
        //After payment being captured
        Paid = 4,
        //By payment system
        Rejected = 5,
        //User can resigned from order (from UI if applicable)
        Resigned = 6,
        //Only by administrator (from various reasons e.g. on customer request)
        Cancelled = 7
    }
}
