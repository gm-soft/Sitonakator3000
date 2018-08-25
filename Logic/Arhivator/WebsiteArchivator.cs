using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Logic.IoProviders;

namespace Logic.Arhivator
{
    public class WebsiteArchivator
    {
        private readonly string _siteDirectoryPath;
        private readonly string _archDirectoryRootPath;

        private readonly IReadOnlyCollection<string> _directoryNamesToIgnore;

        private readonly DirectoryHelper _directoryHelper;

        public WebsiteArchivator(string websiteRootPath, IGlobalInfo globalInfo)
        {
            if (!Directory.Exists(websiteRootPath))
                throw new InvalidOperationException();

            var websiteDirectoryRootPath = websiteRootPath;

            _directoryNamesToIgnore = globalInfo.SpecificContentFolderNames();

            _siteDirectoryPath = Path.Combine(websiteDirectoryRootPath, globalInfo.SiteFolderName());
            _archDirectoryRootPath = Path.Combine(websiteDirectoryRootPath, globalInfo.SiteArchiveFolderName());

            if (!Directory.Exists(_siteDirectoryPath))
                throw new InvalidOperationException($@"Папки с прежними файлами сайта {_siteDirectoryPath} не сущестует");

            if (!Directory.Exists(_archDirectoryRootPath))
                Directory.CreateDirectory(_archDirectoryRootPath);

            _directoryHelper = new DirectoryHelper();
        }

        public void Archive(string archiveFolderNamePostfix, Action<AsyncActionResult> archiveFinishedCallback)
        {
            if (archiveFolderNamePostfix != null &&
                string.Equals(archiveFolderNamePostfix.Trim(), string.Empty, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException(@"Не нужно передавать пустую строку в качестве имени постфикса");

            if (archiveFinishedCallback == null)
                throw new ArgumentNullException(paramName: nameof(archiveFinishedCallback));

            Task.Run(async () =>
            {
                try
                {
                    await ArchiveAsync(archiveFolderNamePostfix);
                    
                    archiveFinishedCallback(AsyncActionResult.Success());
                }
                catch (Exception exception)
                {
                    archiveFinishedCallback(AsyncActionResult.Fail(exception));
                }
            });
        }

        private async Task ArchiveAsync(string archiveFolderNamePostfix = null)
        {
            var archiveFolderName = GetNewFolderNameFromCurrentDate(archiveFolderNamePostfix);

            string archiveDirectoryPath = Path.Combine(_archDirectoryRootPath, archiveFolderName);

            if (Directory.Exists(archiveDirectoryPath))
                throw new InvalidOperationException($@"Папка архива {archiveFolderName} уже сущестует. Не могу архивировать");

            var filesProvider = new FilesReplicator(_siteDirectoryPath, archiveDirectoryPath, _directoryHelper)
            {
                DirectoryNamesToIgnore = _directoryNamesToIgnore
            };

            await filesProvider.CopyAllAsync();

            await filesProvider.RemoveAllContentAsync(_siteDirectoryPath);
        }

        public static string GetNewFolderNameFromCurrentDate(string archiveFolderNamePostfix = null)
        {
            var currentDate = DateTime.Today.ToString("yyyy-MM-dd");
            return !string.IsNullOrWhiteSpace(archiveFolderNamePostfix) 
                ? currentDate + "-" + archiveFolderNamePostfix
                : currentDate;
        }
    }
}