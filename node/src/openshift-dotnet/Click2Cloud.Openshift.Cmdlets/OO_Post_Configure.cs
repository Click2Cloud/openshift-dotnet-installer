using System;
using System.Management.Automation;
using Click2Cloud.Openshift.Runtime;
using Click2Cloud.Openshift.Runtime.Utils;

namespace Click2Cloud.Openshift.Cmdlets
{
    [Cmdlet("OO", "Post-Configure")]
    public class OO_Post_Configure : System.Management.Automation.Cmdlet 
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
                   WithNamespace, null, null, new Hourglass(235), WithUid);
                status.Output = container.PostConfigure(CartName, WithTemplateGitUrl);
                status.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Error running oo-post-configure command: {0} - {1}", ex.Message, ex.StackTrace);
                status.Output = ex.ToString();
                status.ExitCode = 1;
            }
            return status;
        }
    }
}
