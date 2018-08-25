using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic.IoProviders
{
    public class DirectoryHelper
    {
        public bool CheckSourceDirectoryForContent(string directoryPath, 
            IReadOnlyCollection<string> directoryNamesToIgnore)
        {
            var sourceDirInfo = new DirectoryInfo(directoryPath);

            return !sourceDirInfo.EnumerateFiles().Any() || !GetSubDirectoriesWithoutIngored(sourceDirInfo, directoryNamesToIgnore).Any();
        }

        public IEnumerable<DirectoryInfo> GetSubDirectoriesWithoutIngored(DirectoryInfo sourceDir, 
            IReadOnlyCollection<string> directoryNamesToIgnore)
        {
            var directories = sourceDir.EnumerateDirectories();
            if (directoryNamesToIgnore?.Count > 0)
                directories = directories.Where(x => !directoryNamesToIgnore.Contains(x.Name));

            return directories;
        }
    }
}