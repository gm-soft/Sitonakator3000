using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Logic.Arhivator;
using Logic.DirectoryHelpers;

namespace Logic.SiteInstances
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

        public void Archive(string archiveFolderNamePostfix, Action<AsyncActionResult> archiveFinishedCallback)
        {
            var websiteDir = new WebsiteArchivator(WebsiteRootPath, GlobalInfo);

            websiteDir.Archive(archiveFolderNamePostfix, archiveFinishedCallback);
        }

        public async void CopyFilesFromDeployDirectory(Action<AsyncActionResult> copyFinishedCallback)
        {
            var copyTargetDirectory = Path.Combine(WebsiteRootPath, GlobalInfo.SiteFolderName());

            var filesProvider = new FilesReplicator(DeployDirectoryPath, copyTargetDirectory)
            {
                DirectoryNamesToIgnore = GlobalInfo.SpecificContentFolderNames()
            };

            await filesProvider.CopyAllAsync(copyFinishedCallback);
        }
    }
}