using System.Collections.Generic;

namespace Logic.Settings
{
    public class WebsiteNodeData
    {
        public string DisplayableName { get; set; }

        public string DeployDirectoryPath { get; set; }

        public string WebsiteRootPath { get; set; }

        public string ServerMachineName { get; set; }

        public List<string> WebsiteUrls { get; set; }
    }
}