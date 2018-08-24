using System.Collections.Generic;
using System.IO;

namespace Logic.DirectoryHelpers
{
    public class FilePathsProvider
    {
        private readonly string _sourceDirectory;

        private readonly string _targetDirectory;

        public FilePathsProvider(string sourceDirectory, string targetDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
        }

        public IReadOnlyCollection<FileCopyInfo> GetAllFilesPaths()
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

            foreach (DirectoryInfo dir in sourceDir.GetDirectories())
            {
                DirectoryInfo subDirectory = targetDir.CreateSubdirectory(dir.Name);
                PerformDeepCopyScanning(dir.FullName, subDirectory.FullName, ref results);
            }
        }
    }
}