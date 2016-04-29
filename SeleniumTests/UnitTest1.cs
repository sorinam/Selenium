using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.Threading;
using System;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;

namespace SeleniumTests
{
    [TestClass]
    public class VerifyItems
    {
        IWebDriver driver;
        string user = "sorina";
        string password = "sorilimo";
        string hostname = "localhost:1753/FileMonitor";

        //[TestInitialize]
        //public void TestSetup()
        //{
        //    driver = new FirefoxDriver();
        //    driver.Manage().Window.Maximize();

        //}

        [TestMethod]
        public void FileMonitorShoulBeInstalled()
        {
            var exitCode= RunServerInstaller();
            Assert.IsTrue(exitCode,"Some error occured at FileMonitor Server installation!");
            Thread.Sleep(30000);
        }

        [TestMethod]
        public void FileMonitorServiceShoudBePresent()
        {
            var FileMonService = new Utils();
            var serviceExist = FileMonService.ServiceExist("FMServerSvc");
            Assert.IsTrue(serviceExist, "ServerService does not exist!");
        }

        [TestMethod]
        public void FileMonitorServiceShoudBeRunning()
        {
            var FileMonService = new Utils();
            var serviceIsRunning = FileMonService.ServiceIsRunning("FMServerSvc");
            Assert.IsTrue(serviceIsRunning, "Server Service is not running!");
        }

        [TestMethod]
        public void DasboarbPageShouldBeOpenedInFirefox()
        {
            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("network.http.phisy-userpass-length", 255);
            profile.SetPreference("network.automatic-ntlm-auth.trusted-uris", hostname);
            driver = new FirefoxDriver(profile);
            OpenBrowserAndValidateDashboard();
            driver.Quit();
        }

        [TestMethod]
        public void DasboarbPageShouldBeOpenedInChrome()
        {
            driver = new ChromeDriver();
            OpenBrowserAndValidateDashboard();
            driver.Quit();
        }

        [TestMethod]
        public void DasboarbPageShouldBeOpenedInIE()
        {           
            driver = new InternetExplorerDriver();
            OpenBrowserAndValidateDashboard();
            driver.Quit();
        }
        private void OpenBrowserAndValidateDashboard()
        {
            driver.Manage().Window.Maximize();
            var url = String.Format("http://{0}:{1}@{2}", user, password, hostname);
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);

            ValidateHeaderMenu();
                    
            //var refresh = driver.FindElement(By.Id("DashboardEditMenu_DXI0_T"));
            //string refreshclass = refresh.GetAttribute("class");
            //Assert.AreEqual(refreshclass,"dxm-content");

            //var feedbackButton = driver.FindElement(By.Name("testbutton"));
            //string feedbackButtonValue = feedbackButton.GetAttribute("value");
            //Assert.AreEqual(feedbackButtonValue,"Send us feedback");
        }

        private void ValidateHeaderMenu()
        {          
            var element = driver.FindElement(By.TagName("img"));
            string imageId = element.GetAttribute("id");
            Assert.AreEqual(imageId, "logo");

            var dashboard = driver.FindElement(By.Id("dashboardMenu"));
            //string dashboardValue = dashboard.GetAttribute("class");
            //Assert.AreEqual(dashboardValue, "dashboard active");

            var dataViewerLink = driver.FindElement(By.ClassName("logs"));
            string dataViewer = dataViewerLink.Text;
            Assert.AreEqual(dataViewer, "Data viewer");

            var reports = driver.FindElement(By.XPath("//div[@class='menuContainer']/ul/li[@id='reportsMenu']"));
            Assert.AreEqual(reports.Text, "Reports");

            var settings = driver.FindElement(By.PartialLinkText("Settings"));

            var infoImg = driver.FindElement(By.XPath("//li[@class='info']/span"));
            Assert.AreEqual(infoImg.GetAttribute("id"), "ddInfo");

            var helpImg = driver.FindElement(By.ClassName("helpSubmenu"));
          

        }

        private bool RunServerInstaller()
        {
            File.Copy(@"c:\Temp\silentinstallconfig.xml", @"c:\ProgramData\silentinstallconfig.xml", true);
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"c:\Temp\ServerInstaller.exe",
                    Arguments = "/i /q"
                }
            };
            process.Start();
            process.WaitForExit();
            Thread.Sleep(2000);
        
            var utils = new Utils();
            bool finishedConfig=utils.WaitUntilProcessIsNotRunning("PostInstallServerConfig");
           
            if ((process.ExitCode == 0) && finishedConfig)
                return true;
            else
                return false;
        }
              

        //[TestCleanup]
        //public void Cleanup()
        //{
        //    driver.Quit();
        //} 
    }
}
