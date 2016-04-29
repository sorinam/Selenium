using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace SeleniumTests
{
    class Utils
    {
        public bool ServiceExist(string serviceName)
        {
            bool res = false;

            try
            {
                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == serviceName)
                        return true;
                }

            }
            catch (Exception e)
            {
                Assert.Fail(String.Format("Exception: {0} when try to find service {1}"), e, serviceName);
            }
            return res;
        }

        public bool ServiceIsRunning(string serviceName)
        {
            bool result = false;

            try
            {
                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    if ((service.ServiceName == serviceName) && (service.Status == ServiceControllerStatus.Running))
                        return true;
                }

            }
            catch (Exception e)
            {
                Assert.Fail(String.Format("Exception: {0} when try to find service {1}"), e, serviceName);
            }
            return result;
        }

        public bool WaitUntilProcessIsNotRunning(string processName)
        {
            bool notfound = false;
            int count = 0;
            while (!notfound && count <300)
            {
                var proceses = System.Diagnostics.Process.GetProcessesByName(processName);

                if (proceses.Length == 0)
                {
                    notfound = true;
                }

                Thread.Sleep(1000);
                count++;
            }
            return notfound;
        }
    }
}
