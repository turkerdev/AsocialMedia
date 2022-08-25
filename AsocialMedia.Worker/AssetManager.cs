namespace AsocialMedia.Worker;

public class AssetManager
{
    private static bool _isInitialized = false;
    private const string Path = "assets";
    
    public static void Initialize()
    {
        var isExist = Directory.Exists(Path);
        
        if (isExist)
            Directory.Delete(Path, true);
        
        Directory.CreateDirectory(Path);
        _isInitialized = true;
    }

    public static (string, string) CreateOne()
    {
        if(!_isInitialized)
            throw new Exception("AssetManager is not initialized");
            
        var directoryName = Guid.NewGuid().ToString();
        var directory = $"{Path}/{directoryName}";
        Directory.CreateDirectory(directory);
        return (directory, directoryName);
    }

    public static void DeleteOne(string path)
    {
        if(!_isInitialized)
            throw new Exception("AssetManager is not initialized");
        
        Directory.Delete(path, true);
    }
}