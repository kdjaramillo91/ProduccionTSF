using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace DXPANACEASOFT.Helper
{
    public class BatchEditingDemoHelper
    {
        const string BatchEditingHelperSessionKey = "E509E30E-99E3-4CB3-A07B-A08B04784A46";

        public static BatchEditingDemoOptions Options
        {
            get
            {
                if (Session[BatchEditingHelperSessionKey] == null)
                    Session[BatchEditingHelperSessionKey] = new BatchEditingDemoOptions();
                return (BatchEditingDemoOptions)Session[BatchEditingHelperSessionKey];
            }
            set { Session[BatchEditingHelperSessionKey] = value; }
        }
        protected static HttpSessionState Session { get { return HttpContext.Current.Session; } }
    }
}