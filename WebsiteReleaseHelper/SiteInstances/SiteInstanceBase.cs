using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using Logic;
using Logic.Arhivator;

namespace WebsiteReleaseHelper.SiteInstances
{
    public abstract class SiteInstanceBase
    {
        protected SiteInstanceBase(IGlobalInfo globalInfo)
        {
            GlobalInfo = globalInfo;
        }

        protected IGlobalInfo GlobalInfo { get; }

        protected abstract string WebsiteRootPath { get; }

        protected abstract string DeployDirectoryPath { get; }

        protected abstract IReadOnlyCollection<string> WebsiteUrls { get; }

        public void OpenInWebBrowser()
        {
            if (WebsiteUrls == null || WebsiteUrls.Count == 0)
                throw new InvalidOperationException(@"Не указаны веб-урлы для запуска сайта");

            foreach (string websiteUrl in WebsiteUrls)
                Process.Start(websiteUrl);
        }

        public void Archive(string archiveFolderNamePostfix, Action<ArchiveResult> archiveFinishedCallback)
        {
            var websiteDir = new WebsiteArchivator(WebsiteRootPath, GlobalInfo);

            websiteDir.Archive(archiveFolderNamePostfix, archiveFinishedCallback);
        }

        public void CopyFilesFromDeployDirectory()
        {

        }
    }
}