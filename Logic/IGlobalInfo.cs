using System.Collections.Generic;

namespace Logic
{
    public interface IGlobalInfo
    {
        string SiteFolderName();

        string SiteArchiveFolderName();

        IReadOnlyCollection<string> SpecificContentFolderNames();

        string GetConfig(string key);
    }
}