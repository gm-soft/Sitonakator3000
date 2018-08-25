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

        private readonly Action<ArchiveResult> _archiveFinishedCallback;

        private readonly IReadOnlyCollection<string> _direcoryNamesToIgnore;

        public WebsiteArchivator(string directoryRootPath, IGlobalInfo globalInfo, Action<ArchiveResult> archiveFinishedCallback)
        {
            if (!Directory.Exists(directoryRootPath))
                throw new InvalidOperationException();

            var websiteDirectoryRootPath = directoryRootPath;

            _direcoryNamesToIgnore = globalInfo.SpecificContentFolderNames();

            _siteDirectoryPath = Path.Combine(websiteDirectoryRootPath, globalInfo.SiteFolderName());
            _archDirectoryRootPath = Path.Combine(websiteDirectoryRootPath, globalInfo.SiteArchiveFolderName());

            if (!Directory.Exists(_siteDirectoryPath))
                throw new InvalidOperationException($@"Папки с прежними файлами сайта {_siteDirectoryPath} не сущестует");

            if (!Directory.Exists(_archDirectoryRootPath))
                Directory.CreateDirectory(_archDirectoryRootPath);

            _archiveFinishedCallback = archiveFinishedCallback;
        }

        public void Arhive(string archiveFolderNamePostfix)
        {
            if (archiveFolderNamePostfix != null &&
                string.Equals(archiveFolderNamePostfix.Trim(), string.Empty, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException(@"Не нужно передавать пустую строку в качестве имени постфикса");

            Task.Run(async () =>
            {
                try
                {
                    await ArchiveAsync(archiveFolderNamePostfix);
                    
                    _archiveFinishedCallback(ArchiveResult.Success());
                }
                catch (Exception exception)
                {
                    _archiveFinishedCallback(ArchiveResult.Fail(exception));
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

            IReadOnlyCollection<FileCopyInfo> filesToCopy = new FilesToCopyProvider(
                _siteDirectoryPath,
                archiveDirectoryPath,
                _direcoryNamesToIgnore).GetAll();

            var tasks = new List<Task>();
            foreach (FileCopyInfo fileCopyInfo in filesToCopy)
            {
                tasks.Add(fileCopyInfo.CopyAsync());
            }

            Task.WaitAll(tasks.ToArray());

            await RemoveAllContentAsync(_siteDirectoryPath);
        }

        public static string GetNewFolderNameFromCurrentDate(string archiveFolderNamePostfix = null)
        {
            var currentDate = DateTime.Today.ToString("yyyy-MM-dd");
            return !string.IsNullOrWhiteSpace(archiveFolderNamePostfix) 
                ? currentDate + "-" + archiveFolderNamePostfix
                : currentDate;
        }

        private async Task<bool> RemoveAllContentAsync(string directorypPath)
        {
            return await Task.Run(() =>
            {
                var directoryInfo = new DirectoryInfo(directorypPath);

                // https://stackoverflow.com/a/1288747
                foreach (FileInfo file in directoryInfo.EnumerateFiles())
                {
                    file.Delete();
                }

                IEnumerable<DirectoryInfo> dirsToRemove = directoryInfo.EnumerateDirectories();

                if (_direcoryNamesToIgnore != null && _direcoryNamesToIgnore.Count > 0)
                    dirsToRemove = dirsToRemove.Where(x => !_direcoryNamesToIgnore.Contains(x.Name.ToLowerInvariant()));

                foreach (DirectoryInfo dir in dirsToRemove)
                    dir.Delete(true);

                return true;
            });
        }
    }
}