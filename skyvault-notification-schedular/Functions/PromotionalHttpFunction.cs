using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using skyvault_notification_schedular.Data;
using skyvault_notification_schedular.Helpers;
using skyvault_notification_schedular.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace skyvault_notification_schedular.Functions
{
    public class PromotionalHttpFunction(
        ILogger<PromotionalHttpFunction> logger,
        ICustomerRepository customerRepository,
        ITemplateRepository templateRepository,
        IEmailService emailService)
    {
        private readonly ILogger<PromotionalHttpFunction> _logger = logger;

        [Function("PromotionalHTTPFunction")]
        [Authorize]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("PromotionalHTTPFunction called");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };

                var requestBodyDeserialized = JsonSerializer.Deserialize<PromotionalRequest>(requestBody, options);

                if (requestBodyDeserialized == null || requestBodyDeserialized.TemplateId == 0 || requestBodyDeserialized.PromotionType == 0)
                {
                    _logger.LogError("Invalid request: TemplateId is 0 or PromotionType 0.");
                    return new BadRequestObjectResult("Invalid request: No values received for TemplateId or PromotionType.");
                }

                var promotion = await templateRepository.GetPromotionContent(requestBodyDeserialized.TemplateId);

                if (promotion == null || string.IsNullOrEmpty(promotion.Content))
                {
                    _logger.LogError("Invalid request: Promotion or content not found.");
                    return new BadRequestObjectResult("Invalid request: Promotion not found.");
                }

                // Content is stored in the database as a string with the format "email subject | email body"
                var content = promotion.Content.Split('|');
                if (content.Length < 2)
                {
                    _logger.LogError("Invalid request: Promotion content format is incorrect.");
                    return new BadRequestObjectResult("Invalid request: Promotion content format is incorrect.");
                }
                promotion.Content = content[1];

                var recipients = await customerRepository.GetCustomersForPromotion(requestBodyDeserialized.PromotionType);

                if (recipients == null || recipients.Count == 0)
                {
                    _logger.LogInformation("No customers found for promotion.");
                    return new OkObjectResult("No customers found for promotion.");
                }

                recipients.ForEach(recipient => recipient.SetPromotionEmailBody(promotion));

                await emailService.SendEmailAsync(recipients, content[0]);

                return new OkObjectResult($"Request received with template id: {requestBodyDeserialized.TemplateId} and promotion type: {requestBodyDeserialized.PromotionType}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid request: Unable to deserialize the request body.");
                return new BadRequestObjectResult("Invalid request: Unable to deserialize the request body.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return new BadRequestObjectResult("An unexpected error occurred.");
            }
        }

        [Function("GetAccountInformtaionFunction")]
        public async Task<IActionResult> GetAccountInformtaionFunction([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {

            try
            {
                _logger.LogInformation("GetAccountInformtaionFunction called");

                var accountInfo = await emailService.GetAccountInfomationAsync();

                return new OkObjectResult(new { data = accountInfo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return new BadRequestObjectResult("An unexpected error occurred.");
            }
        }

        [Function("Unsubscribe")]
        public async Task<IActionResult> Unsubscribe([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("Unsubscribe called");
            try
            {
                var query = HttpUtility.ParseQueryString(req.Url.Query);

                string? token = query["token"];

                if (string.IsNullOrEmpty(token) || !token.Contains('|'))
                {
                    _logger.LogError("Invalid request: Token not found.");
                    return new BadRequestObjectResult("Invalid request: Token not found.");
                }

                string[] parts = token.Split('|');

                if (parts.Length != 2)
                {
                    return new BadRequestObjectResult("Invalid token format.");
                }

                string email = HttpUtility.UrlDecode(parts[0]);
                string receivedSignature = parts[1];

                if (receivedSignature != TokenService.GenerateToken(email))
                {
                    return new UnauthorizedResult();
                }

                var unsubscribe = customerRepository.UnsubscribeEmail(email);


                return new OkObjectResult("You have been unsubscribed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return new BadRequestObjectResult("An unexpected error occurred.");
            }
        }

    }
}
