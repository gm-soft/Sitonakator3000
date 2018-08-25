using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic.DirectoryHelpers
{
    public class FilesToCopyProvider
    {
        private readonly string _sourceDirectory;

        private readonly string _targetDirectory;

        private readonly IReadOnlyCollection<string> _direcoryNamesToIgnore;

        public FilesToCopyProvider(string sourceDirectory, string targetDirectory, IReadOnlyCollection<string> direcoryNamesToIgnore = null)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _direcoryNamesToIgnore = direcoryNamesToIgnore;
        }

        public IReadOnlyCollection<FileCopyInfo> GetAll()
        {
            var result = new List<FileCopyInfo>();
            PerformDeepCopyScanning(_sourceDirectory, _targetDirectory, ref result);

            return result;
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

            IEnumerable<DirectoryInfo> directories = GetSubDirectoriesWithoutIngored(sourceDir);

            foreach (DirectoryInfo dir in directories)
            {
                DirectoryInfo subDirectory = targetDir.CreateSubdirectory(dir.Name);
                PerformDeepCopyScanning(dir.FullName, subDirectory.FullName, ref results);
            }
        }

        private IEnumerable<DirectoryInfo> GetSubDirectoriesWithoutIngored(DirectoryInfo sourceDir)
        {
            var directories = sourceDir.EnumerateDirectories();
            if (_direcoryNamesToIgnore?.Count > 0)
                directories = directories.Where(x => !_direcoryNamesToIgnore.Contains(x.Name));

            return directories;
        }
    }
}