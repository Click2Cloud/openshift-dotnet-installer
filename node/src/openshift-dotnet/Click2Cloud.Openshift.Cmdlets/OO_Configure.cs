﻿using System;
using System.Management.Automation;
using Click2Cloud.Openshift.Runtime;

namespace Click2Cloud.Openshift.Cmdlets
{
    [Cmdlet("OO", "Configure")]
    public class OO_Configure : System.Management.Automation.Cmdlet 
    {
        [Parameter]
        public string WithAppUuid;

        [Parameter]
        public string WithAppName;

        [Parameter]
        public string WithContainerUuid;

        [Parameter]
        public string WithContainerName;

        [Parameter]
        public string WithNamespace;

        [Parameter]
        public string WithRequestId;

        [Parameter]
        public string CartName;

        [Parameter]
        public string ComponentName;

        [Parameter]
        public string WithSoftwareVersion;

        [Parameter]
        public string CartridgeVendor;

        [Parameter]
        public string WithTemplateGitUrl;

        [Parameter]
        public string WithCartridgeManifest;
        
        [Parameter]
        public string WithExposePorts;

        [Parameter]
        public int WithUid;

        protected override void ProcessRecord()
        {
            this.WriteObject(Execute());
        }

        public ReturnStatus Execute()
        {
            ReturnStatus status = new ReturnStatus();
            try
            {
                ApplicationContainer container = new ApplicationContainer(WithAppUuid, WithContainerUuid, null, WithAppName, WithContainerName,
                                WithNamespace, null, null, null, WithUid);

                status.Output = container.Configure(CartName, WithTemplateGitUrl, WithCartridgeManifest, bool.Parse(WithExposePorts));
                status.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Error running oo-configure command: {0} - {1}", ex.Message, ex.StackTrace);
                status.Output = ex.ToString();
                status.ExitCode = 1;
            }
            return status;
        }
    }
}
