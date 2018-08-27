using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Logic.Arhivator;
using Logic.IoProviders;
using Logic.Settings;

namespace Logic
{
    public class SiteInstance
    {
        public SiteInstance(WebsiteNodeData nodeData, IGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;

            DisplayableName = nodeData.DisplayableName;
            DeployDirectoryPath = nodeData.DeployDirectoryPath;
            WebsiteRootPath = nodeData.WebsiteRootPath;

            _websiteUrls = nodeData.WebsiteUrls;

            DirectoryHelper = new DirectoryHelper(_globalInfo.SpecificContentFolderNames());
        }

        private readonly IGlobalInfo _globalInfo;

        /// <summary>
        /// Отображаемое в программе имя
        /// </summary>
        public string DisplayableName { get; }

        /// <summary>
        /// Путь до папки сайта, который содержит подпапки с файлами проекта, архивами и логами
        /// </summary>
        public string WebsiteRootPath { get; }

        /// <summary>
        /// Путь до папки, куда деплоятся файлы с машин разработчиков
        /// </summary>
        public string DeployDirectoryPath { get; }

        /// <summary>
        /// Урлы сайта, которые нужно открыть в браузере, чтобы запустить ноды
        /// </summary>
        private readonly IReadOnlyCollection<string> _websiteUrls;

        public string WebsiteUrlsAsString => string.Join(", ", _websiteUrls);

        protected DirectoryHelper DirectoryHelper { get; }

        public void OpenInWebBrowser()
        {
            if (_websiteUrls == null || _websiteUrls.Count == 0)
                throw new InvalidOperationException(@"Не указаны веб-урлы для запуска сайта");

            foreach (string websiteUrl in _websiteUrls)
                Process.Start(websiteUrl);
        }

        public void Archive(string archiveFolderNamePostfix, Action<AsyncActionResult> archiveFinishedCallback)
        {
            var websiteDir = new WebsiteArchivator(WebsiteRootPath, _globalInfo, DirectoryHelper);

            websiteDir.Archive(archiveFolderNamePostfix, archiveFinishedCallback);
        }

        public async void CopyFilesFromDeployDirectory(Action<AsyncActionResult> copyFinishedCallback)
        {
            var copyTargetDirectory = Path.Combine(WebsiteRootPath, _globalInfo.SiteFolderName());

            var filesProvider = new FilesReplicator(DeployDirectoryPath, copyTargetDirectory, DirectoryHelper);

            await filesProvider.CopyAllAsync(copyFinishedCallback);
        }

        public void OpenDeployDirectory()
        {
            Process.Start(DeployDirectoryPath);
        }

        public void OpenWebsiteRootDirectory()
        {
            Process.Start(WebsiteRootPath);
        }

        public override string ToString()
        {
            return DisplayableName;
        }
    }
}