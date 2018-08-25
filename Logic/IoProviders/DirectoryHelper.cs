using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.IoProviders
{
    public class DirectoryHelper
    {
        private readonly IReadOnlyCollection<string> _directoryNamesToIgnore;

        public DirectoryHelper(IReadOnlyCollection<string> directoryNamesToIgnore)
        {
            _directoryNamesToIgnore = directoryNamesToIgnore
                ?? throw new ArgumentNullException(paramName: nameof(directoryNamesToIgnore));
        }

        public bool CheckSourceDirectoryForContent(string directoryPath)
        {
            var sourceDirInfo = new DirectoryInfo(directoryPath);

            return !sourceDirInfo.EnumerateFiles().Any() || !GetSubDirectoriesWithoutIngored(sourceDirInfo).Any();
        }

        public IEnumerable<DirectoryInfo> GetSubDirectoriesWithoutIngored(DirectoryInfo sourceDir)
        {
            return sourceDir.EnumerateDirectories().Where(x => !_directoryNamesToIgnore.Contains(x.Name));
        }

        public async Task RemoveAllContentAsync(string directoryPath)
        {
            await Task.Run(() =>
            {
                var directoryInfo = new DirectoryInfo(directoryPath);

                // https://stackoverflow.com/a/1288747
                foreach (FileInfo file in directoryInfo.EnumerateFiles())
                    file.Delete();

                IEnumerable<DirectoryInfo> dirsToRemove = GetSubDirectoriesWithoutIngored(directoryInfo);

                foreach (DirectoryInfo dir in dirsToRemove)
                    dir.Delete(true);
            });
        }
    }
}