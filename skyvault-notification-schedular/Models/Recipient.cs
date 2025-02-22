using skyvault_notification_schedular.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Models;
public class Recipient
{
    public string Name { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string EmailBody { get; set; } = String.Empty;

    public void SetEmailBody(string imageURL)
    {
        EmailBody =  $@"
                <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                            }}
                        </style>
                    </head>
                    <body>
                        <p>Dear {Name},</p>
                        <img src='{imageURL}' alt='Birthday Image' />
                        <p>Best regards,</p>
                    </body>
                </html>
            ";
    }
}

