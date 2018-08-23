using System.IO;

namespace Logic.DirectoryHelpers
{
    public class FileCopyInfo
    {
        private readonly FileInfo _fileInfo;
        private readonly string _copyDestinationFullFilePath;

        public FileCopyInfo(FileInfo fileInfo, DirectoryInfo destinationToCopy)
        {
            _fileInfo = fileInfo;
            _copyDestinationFullFilePath = Path.Combine(destinationToCopy.ToString(), fileInfo.Name);
        }

        public void Copy()
        {
            _fileInfo.CopyTo(destFileName: _copyDestinationFullFilePath, overwrite: true);
        }

        public override string ToString()
        {
            return _copyDestinationFullFilePath;
        }
    }
}