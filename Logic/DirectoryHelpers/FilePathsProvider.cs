using System;
using System.Collections.Generic;
using System.IO;

namespace Logic.DirectoryHelpers
{
    public class FilePathsProvider
    {
        private readonly string _sourceDirectory;

        private readonly string _targetDirectory;

        private readonly int _expectedDirectoriesLevel;

        public FilePathsProvider(string sourceDirectory, string targetDirectory, int expectedDirectoriesLevel = 5)
        {
            _sourceDirectory = sourceDirectory;
            _targetDirectory = targetDirectory;
            _expectedDirectoriesLevel = expectedDirectoriesLevel;
        }

        public IReadOnlyCollection<FileCopyInfo> GetAllFilesPaths()
        {
            var result = new List<FileCopyInfo>();
            PerformDeepCopy(_sourceDirectory, _targetDirectory, ref result);

            return result;
        }

        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/dab86e37-a25b-4bdb-9552-7e6c7ed509c7/how-to-copy-files-and-directories-recursively?forum=csharpgeneral
        private void PerformDeepCopy(string sourceDirectory, string destinationDirectory, ref List<FileCopyInfo> results)
        {
            if (ExpectedLevelReached(destinationDirectory))
            {
                return;
            }
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            var sourceDir = new DirectoryInfo(sourceDirectory);
            var targetDir = new DirectoryInfo(destinationDirectory);

            FileInfo[] fileInfos = sourceDir.GetFiles();

            foreach (FileInfo fileInfo in fileInfos)
            {
                string pathToCopy = Path.Combine(targetDir.ToString(), fileInfo.Name);

                results.Add(new FileCopyInfo(fileInfo, pathToCopy));
                //fileInfo.CopyTo(pathToCopy, true);
            }

            foreach (DirectoryInfo dir in sourceDir.GetDirectories())
            {
                var subDirectory = targetDir.CreateSubdirectory(dir.Name);
                PerformDeepCopy(dir.FullName, subDirectory.FullName, ref results);
            }
        }

        private bool ExpectedLevelReached(string destinationDirectory)
        {
            int i = 0;
            var targetDir = new DirectoryInfo(destinationDirectory);
            while (targetDir.Parent != null)
            {
                targetDir = targetDir.Parent;
                i++;
            }

            // replace this number how many levels you are looking for
            return i >= _expectedDirectoriesLevel;
        }
    }
}