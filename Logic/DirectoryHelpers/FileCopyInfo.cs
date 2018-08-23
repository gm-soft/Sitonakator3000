using System.IO;

namespace Logic.DirectoryHelpers
{
    public class FileCopyInfo
    {
        private readonly FileInfo _fileInfo;
        private readonly string _copyDestinationFullFilePath;

        public FileCopyInfo(FileInfo fileInfo, string copyDestinationFullFilePath)
        {
            _fileInfo = fileInfo;
            _copyDestinationFullFilePath = copyDestinationFullFilePath;
        }

        public override string ToString()
        {
            return _copyDestinationFullFilePath;
        }
    }
}