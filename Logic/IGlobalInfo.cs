using System.Collections.Generic;

namespace Logic
{
    public interface IGlobalInfo
    {
        string SiteFolderName();

        string SiteArchiveFolderName();

        string PrimaryInstanceDeployDirectory();

        string SecondaryInstanceDeployDirectory();

        string KaspiManagerInstanceDeployDirectory();

        IReadOnlyCollection<string> SpecificContentFolderNames();

        string GetConfig(string key);
    }
}