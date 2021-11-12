namespace KamenReader;

public interface IFileReader {
    FileReaderResult Read(String fullFilepath, IList<FileReaderMap> maps, Boolean firstRowAreTitles = true);
}