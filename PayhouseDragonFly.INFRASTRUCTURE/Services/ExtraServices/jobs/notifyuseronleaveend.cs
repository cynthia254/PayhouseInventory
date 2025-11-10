using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IUserServices;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices.jobs
{
    public class notifyuseronleaveend:IJob
    {

        private readonly IUserServices _userservices;

        public notifyuseronleaveend(IUserServices userservices)
        {
            _userservices=userservices;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _userservices.EmailOnLeaveCompletion();
            return Task.CompletedTask;
        }
    }
}
