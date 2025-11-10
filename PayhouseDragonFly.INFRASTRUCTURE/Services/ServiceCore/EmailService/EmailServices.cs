using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PayhouseDragonFly.CORE.ConnectorClasses.Response;
using PayhouseDragonFly.CORE.DTOs.EmaillDtos;
using PayhouseDragonFly.CORE.Models.Emails;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IEmailServices;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.EmailService
{
    public class EmailServices : IEmailServices
    {

        private readonly ILogger<IEmailServices> _logger;
        private readonly EmailConfiguration _emailconfig;
        private IServiceScopeFactory _scopefactory;

        public EmailServices(IOptions<EmailConfiguration> emailconfig, IServiceScopeFactory scopefactory,
            ILogger<IEmailServices> logger
            )
        {
            _emailconfig = emailconfig.Value;
            _logger = logger;
            _scopefactory = scopefactory;

        }

        public async Task<mailresponse> SendEmail(string mailText, string subject, string recipient)
        {
            try
            {
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                email.To.Add(MailboxAddress.Parse(recipient));
                email.Subject = subject;
                var builder = new BodyBuilder { HtmlBody = mailText };
                email.Body = builder.ToMessageBody();
                var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort), SecureSocketOptions.StartTls);
                var results = smtp.AuthenticateAsync(_emailconfig.EmailFrom, _emailconfig.SmtpPass);

                var resped = await smtp.SendAsync(email);

                _logger.LogInformation(" logging reponse : ", resped);
                await smtp.DisconnectAsync(true);

                return new mailresponse(true, "successfully sent email");
            }

            catch (Exception ex)
            {
                return new mailresponse(false, ex.Message);
            }
        }
        public async Task<mailresponse> SendEmailOnUserActivation(emailbody emailvm)
        {
            try
            {
                _logger.LogInformation($"[1] Activation email service started at {DateTime.Now}");

                var filePath = @"Templates/Email/user_activationemail.html";
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"[Error] Email template not found at: {filePath}");
                    return new mailresponse(false, "Email template not found.");
                }

                string template;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    template = await reader.ReadToEndAsync();
                }

                string emailContent = template
                    .Replace("{{userName}}", emailvm.UserName ?? "User")
                    .Replace("{{userEmail}}", emailvm.ToEmail ?? "")
                    .Replace("{{activationDate}}", DateTime.Now.ToString("dd/MM/yyyy"))
                    .Replace("{{payload}}", emailvm.PayLoad ?? "")
                    .Replace("{{password}}", emailvm.Password ?? "");

                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_emailconfig.SmtpUser);
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Your Account Has Been Activated";

                var builder = new BodyBuilder { HtmlBody = emailContent };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.CheckCertificateRevocation = false;

                try
                {
                    await smtp.ConnectAsync(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort), SecureSocketOptions.Auto);
                }
                catch
                {
                    await smtp.ConnectAsync(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort), SecureSocketOptions.SslOnConnect);
                }

                await smtp.AuthenticateAsync(_emailconfig.SmtpUser, _emailconfig.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("[✓] Activation email sent successfully.");
                return new mailresponse(true, "Activation email sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending activation email.");
                return new mailresponse(false, $"Failed to send activation email: {ex.Message}");
            }
        }



        public async Task<mailresponse> SenTestMail(emailbody emailvm)
        {
            try
            {
                var currentdate = DateTime.Now.ToString("dd/MM/yy");
                _logger.LogInformation($"_____________________1. email service started  at {DateTime.Now} _______________________________");
                var file = @"Templates/Email/sendtext.html";
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                StreamReader str = new StreamReader(file);
                string MailText = await str.ReadToEndAsync();
                str.Close();
                MailText = MailText.Replace("verificationstring", _emailconfig.SmtpUser)
                    .Replace("user", _emailconfig.SmtpUser)

                    .Replace("sentTime", Convert.ToString(DateTime.Now));
                var builder = new BodyBuilder { HtmlBody = MailText };
                email.Body = builder.ToMessageBody();
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Payhouse test mail";
                using var smtp = new SmtpClient();
                smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                var resp = await smtp.SendAsync(email);
                _logger.LogInformation("____________________ 3 email sender links ________________________________");
                smtp.Disconnect(true);
                _logger.LogInformation("____________________ 4 email sender links ________________________________");

                return new mailresponse(true, "mail sent successfully");
            }
            catch (Exception ex)
            {

                return new mailresponse(false, ex.Message);
            }
        }
        public async Task<mailresponse> SendEmailOnRegistration(emailbody emailvm)
        {
            try
            {
                _logger.LogInformation($"[1] Email registration service started at {DateTime.Now}");

                // Load the HTML email template
                var file = @"Templates/Email/EmailOnRegistration.html";
                if (!File.Exists(file))
                {
                    _logger.LogError($"[Error] Email template not found at: {file}");
                    return new mailresponse(false, "Email template not found.");
                }

                string mailText;
                using (StreamReader reader = new StreamReader(file))
                {
                    mailText = await reader.ReadToEndAsync();
                }

                // Replace placeholders in the template
                var datesent = DateTime.Now.ToString("dd/MM/yyyy");
                mailText = mailText.Replace("subject", emailvm.UserName ?? "")
                                   .Replace("user", _emailconfig.SmtpUser ?? "")
                                   .Replace("emailsentdate", datesent)
                                   .Replace("payload", emailvm.PayLoad ?? "");

                // Create the email
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_emailconfig.SmtpUser);
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Successful registration";

                var builder = new BodyBuilder { HtmlBody = mailText };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.CheckCertificateRevocation = false; // Helpful in dev

                _logger.LogInformation("[2] Attempting SMTP connection...");

                try
                {
                    await smtp.ConnectAsync(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort), SecureSocketOptions.StartTls);
                }
                catch (Exception exStartTls)
                {
                    _logger.LogWarning($"[2.1] StartTLS failed: {exStartTls.Message}. Trying SSL fallback...");
                    await smtp.ConnectAsync(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort), SecureSocketOptions.SslOnConnect);
                }

                _logger.LogInformation("[3] Authenticating...");
                await smtp.AuthenticateAsync(_emailconfig.SmtpUser, _emailconfig.SmtpPass);

                _logger.LogInformation("[4] Sending email...");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("[5] Email sent successfully.");
                return new mailresponse(true, "Mail sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending registration email.");
                return new mailresponse(false, $"Failed to send email: {ex.Message}");
            }
        }



        //send email on leave completion


        public async Task<mailresponse> SendEmailOnLeaveCompletion(EmailbodyOnLeaveEnd emailvm)
        {
            try
            {
                _logger.LogInformation($"_____________________1.  email starts {DateTime.Now} _______________________________");
                var file = @"Templates/Email/emailonleaveend.html";
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                StreamReader str = new StreamReader(file);
                string MailText = await str.ReadToEndAsync();
                str.Close();
                var datesent = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                MailText = MailText.Replace("subject", emailvm.UserName)
                    .Replace("user", _emailconfig.SmtpUser)
                    .Replace("Names", emailvm.Names)
                    .Replace("leaveEndDate", Convert.ToString(emailvm.LeaveEndDate));

                var builder = new BodyBuilder { HtmlBody = MailText };
                email.Body = builder.ToMessageBody();
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Successfull registration";
                using var smtp = new SmtpClient();
                smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                var resp = await smtp.SendAsync(email);
                _logger.LogInformation("____________________ 3 email sender links ________________________________");
                smtp.Disconnect(true);
                _logger.LogInformation("____________________ 4 email sender links ________________________________");
                _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");
                return new mailresponse(true, "mail sent successfully");
            }
            catch (Exception ex)
            {

                return new mailresponse(false, ex.Message);
            }
        }


        //email on created user

        public async Task<mailresponse> EmailOnCreatedUser(EmailbodyOnCreatedUser usermailvm)
        {
            try
            {
                _logger.LogInformation($"_____________________1.  email on registration service started  at {DateTime.Now} _______________________________");
                var file = @"Templates/Email/emailon_new_User_Created.html";
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                StreamReader str = new StreamReader(file);
                string MailText = await str.ReadToEndAsync();
                str.Close();
                var datesent = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                var datecreated = usermailvm.CreatedDate.ToString("dddd, dd MMMM yyyy");
                MailText = MailText.Replace("subject", "Notification on User Creation")
                    .Replace("usermail", _emailconfig.SmtpUser)
                    .Replace("emailsentdate", datesent)
                    .Replace("payload", usermailvm.PayLoad)
                    .Replace("useremail", usermailvm.UserEmail)
                    .Replace("dateCreated", datecreated)
                    .Replace("admin_names", usermailvm.AdminNames);


                var builder = new BodyBuilder { HtmlBody = MailText };
                email.Body = builder.ToMessageBody();
                email.To.Add(MailboxAddress.Parse(usermailvm.ToEmail));
                email.Subject = "Notification on User Creation";
                using var smtp = new SmtpClient();
                smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                var resp = await smtp.SendAsync(email);
                _logger.LogInformation("____________________ 3 email sender links ________________________________");
                smtp.Disconnect(true);
                _logger.LogInformation("____________________ 4 email sender links ________________________________");
                _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");
                return new mailresponse(true, "mail sent successfully");
            }
            catch (Exception ex)
            {

                return new mailresponse(false, ex.Message);
            }
        }
        public async Task<mailresponse> SendForgotPasswordLink(emailbody emailvm)
        {
            try
            {
                _logger.LogInformation($"_____________________1. email sent for forget password service started  at {DateTime.Now} _______________________________");

                var file = @"Templates/Email/user_link_sent_on_forget_password.html";
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                StreamReader str = new StreamReader(file);
                string MailText = await str.ReadToEndAsync();
                str.Close();
                var datesent = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                MailText = MailText
                    .Replace("receivernames", emailvm.UserName)
                    .Replace("emailsentdate", datesent)
                    .Replace("providedlink", emailvm.PayLoad);

                var builder = new BodyBuilder { HtmlBody = MailText };
                email.Body = builder.ToMessageBody();
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Reset Password";

                using var smtp = new SmtpClient();

                // Attempt to connect using SSL/TLS options
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Bypass certificate validation temporarily (use only for debugging)

                smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort), MailKit.Security.SecureSocketOptions.StartTls);

                smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                var resp = await smtp.SendAsync(email);

                _logger.LogInformation("____________________ 3 email sender links ________________________________");
                smtp.Disconnect(true);
                _logger.LogInformation("____________________ 4 email sender links ________________________________");
                _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");

                return new mailresponse(true, "mail sent successfully");
            }
            catch (Exception ex)
            {
                return new mailresponse(false, ex.Message);
            }
        }


        public async Task<mailresponse> send_status_to_Requester(emailbody emailvm)
        {
            try
            {
                _logger.LogInformation($"_____________________1.  email sent on status to requester service started  at {DateTime.Now} _______________________________");
                var file = @"Templates/Email/update_request_status.html";
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                StreamReader str = new StreamReader(file);
                string MailText = await str.ReadToEndAsync();
                str.Close();
                var datesent = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                MailText = MailText
                    //.Replace("receivernames", emailvm.UserName)
                    .Replace("emailsentdate", datesent)
                     .Replace("payload", emailvm.PayLoad);

                var builder = new BodyBuilder { HtmlBody = MailText };
                email.Body = builder.ToMessageBody();
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Request Status";
                using var smtp = new SmtpClient();
                smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                var resp = await smtp.SendAsync(email);
                _logger.LogInformation("____________________ 3 email sender links ________________________________");
                smtp.Disconnect(true);
                _logger.LogInformation("____________________ 4 email sender links ________________________________");
                _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");
                return new mailresponse(true, "mail sent successfully");
            }
            catch (Exception ex)
            {

                return new mailresponse(false, ex.Message);
            }
        }

        public async Task<mailresponse> Send_On_Approval_to_Requester(emailbody emailvm)
        {
            try
            {
                _logger.LogInformation($"_____________________1.  email sent to requester service started  at {DateTime.Now} _______________________________");
                var file = @"Templates/Email/send_to_Requester.html";
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                StreamReader str = new StreamReader(file);
                string MailText = await str.ReadToEndAsync();
                str.Close();
                var datesent = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                MailText = MailText
                    .Replace("receivernames", emailvm.UserName)
                    .Replace("emailsentdate", datesent);
                //.Replace("providedlink", emailvm.PayLoad);

                var builder = new BodyBuilder { HtmlBody = MailText };
                email.Body = builder.ToMessageBody();
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Reset Password";
                using var smtp = new SmtpClient();
                smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                var resp = await smtp.SendAsync(email);
                _logger.LogInformation("____________________ 3 email sender links ________________________________");
                smtp.Disconnect(true);
                _logger.LogInformation("____________________ 4 email sender links ________________________________");
                _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");
                return new mailresponse(true, "mail sent successfully");
            }
            catch (Exception ex)
            {
                return new mailresponse(false, ex.Message);
            }
        }

        public async Task<mailresponse> Send_On_Issued(emailbody emailvm)
        {
            try
            {
                _logger.LogInformation($"_____________________1.  email sent on approval service started  at {DateTime.Now} _______________________________");
                var file = @"Templates/Email/send_on_Approval.html";
                var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                StreamReader str = new StreamReader(file);
                string MailText = await str.ReadToEndAsync();
                str.Close();
                var datesent = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                MailText = MailText
                    .Replace("receivernames", emailvm.UserName)
                    .Replace("emailsentdate", datesent)
                    .Replace("providedlink", emailvm.PayLoad);

                var builder = new BodyBuilder { HtmlBody = MailText };
                email.Body = builder.ToMessageBody();
                email.To.Add(MailboxAddress.Parse(emailvm.ToEmail));
                email.Subject = "Reset Password";
                using var smtp = new SmtpClient();
                smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                    MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                var resp = await smtp.SendAsync(email);
                _logger.LogInformation("____________________ 3 email sender links ________________________________");
                smtp.Disconnect(true);
                _logger.LogInformation("____________________ 4 email sender links ________________________________");
                _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");
                return new mailresponse(true, "mail sent successfully");
            }
            catch (Exception ex)
            {

                return new mailresponse(false, ex.Message);
            }
        }



        public async Task IssuerEmail()
        {

            try
            {
                using (var scope = _scopefactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var all_issures = await scopedcontext.PayhouseDragonFlyUsers.Where(y => y.Issuer == true).ToListAsync();

                    foreach (var user in all_issures)
                    {


                        _logger.LogInformation($"_____________________1.  email sent to requester service started  at {DateTime.Now} _______________________________");
                        var file = @"Templates/Email/sent_to_issuer.html";
                        var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                        StreamReader str = new StreamReader(file);
                        string MailText = await str.ReadToEndAsync();
                        str.Close();
                        var send_message = "A request has been approved kindly log in to issue ";
                        var datesent = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                        MailText = MailText
                            .Replace("receivernames", user.FirstName)
                            .Replace("emailsentdate", datesent).
                             Replace("payload_message", send_message);

                        var builder = new BodyBuilder { HtmlBody = MailText };
                        email.Body = builder.ToMessageBody();
                        email.To.Add(MailboxAddress.Parse(user.Email));
                        email.Subject = "Approved Request";
                        using var smtp = new SmtpClient();
                        smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                            MailKit.Security.SecureSocketOptions.StartTls);
                        smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                        var resp = await smtp.SendAsync(email);
                        _logger.LogInformation("____________________ 3 email sender links ________________________________");
                        smtp.Disconnect(true);
                        _logger.LogInformation("____________________ 4 email sender links ________________________________");
                        _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("___________******______", ex.Message);

            }
        }






        public async Task MakerEmail()
        {

            try
            {
                using (var scope = _scopefactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var all_issures = await scopedcontext.PayhouseDragonFlyUsers.Where(y => y.Checker == true).ToListAsync();

                    foreach (var user in all_issures)
                    {


                        _logger.LogInformation($"_____________________1.  email sent to requester service started  at {DateTime.Now} _______________________________");
                        var file = @"Templates/Email/checker_mail.html";
                        var email = new MimeMessage { Sender = MailboxAddress.Parse(_emailconfig.SmtpUser) };
                        StreamReader str = new StreamReader(file);
                        string MailText = await str.ReadToEndAsync();
                        str.Close();
                        var send_message = "A request has been made , Kindly log in and provide the appropriate action ";
                        var datesent = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
                        MailText = MailText
                            .Replace("receivernames", user.FirstName)
                            .Replace("emailsentdate", datesent).
                             Replace("payload_message", send_message);

                        var builder = new BodyBuilder { HtmlBody = MailText };
                        email.Body = builder.ToMessageBody();
                        email.To.Add(MailboxAddress.Parse(user.Email));
                        email.Subject = "New Requisition Application";
                        using var smtp = new SmtpClient();
                        smtp.Connect(_emailconfig.SmtpHost, Convert.ToInt32(_emailconfig.SmtpPort),
                            MailKit.Security.SecureSocketOptions.StartTls);
                        smtp.Authenticate(_emailconfig.EmailFrom, _emailconfig.SmtpPass);
                        var resp = await smtp.SendAsync(email);
                        _logger.LogInformation("____________________ 3 email sender links ________________________________");
                        smtp.Disconnect(true);
                        _logger.LogInformation("____________________ 4 email sender links ________________________________");
                        _logger.LogInformation($"Email on registration sent successfully {DateTime.Now}");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("___________******______", ex.Message);

            }
        }
    }

}


    //send reset email






    

