using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Services
{
    public static class TemplateService
    {
        public static string GetBirthdayPersonalizedHtmlMessage(string recipientName, string imageURL)
        {
            return $@"
                <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                            }}
                        </style>
                    </head>
                    <body>
                        <p>Dear {recipientName},</p>
                        <img src='{imageURL}' alt='Birthday Image' />
                        <p>Best regards,</p>
                    </body>
                </html>
            ";
        }
    }
}
