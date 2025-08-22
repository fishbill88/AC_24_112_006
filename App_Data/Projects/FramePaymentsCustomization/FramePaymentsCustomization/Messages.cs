using PX.Common;

namespace FramePaymentsIntegration
{
    /// <summary>
    /// Contains all localized message constants for Frame Payments integration
    /// </summary>
    [PXLocalizable]
    public static class Messages
    {
        #region Action Display Names
        public const string ProcessFramePayment = "Process Frame Payment";
        #endregion

        #region Plugin Information
        public const string PluginDisplayName = "Frame Payments";
        #endregion

        #region Validation Messages
        public const string FramePaymentsOnlySupportsPaymentAndPrepayment = "Frame Payments can only process Payment and Prepayment documents.";
        public const string PaymentAmountMustBeGreaterThanZero = "Payment amount must be greater than zero.";
        public const string NoProcessingCenterConfigured = "No processing center configured for this payment method.";
        public const string ProcessingCenterNotConfiguredForFramePayments = "Selected processing center is not configured for Frame Payments.";
        public const string FramePaymentProcessingFailed = "Frame Payment processing failed: {0}";
        public const string FrameApiKeyNotConfigured = "Frame API Key is not configured in the processing center.";
        public const string FramePaymentFailed = "Frame Payment failed: {0}";
        public const string ErrorProcessingFramePayment = "Error processing Frame Payment: {0}";
        #endregion

        #region Success Messages
        public const string PaymentProcessedSuccessfullyThroughFrame = "Payment processed successfully through Frame Payments";
        public const string FramePaymentProcessedSuccessfully = "Frame Payment processed successfully.\nTransaction ID: {0}\nAuth Code: {1}";
        #endregion

        #region Warning Messages
        public const string PaymentMethodConfiguredForFramePayments = "This payment method is configured to use Frame Payments processing.";
        #endregion

        #region Plugin Configuration
        public const string FrameApiKey = "Frame API Key";
        public const string ApiKey = "API Key";
        public const string Environment = "Environment";
        public const string ApiUrl = "API URL";
        public const string WebhookUrl = "Webhook URL";
        public const string TimeoutSeconds = "Timeout (Seconds)";
        public const string Sandbox = "Sandbox";
        public const string Production = "Production";
        #endregion

        #region Plugin Validation
        public const string FrameApiKeyRequired = "Frame API Key is required.";
        public const string FramePaymentsApiKeyRequired = "Frame Payments API Key is required.";
        public const string FrameApiKeyRequiredForTesting = "Frame API Key is required for credential testing.";
        public const string InvalidApiKeyFormat = "Invalid API Key format.";
        public const string FrameApiConnectionTestFailed = "Frame API connection test failed: {0}";
        public const string EnvironmentMustBeSandboxOrProduction = "Environment must be either 'Sandbox' or 'Production'.";
        public const string ApiUrlMustBeValid = "API URL must be a valid URL.";
        public const string TimeoutMustBePositiveNumber = "Timeout must be a positive number.";
        #endregion

        #region Transaction Processing
        public const string TransactionTypeNotSupported = "Transaction type {0} is not supported.";
        #endregion

        #region API Response Messages
        public const string TransactionApproved = "Transaction approved";
        public const string CaptureApproved = "Capture approved";
        public const string VoidApproved = "Void approved";
        public const string RefundApproved = "Refund approved";
        public const string ApiError = "API Error: {0}";
        public const string CaptureError = "Capture Error: {0}";
        public const string VoidError = "Void Error: {0}";
        public const string RefundError = "Refund Error: {0}";
        #endregion

        #region Default Values
        public const string DefaultPaymentDescription = "Payment";
        public const string DefaultCurrency = "USD";
        public const string DefaultApiUrl = "https://api.framepayments.com/v1/";
        public const string DefaultSandboxApiUrl = "https://sandbox-api.framepayments.com/v1/";
        public const string PlaceholderApiKey = "your_frame_api_key_here";
        public const string DefaultEnvironment = "Sandbox";
        public const string EmptyString = "";
        #endregion

        #region Transaction Types
        public const string TransactionTypeAuthorize = "authorize";
        public const string TransactionTypeSale = "sale";
        #endregion

        #region Payment Description Templates
        public const string PaymentDescriptionTemplate = "Payment {0}";
        #endregion

        #region Setting IDs
        public const string SettingApiKey = "API_KEY";
        public const string SettingApiUrl = "API_URL";
        public const string SettingEnvironment = "FRAME_ENVIRONMENT";
        public const string SettingWebhookUrl = "FRAME_WEBHOOK_URL";
        public const string SettingTimeoutSeconds = "FRAME_TIMEOUT";
        #endregion

        #region Class Names
        public const string PluginTypeName = "FramePaymentsIntegration.FrameProcessingCenterPlugin";
        #endregion

        #region Numeric Constants
        public const int DefaultTimeoutSeconds = 30;
        #endregion
    }
}