using PayhouseDragonFly.CORE.ConnectorClasses.Response;
using PayhouseDragonFly.CORE.DTOs.EmaillDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IEmailServices
{
    public interface IEmailServices
    {
        Task<mailresponse> SenTestMail(emailbody emailvm);
        Task<mailresponse> SendEmailOnRegistration(emailbody emailvm);
        Task<mailresponse> SendEmailOnLeaveCompletion(EmailbodyOnLeaveEnd emailvm);
        Task<mailresponse> EmailOnCreatedUser(EmailbodyOnCreatedUser usermailvm);
        Task<mailresponse> SendForgotPasswordLink(emailbody emailvm);
       // Task<mailresponse> Send_On_Approval_to_Approver(emailbody emailvm);
        Task<mailresponse> send_status_to_Requester(emailbody emailvm);
        Task<mailresponse> Send_On_Issued(emailbody emailvm);
        Task IssuerEmail();
        Task<mailresponse> SendEmailOnUserActivation(emailbody emailvm);
        Task MakerEmail();
    }
}
