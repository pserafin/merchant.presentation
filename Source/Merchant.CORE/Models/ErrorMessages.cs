namespace Merchant.CORE.Models
{
    public class ErrorMessages
    {
        public const string RetrievalError = "Application encountered an issue during {0} retrieval";
        public const string ModificationError = "Application was unable to {0} {1}";
        public const string ProductDeleteError = "Product cannot be deleted because is ordered by customer";
        public const string GeneralValidationError = "Application encountered an issue during cart validation";
        public const string PriceChangedError = "{0} price has changed. The current price is {1}";
        public const string ProductNoLongerAvailableError = "{0} is no longer available in store";
        public const string ProductTemporarilyAvailableError = "{0} is temporarily unavailable";
        public const string NotEnoughProductsError = "The selected quantity of {0} exceeds quantity available in stock";
        public const string OrderModificationForbiden = "Operation is not allowed for order is in status {0}";
        public const string OrderStatusSetError = "System was unable to update your order status";
    }
}
