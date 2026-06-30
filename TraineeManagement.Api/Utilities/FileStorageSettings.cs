public class FileStorageSettings
{
    public string RootPath {get; set;}="";
    public long MaxFileSizeBytes {get; set;}=0;
    public List<string> AllowedExtensions {get; set;} = [];
    public List<string> AllowedContentTypes {get; set;} = [];
}