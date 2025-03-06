using skyvault_notification_schedular.Data;
using skyvault_notification_schedular.Helpers;
using System.Web;

namespace skyvault_notification_schedular.Models;
public class Recipient
{
    public string Name { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string VisaCountry { get; set; } = String.Empty;
    public string EmailBody { get; set; } = String.Empty;

    private const string HtmlTemplate = @"
        <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        width: 100%;
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #ffffff;
                        padding: 20px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }}
                    .content {{
                        line-height: 1.6;
                    }}
                    .footer {{
                        margin-top: 20px;
                        font-size: 0.9em;
                        color: #888888;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='content'>
                        <p>Dear {0},</p>
                        {1}
                        <p>Best regards,</p>
                    </div>
                    <div class='footer'>
                        <p>This is an automated message, please do not reply.</p>
                        {2}
                    </div>
                </div>
            </body>
        </html>
    ";

    public void SetBirthdayEmailBody(string imageURL)
    {
        string content = $"<img src='{imageURL}' alt='Birthday Image' />";
        EmailBody = string.Format(HtmlTemplate, Name, content, GetUnsubscribeLink());
    }

    public void SetPassportOrVisaEmailBody(string content)
    {
        EmailBody = string.Format(HtmlTemplate, Name, $"<p>{content}</p>", GetUnsubscribeLink());
    }

    public void SetPromotionEmailBody(EmailContent emailContent)
    {
        string imageTag = string.IsNullOrEmpty(emailContent.File) ? "" : $"<img src='{emailContent.File}' alt='Oportunity Image' />";
        string contentBody = string.IsNullOrEmpty(emailContent.Content) ? "" : $"<p>{emailContent.Content}</p>";

        EmailBody = string.Format(HtmlTemplate, Name, $"{imageTag}<br/>{contentBody}", GetUnsubscribeLink());
    }

    private string GetUnsubscribeLink() 
    {
        string token = TokenService.GenerateToken(Email);
        string url = Environment.GetEnvironmentVariable("AZURE_FUNCTION_HOSTED_URL");

        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException("Azure Function hosted URL not found in environment variables.");
        }

        string unsubscribeLink = @"<p style=""font-size: 12px; text-align: center; color: #999;"">
                                You are receiving this email because you subscribed to our updates.
                                <a href=""{0}/api/Unsubscribe?token={1}"" 
                                   style=""color: #aaa; text-decoration: none;"">
                                   Unsubscribe
                                </a>
                            </p>";

        string emailWithSignature = String.Concat(HttpUtility.UrlEncode(Email), '|', token);

        return String.Format(unsubscribeLink, url, emailWithSignature);
    }
}

