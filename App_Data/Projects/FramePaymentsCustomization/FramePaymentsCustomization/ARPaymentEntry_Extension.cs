using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.AR.CCPaymentProcessing;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.CA;
using PX.CCProcessingBase.Interfaces.V2;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;

namespace FramePaymentsIntegration
{
    /// <summary>
    /// Extension for ARPaymentEntry to integrate with Frame Payments
    /// </summary>
    public class ARPaymentEntry_Extension : PXGraphExtension<ARPaymentEntry>
    {
        public static bool IsActive() => true;

        #region Actions

        public PXAction<ARPayment> ProcessFramePayment;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = Messages.ProcessFramePayment, Enabled = false)]
        protected virtual void processFramePayment()
        {
            var payment = Base.Document.Current;
            if (payment == null) return;

            try
            {
                // Validate payment can be processed
                if (payment.DocType != ARDocType.Payment && payment.DocType != ARDocType.Prepayment)
                {
                    throw new PXException(Messages.FramePaymentsOnlySupportsPaymentAndPrepayment);
                }

                if (payment.CuryOrigDocAmt <= 0)
                {
                    throw new PXException(Messages.PaymentAmountMustBeGreaterThanZero);
                }

                // Get the processing center
                var paymentMethod = PaymentMethod.PK.Find(Base, payment.PaymentMethodID);
                CCProcessingCenterPmntMethod processingCenterPmntMethod = SelectFrom<CCProcessingCenterPmntMethod>
                    .Where<CCProcessingCenterPmntMethod.paymentMethodID.IsEqual<@P.AsString>>
                    .View.Select(Base, payment.PaymentMethodID);
                if (processingCenterPmntMethod?.ProcessingCenterID == null)
                {
                    throw new PXException(Messages.NoProcessingCenterConfigured);
                }

                var processingCenter = CCProcessingCenter.PK.Find(Base, processingCenterPmntMethod?.ProcessingCenterID);
                if (processingCenter?.ProcessingTypeName != typeof(FrameProcessingCenterPlugin).FullName)
                {
                    throw new PXException(Messages.ProcessingCenterNotConfiguredForFramePayments);
                }

                // Process the payment through Frame
                ProcessPaymentThroughFrame(payment, processingCenter);
                
                Base.Persist();
                
                throw new PXRedirectRequiredException(Base, Messages.PaymentProcessedSuccessfullyThroughFrame);
            }
            catch (Exception ex)
            {
                throw new PXException(Messages.FramePaymentProcessingFailed, ex.Message);
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void ARPayment_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var payment = e.Row as ARPayment;
            if (payment == null) return;

            // Enable Frame payment button if appropriate conditions are met
            bool enableFramePayment = IsFramePaymentEnabled(payment);
            ProcessFramePayment.SetEnabled(enableFramePayment);
        }

        protected virtual void ARPayment_PaymentMethodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var payment = e.Row as ARPayment;
            if (payment == null) return;

            // Check if the new payment method uses Frame Payments
            CheckFramePaymentMethod(payment);
        }

        #endregion

        #region Helper Methods

        private bool IsFramePaymentEnabled(ARPayment payment)
        {
            if (payment == null) return false;
            
            // Check document type
            if (payment.DocType != ARDocType.Payment && payment.DocType != ARDocType.Prepayment)
                return false;

            // Check if payment is released or voided
            if (payment.Released == true || payment.Voided == true)
                return false;

            // Check if amount is valid
            if (payment.CuryOrigDocAmt <= 0)
                return false;

            // Check if payment method is Frame-enabled
            var paymentMethod = PaymentMethod.PK.Find(Base, payment.PaymentMethodID);
            CCProcessingCenterPmntMethod processingCenterPmntMethod = SelectFrom<CCProcessingCenterPmntMethod>
                    .Where<CCProcessingCenterPmntMethod.paymentMethodID.IsEqual<@P.AsString>>
                    .View.Select(Base, payment.PaymentMethodID);
            if (processingCenterPmntMethod?.ProcessingCenterID == null)
                return false;

            var processingCenter = CCProcessingCenter.PK.Find(Base, processingCenterPmntMethod?.ProcessingCenterID);
            if (processingCenter?.ProcessingTypeName != typeof(FrameProcessingCenterPlugin).FullName)
                return false;

            return true;
        }

        private void CheckFramePaymentMethod(ARPayment payment)
        {
            if (payment?.PaymentMethodID == null) return;

            var paymentMethod = PaymentMethod.PK.Find(Base, payment.PaymentMethodID);
            CCProcessingCenterPmntMethod processingCenterPmntMethod = SelectFrom<CCProcessingCenterPmntMethod>
                    .Where<CCProcessingCenterPmntMethod.paymentMethodID.IsEqual<@P.AsString>>
                    .View.Select(Base, payment.PaymentMethodID);
            if (processingCenterPmntMethod?.ProcessingCenterID == null) return;

            var processingCenter = CCProcessingCenter.PK.Find(Base, processingCenterPmntMethod?.ProcessingCenterID);
            if (processingCenter?.ProcessingTypeName == typeof(FrameProcessingCenterPlugin).FullName)
            {
                // This payment method uses Frame Payments
                PXUIFieldAttribute.SetWarning<ARPayment.paymentMethodID>(
                    Base.Document.Cache, 
                    payment, 
                    Messages.PaymentMethodConfiguredForFramePayments);
            }
        }

        private void ProcessPaymentThroughFrame(ARPayment payment, CCProcessingCenter processingCenter)
        {
            try
            {
                // Create Frame API client
                var settings = GetProcessingCenterSettings(processingCenter);
                var apiKey = GetSetting(settings, "ApiKey");
                var apiUrl = GetSetting(settings, "ApiUrl");
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new PXException(Messages.FrameApiKeyNotConfigured);
                }

                var frameClient = new FrameApiClient(apiKey, apiUrl);

                // Build payment request
                var customer = Customer.PK.Find(Base, payment.CustomerID);
                var contact = Contact.PK.Find(Base, payment.CustomerID);
                var paymentRequest = new FramePaymentRequest
                {
                    Amount = payment.CuryOrigDocAmt,
                    Currency = payment.CuryID ?? Base.Accessinfo.BaseCuryID,
                    Description = string.Format(Messages.PaymentDescriptionTemplate, payment.RefNbr),
                    TransactionType = Messages.TransactionTypeSale, // Authorize and capture
                    CustomerInfo = new FrameCustomerInfo
                    {
                        CustomerId = customer?.AcctCD,
                        Email = contact?.EMail,
                        FirstName = customer?.AcctName?.Split(' ')[0],
                        LastName = customer?.AcctName?.Contains(" ") == true ? 
                                   customer.AcctName.Substring(customer.AcctName.IndexOf(' ') + 1) : ""
                    }
                };

                // Process payment
                var response = frameClient.ProcessPayment(paymentRequest);

                if (response.IsSuccess)
                {
                    // Update payment with transaction details
                    payment.ExtRefNbr = response.TransactionId;
                    
                    // Add notes about the transaction
                    var note = string.Format(Messages.FramePaymentProcessedSuccessfully, 
                        response.TransactionId, response.AuthorizationCode);
                    PXNoteAttribute.SetNote(Base.Document.Cache, payment, note);
                    
                    Base.Document.Update(payment);
                }
                else
                {
                    throw new PXException(Messages.FramePaymentFailed, response.ResponseMessage);
                }
            }
            catch (Exception ex)
            {
                throw new PXException(Messages.ErrorProcessingFramePayment, ex.Message);
            }
        }

        private System.Collections.Generic.Dictionary<string, string> GetProcessingCenterSettings(CCProcessingCenter processingCenter)
        {
            var settings = new System.Collections.Generic.Dictionary<string, string>();
            
            // In a real implementation, you would retrieve settings from CCProcessingCenterDetail
            // For now, return basic settings
            settings["ApiKey"] = Messages.PlaceholderApiKey; // This should come from processing center configuration
            settings["ApiUrl"] = Messages.DefaultApiUrl;
            settings["ProcessingCenterID"] = processingCenter.ProcessingCenterID;
            
            return settings;
        }

        private string GetSetting(System.Collections.Generic.Dictionary<string, string> settings, string key)
        {
            settings.TryGetValue(key, out string value);
            return value;
        }

        #endregion
    }
}