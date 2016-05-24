using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Click2Cloud.Openshift.Common;
using System.IO;
using System.Linq;
using Click2Cloud.Openshift.Runtime.Config;
using Click2Cloud.Openshift.Runtime;

namespace Click2Cloud.Openshift.Tests.Unit
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void TestLogsOK()
        {
            // Arrange
            Logger.LogFile = Path.GetTempFileName();

            // Act
            Logger.Debug("test message");

            // Assert
            Assert.IsTrue(File.Exists(Logger.LogFile));            
        }
    }
}
