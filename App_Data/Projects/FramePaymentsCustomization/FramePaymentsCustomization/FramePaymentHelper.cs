using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PX.Data;

namespace FramePaymentsIntegration
{
    /// <summary>
    /// Frame Payments API Client
    /// </summary>
    public class FrameApiClient
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public FrameApiClient(string apiKey, string baseUrl = null)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _baseUrl = baseUrl?.TrimEnd('/') + "/" ?? Messages.DefaultApiUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public FramePaymentResponse ProcessPayment(FramePaymentRequest request)
        {
            return ProcessPaymentAsync(request).GetAwaiter().GetResult();
        }

        public FramePaymentResponse CapturePayment(string transactionId, decimal? amount)
        {
            return CapturePaymentAsync(transactionId, amount).GetAwaiter().GetResult();
        }

        public FramePaymentResponse VoidPayment(string transactionId)
        {
            return VoidPaymentAsync(transactionId).GetAwaiter().GetResult();
        }

        public FramePaymentResponse RefundPayment(string transactionId, decimal? amount)
        {
            return RefundPaymentAsync(transactionId, amount).GetAwaiter().GetResult();
        }

        private async Task<FramePaymentResponse> ProcessPaymentAsync(FramePaymentRequest request)
        {
            try
            {
                var endpoint = request.TransactionType == Messages.TransactionTypeAuthorize ? Messages.TransactionTypeAuthorize : Messages.TransactionTypeSale;
                var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}payments/{endpoint}", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var frameResponse = JsonConvert.DeserializeObject<FrameApiSuccessResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = true,
                        TransactionId = frameResponse.TransactionId,
                        AuthorizationCode = frameResponse.AuthorizationCode,
                        ResponseMessage = Messages.TransactionApproved,
                        RawResponse = responseText
                    };
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<FrameApiErrorResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = false,
                        ResponseMessage = errorResponse?.Message ?? response.ReasonPhrase,
                        ErrorCode = errorResponse?.ErrorCode,
                        RawResponse = responseText
                    };
                }
            }
            catch (Exception ex)
            {
                return new FramePaymentResponse
                {
                    IsSuccess = false,
                    ResponseMessage = string.Format(Messages.ApiError, ex.Message),
                    RawResponse = ex.ToString()
                };
            }
        }

        private async Task<FramePaymentResponse> CapturePaymentAsync(string transactionId, decimal? amount)
        {
            try
            {
                var request = new { amount = amount };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}payments/{transactionId}/capture", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var frameResponse = JsonConvert.DeserializeObject<FrameApiSuccessResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = true,
                        TransactionId = frameResponse.TransactionId,
                        AuthorizationCode = frameResponse.AuthorizationCode,
                        ResponseMessage = Messages.CaptureApproved,
                        RawResponse = responseText
                    };
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<FrameApiErrorResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = false,
                        ResponseMessage = errorResponse?.Message ?? response.ReasonPhrase,
                        ErrorCode = errorResponse?.ErrorCode,
                        RawResponse = responseText
                    };
                }
            }
            catch (Exception ex)
            {
                return new FramePaymentResponse
                {
                    IsSuccess = false,
                    ResponseMessage = string.Format(Messages.CaptureError, ex.Message),
                    RawResponse = ex.ToString()
                };
            }
        }

        private async Task<FramePaymentResponse> VoidPaymentAsync(string transactionId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}payments/{transactionId}/void", null);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var frameResponse = JsonConvert.DeserializeObject<FrameApiSuccessResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = true,
                        TransactionId = frameResponse.TransactionId,
                        ResponseMessage = Messages.VoidApproved,
                        RawResponse = responseText
                    };
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<FrameApiErrorResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = false,
                        ResponseMessage = errorResponse?.Message ?? response.ReasonPhrase,
                        ErrorCode = errorResponse?.ErrorCode,
                        RawResponse = responseText
                    };
                }
            }
            catch (Exception ex)
            {
                return new FramePaymentResponse
                {
                    IsSuccess = false,
                    ResponseMessage = string.Format(Messages.VoidError, ex.Message),
                    RawResponse = ex.ToString()
                };
            }
        }

        private async Task<FramePaymentResponse> RefundPaymentAsync(string transactionId, decimal? amount)
        {
            try
            {
                var request = new { amount = amount };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}payments/{transactionId}/refund", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var frameResponse = JsonConvert.DeserializeObject<FrameApiSuccessResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = true,
                        TransactionId = frameResponse.TransactionId,
                        ResponseMessage = Messages.RefundApproved,
                        RawResponse = responseText
                    };
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<FrameApiErrorResponse>(responseText);
                    return new FramePaymentResponse
                    {
                        IsSuccess = false,
                        ResponseMessage = errorResponse?.Message ?? response.ReasonPhrase,
                        ErrorCode = errorResponse?.ErrorCode,
                        RawResponse = responseText
                    };
                }
            }
            catch (Exception ex)
            {
                return new FramePaymentResponse
                {
                    IsSuccess = false,
                    ResponseMessage = string.Format(Messages.RefundError, ex.Message),
                    RawResponse = ex.ToString()
                };
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    #region Data Models

    public class FramePaymentRequest
    {
        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("transaction_type")]
        public string TransactionType { get; set; }

        [JsonProperty("customer")]
        public FrameCustomerInfo CustomerInfo { get; set; }

        [JsonProperty("payment_method")]
        public FramePaymentMethod PaymentMethod { get; set; }

        [JsonProperty("billing_address")]
        public FrameAddressInfo BillingAddress { get; set; }
    }

    public class FrameCustomerInfo
    {
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }

    public class FramePaymentMethod
    {
        [JsonProperty("card_number")]
        public string CardNumber { get; set; }

        [JsonProperty("expiration_month")]
        public string ExpirationMonth { get; set; }

        [JsonProperty("expiration_year")]
        public string ExpirationYear { get; set; }

        [JsonProperty("security_code")]
        public string SecurityCode { get; set; }

        [JsonProperty("cardholder_name")]
        public string CardHolderName { get; set; }
    }

    public class FrameAddressInfo
    {
        [JsonProperty("address_line1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("address_line2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class FramePaymentResponse
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string AuthorizationCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ErrorCode { get; set; }
        public string RawResponse { get; set; }
    }

    public class FrameApiSuccessResponse
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class FrameApiErrorResponse
    {
        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }
    }

    #endregion
}