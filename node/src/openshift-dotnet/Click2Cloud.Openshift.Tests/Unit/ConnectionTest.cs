using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Click2Cloud.Openshift.Common.Models;
using Click2Cloud.Openshift.Utilities;

namespace Click2Cloud.Openshift.Tests.Unit
{
    [TestClass]
    public class ConnectionTest
    {

        [TestMethod]
        [TestCategory("Unit")]
        public void Test_ConnectionModel()
        {
            bool result = false;
            RubyHash specs = new RubyHash();
            specs["Components"] = new List<object>(){"TestComponent"};
            Connection m = Connection.FromDescriptor("testConnection", specs);
            if (m != null)
            {
                var desc = m.ToDescriptor();
                if (desc != null)
                {
                    result = true;
                }
            }
            Assert.AreEqual(true, result);
        }
    }
}
