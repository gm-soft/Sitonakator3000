using System;
using System.Collections.Generic;
using System.Configuration;
using Logic;

namespace WebsiteReleaseHelper.Helpers
{
    public class WebsiteReleaseGlobalInfo : IGlobalInfo
    {
        private string _webSiteCommondDirectoryName;

        public string SiteFolderName()
        {
            if (_webSiteCommondDirectoryName == null)
                _webSiteCommondDirectoryName = GetConfig("website_common_dir");

            return _webSiteCommondDirectoryName;
        }

        private string _webSiteArchiveDirectoryName;
        public string SiteArchiveFolderName()
        {
            if (_webSiteArchiveDirectoryName == null)
                _webSiteArchiveDirectoryName = GetConfig("website_arch_dir");

            return _webSiteArchiveDirectoryName;
        }

        private string _primaryInstanceDeployDirectory;

        public string PrimaryInstanceDeployDirectory()
        {
            if (_primaryInstanceDeployDirectory == null)
                _primaryInstanceDeployDirectory = GetConfig("primary_instance_deploy_dir");

            return _primaryInstanceDeployDirectory;
        }

        private string _secondaryInstanceDeployDirectory;

        public string SecondaryInstanceDeployDirectory()
        {
            if (_secondaryInstanceDeployDirectory == null)
                _secondaryInstanceDeployDirectory = GetConfig("secondary_instance_deploy_dir");

            return _secondaryInstanceDeployDirectory;
        }

        private string _kaspiManagerInstanceDeployDirectory;

        public string KaspiManagerInstanceDeployDirectory()
        {
            if (_kaspiManagerInstanceDeployDirectory == null)
                _kaspiManagerInstanceDeployDirectory = GetConfig("kaspimanager_instance_deploy_dir");

            return _kaspiManagerInstanceDeployDirectory;
        }

        private IReadOnlyCollection<string> _specificContentFolderNames;
        public IReadOnlyCollection<string> SpecificContentFolderNames()
        {
            if (_specificContentFolderNames == null)
                _specificContentFolderNames = GetConfig("specific_content_folder_names").ToLowerInvariant().Split(',');

            return _specificContentFolderNames;
        }

        public string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? throw new KeyNotFoundException();
        }
    }
}