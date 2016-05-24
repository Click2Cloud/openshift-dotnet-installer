using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Click2Cloud.Openshift.Common;
using System.IO;
using System.Linq;
using Click2Cloud.Openshift.Runtime.Config;
using Click2Cloud.Openshift.Runtime;
using Click2Cloud.Openshift.Utilities;

namespace Click2Cloud.Openshift.Tests.Integration
{
    /// <summary>
    /// These tests assume there is a node.conf, and that ruby is properly setup
    /// </summary>
    [TestClass]
    public class FacterTest
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void TestFacterOK()
        {
            // Arrange

            // Act
            RubyHash facts = Click2Cloud.Openshift.Runtime.Utils.Facter.GetFacterFacts();

            // Assert
            Assert.IsTrue(facts.ContainsKey("operatingsystem"));
            Assert.AreEqual(facts["operatingsystem"], "windows");
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Test_FacterOpenshift()
        {
            RubyHash facts = Click2Cloud.Openshift.Runtime.Utils.Facter.GetOpenshiftFacts();
            Assert.IsTrue(facts.ContainsKey("node_profile"));
        }
    }
}
