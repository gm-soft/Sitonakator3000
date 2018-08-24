namespace Logic
{
    public interface IGlobalInfo
    {
        string WebsiteCommonDirectoryName();

        string WebsiteArchiveDirectoryName();

        string PrimaryInstanceDeployDirectory();

        string SecondaryInstanceDeployDirectory();

        string KaspiManagerInstanceDeployDirectory();

        string GetConfig(string key);
    }
}