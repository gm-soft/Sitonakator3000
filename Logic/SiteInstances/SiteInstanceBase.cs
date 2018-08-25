using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Logic.Arhivator;
using Logic.IoProviders;

namespace Logic.SiteInstances
{
    public abstract class SiteInstanceBase
    {
        protected SiteInstanceBase(IGlobalInfo globalInfo)
        {
            GlobalInfo = globalInfo;

            DirectoryHelper = new DirectoryHelper(GlobalInfo.SpecificContentFolderNames());
        }

        protected IGlobalInfo GlobalInfo { get; }

        /// <summary>
        /// Путь до папки сайта, который содержит подпапки с файлами проекта, архивами и логами
        /// </summary>
        protected abstract string WebsiteRootPath { get; }
        
        /// <summary>
        /// Путь до папки, куда деплоятся файлы с машин разработчиков
        /// </summary>
        protected abstract string DeployDirectoryPath { get; }

        /// <summary>
        /// Урлы сайта, которые нужно открыть в браузере, чтобы запустить ноды
        /// </summary>
        protected abstract IReadOnlyCollection<string> WebsiteUrls { get; }

        protected DirectoryHelper DirectoryHelper { get; }

        public void OpenInWebBrowser()
        {
            if (WebsiteUrls == null || WebsiteUrls.Count == 0)
                throw new InvalidOperationException(@"Не указаны веб-урлы для запуска сайта");

            foreach (string websiteUrl in WebsiteUrls)
                Process.Start(websiteUrl);
        }

        public void Archive(string archiveFolderNamePostfix, Action<AsyncActionResult> archiveFinishedCallback)
        {
            var websiteDir = new WebsiteArchivator(WebsiteRootPath, GlobalInfo, DirectoryHelper);

            websiteDir.Archive(archiveFolderNamePostfix, archiveFinishedCallback);
        }

        public async void CopyFilesFromDeployDirectory(Action<AsyncActionResult> copyFinishedCallback)
        {
            var copyTargetDirectory = Path.Combine(WebsiteRootPath, GlobalInfo.SiteFolderName());

            var filesProvider = new FilesReplicator(DeployDirectoryPath, copyTargetDirectory, DirectoryHelper);

            await filesProvider.CopyAllAsync(copyFinishedCallback);
        }
    }
}