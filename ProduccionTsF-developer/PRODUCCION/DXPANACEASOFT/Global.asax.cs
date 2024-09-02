using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using Quartz.Impl;
using Quartz;
using static DXPANACEASOFT.Services.ServiceTransCtl;

namespace DXPANACEASOFT
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            DapperConnectionConfig.ConfigPaths();


            InitializeQuartz();

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;
        }

        protected void Application_Error(object sender, EventArgs e) 
        {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
        }

        protected void Application_PostAuthenticateRequest()
       {
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                ApplicationUser user = jsSerializer.Deserialize<ApplicationUser>(ticket.UserData);

                HttpContext.Current.User = new LogedUser(user);
            }
        }

        private void InitializeQuartz()
        {
            
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();

            
            IJobDetail job = JobBuilder.Create<JobTransCtl>()
                .WithIdentity("TransCtl", "Infra")
                .Build();

            var timeOffsetEstimateUpService = DateTimeOffset.Now.AddMinutes(2);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("TransCtl", "Infra")
                .StartAt(timeOffsetEstimateUpService)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(160)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger).Wait();
        }
    }
}