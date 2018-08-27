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

            _displayableName = nodeData.DisplayableName;
            _websiteUrls = nodeData.WebsiteUrls;
            _deployDirectoryPath = nodeData.DeployDirectoryPath;
            _websiteRootPath = nodeData.WebsiteRootPath;

            DirectoryHelper = new DirectoryHelper(_globalInfo.SpecificContentFolderNames());
        }

        private readonly IGlobalInfo _globalInfo;

        /// <summary>
        /// Отображаемое в программе имя
        /// </summary>
        private readonly string _displayableName;

        /// <summary>
        /// Путь до папки сайта, который содержит подпапки с файлами проекта, архивами и логами
        /// </summary>
        private readonly string _websiteRootPath;

        /// <summary>
        /// Путь до папки, куда деплоятся файлы с машин разработчиков
        /// </summary>
        private readonly string _deployDirectoryPath;

        /// <summary>
        /// Урлы сайта, которые нужно открыть в браузере, чтобы запустить ноды
        /// </summary>
        private readonly IReadOnlyCollection<string> _websiteUrls;

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
            var websiteDir = new WebsiteArchivator(_websiteRootPath, _globalInfo, DirectoryHelper);

            websiteDir.Archive(archiveFolderNamePostfix, archiveFinishedCallback);
        }

        public async void CopyFilesFromDeployDirectory(Action<AsyncActionResult> copyFinishedCallback)
        {
            var copyTargetDirectory = Path.Combine(_websiteRootPath, _globalInfo.SiteFolderName());

            var filesProvider = new FilesReplicator(_deployDirectoryPath, copyTargetDirectory, DirectoryHelper);

            await filesProvider.CopyAllAsync(copyFinishedCallback);
        }

        public override string ToString()
        {
            return _displayableName;
        }
    }
}