using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Logic.Arhivator;

namespace Logic.IoProviders
{
    public class FilesReplicator
    {
        private readonly string _sourceDirectory;

        private readonly string _targetDirectory;

        private readonly DirectoryHelper _directoryHelper;

        public FilesReplicator(string sourceDirectory, string targetDirectory, DirectoryHelper directoryHelper)
        {
            _sourceDirectory = sourceDirectory
                               ?? throw new ArgumentNullException(paramName: nameof(sourceDirectory));

            _targetDirectory = targetDirectory
                               ?? throw new ArgumentNullException(paramName: nameof(targetDirectory));

            _directoryHelper = directoryHelper 
                               ?? throw new ArgumentNullException(paramName: nameof(directoryHelper));
        }

        /// <summary>
        /// Копирование без обработки исключений
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        internal async Task CopyAllAsync()
        {
            CheckSourceDirectoryForContent();

            await Task.Run(() =>
            {
                IReadOnlyCollection<FileCopyInfo> filesToCopy = GetAll();

                var tasks = new List<Task>();
                foreach (FileCopyInfo fileCopyInfo in filesToCopy)
                    tasks.Add(fileCopyInfo.CopyAsync());

                Task.WaitAll(tasks.ToArray());
            });
        }

        /// <summary>
        /// Копирование с обработчиком исключений
        /// </summary>
        /// <param name="copyFinishedCallback"></param>
        /// <returns></returns>
        public async Task CopyAllAsync(Action<AsyncActionResult> copyFinishedCallback)
        {
            if (copyFinishedCallback == null)
                throw new ArgumentNullException(paramName: nameof(copyFinishedCallback));

            try
            {
                CheckSourceDirectoryForContent();

                // Сначала удалим все файлы в паке, куда копируем
                await _directoryHelper.RemoveAllContentAsync(_targetDirectory);

                await CopyAllAsync();

                copyFinishedCallback(AsyncActionResult.Success());
            }
            catch (Exception ex)
            {
                copyFinishedCallback(AsyncActionResult.Fail(ex));
            }
        }

        private IReadOnlyCollection<FileCopyInfo> GetAll()
        {
            var result = new List<FileCopyInfo>();
            PerformDeepCopyScanning(_sourceDirectory, _targetDirectory, ref result);

            return result;
        }

        private void CheckSourceDirectoryForContent()
        {
            if (_directoryHelper.CheckSourceDirectoryForContent(_sourceDirectory))
                throw new InvalidOperationException($@"В папке-источнике {_sourceDirectory} не обнаружено никаких файлов или папок");
        }

        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/dab86e37-a25b-4bdb-9552-7e6c7ed509c7/how-to-copy-files-and-directories-recursively?forum=csharpgeneral
        private void PerformDeepCopyScanning(string sourceDirectory, string destinationDirectory, ref List<FileCopyInfo> results)
        {
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            var sourceDir = new DirectoryInfo(sourceDirectory);
            var targetDir = new DirectoryInfo(destinationDirectory);

            FileInfo[] fileInfos = sourceDir.GetFiles();

            foreach (FileInfo fileInfo in fileInfos)
                results.Add(new FileCopyInfo(fileInfo, targetDir));

            IEnumerable<DirectoryInfo> directories 
                = _directoryHelper.GetSubDirectoriesWithoutIngored(sourceDir);

            foreach (DirectoryInfo dir in directories)
            {
                DirectoryInfo subDirectory = targetDir.CreateSubdirectory(dir.Name);
                PerformDeepCopyScanning(dir.FullName, subDirectory.FullName, ref results);
            }
        }
    }
}