using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Click2Cloud.Openshift.Runtime;
using Click2Cloud.Openshift.Runtime.Utils;

namespace Click2Cloud.OpenShift.TrapUser
{
    class Program
    {
        static int Main(string[] args)
        {
            return UserShellTrap.StartShell(Environment.CommandLine);
        }
    }
}