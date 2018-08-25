using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Logic.DirectoryHelpers;

namespace Logic.Arhivator
{
    public class WebsiteArchivator
    {
        private readonly string _siteDirectoryPath;
        private readonly string _archDirectoryRootPath;

        private readonly IReadOnlyCollection<string> _directoryNamesToIgnore;

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

            Directory.CreateDirectory(archiveDirectoryPath);

            var filesProvider = new FilesToCopyProvider(_siteDirectoryPath, archiveDirectoryPath)
            {
                DirectoryNamesToIgnore = _directoryNamesToIgnore
            };

            await filesProvider.CopyAllFilesAsync();

            await RemoveAllContentAsync(_siteDirectoryPath);
        }

        public static string GetNewFolderNameFromCurrentDate(string archiveFolderNamePostfix = null)
        {
            var currentDate = DateTime.Today.ToString("yyyy-MM-dd");
            return !string.IsNullOrWhiteSpace(archiveFolderNamePostfix) 
                ? currentDate + "-" + archiveFolderNamePostfix
                : currentDate;
        }

        private async Task RemoveAllContentAsync(string directoryPath)
        {
            await Task.Run(() =>
            {
                var directoryInfo = new DirectoryInfo(directoryPath);

                // https://stackoverflow.com/a/1288747
                foreach (FileInfo file in directoryInfo.EnumerateFiles())
                {
                    file.Delete();
                }

                IEnumerable<DirectoryInfo> dirsToRemove = directoryInfo.EnumerateDirectories();

                if (_directoryNamesToIgnore != null && _directoryNamesToIgnore.Count > 0)
                    dirsToRemove = dirsToRemove.Where(x => !_directoryNamesToIgnore.Contains(x.Name.ToLowerInvariant()));

                foreach (DirectoryInfo dir in dirsToRemove)
                    dir.Delete(true);
            });
        }
    }
}