namespace PaymentGateway.Infrastructure;

public static class Constants
{
    public static class ApiScopes
    {
        public const string PaymentReadScope = "payment.read";
        public const string PaymentWriteScope = "payment.write";
    }

    public static class Policies
    {
        public const string PaymentReadPolicy = "PaymentReadScope";
        public const string PaymentWritePolicy = "PaymentWriteScope";
    }

    public static class ClaimsExtension
    {
        public const string MerchantId = "merchant_id";
    }
}