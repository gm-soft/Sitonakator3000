using System.Collections.Generic;
using Logic;

namespace WebsiteReleaseHelper.SiteInstances
{
    public class PrimaryInstanceHelper : SiteInstanceBase
    {
        public PrimaryInstanceHelper(IGlobalInfo globalInfo) : base(globalInfo)
        {
            WebsiteUrls = new[] { GlobalInfo.GetConfig("primary_instance_url") };
        }

        protected override string WebsiteRootPath => GlobalInfo.GetConfig("primary_instance_target_dir");
        protected override string DeployDirectoryPath => GlobalInfo.GetConfig("primary_instance_deploy_dir");
        protected override IReadOnlyCollection<string> WebsiteUrls { get; }
    }
}