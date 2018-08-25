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