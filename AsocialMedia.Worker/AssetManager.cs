namespace AsocialMedia.Worker;

public class AssetManager
{
    private const string Root = "assets";

    static AssetManager()
    {
        var isExist = Directory.Exists(Root);

        if (isExist)
            Directory.Delete(Root, true);

        Directory.CreateDirectory(Root);
    }

    public static string CreateResource()
    {
        var resourceId = Guid.NewGuid().ToString();
        return resourceId;
    }

    public static string CreateResourceGroup()
    {
        var resourceGroupId = Guid.NewGuid().ToString();
        var resourceGroupPath = GetResourceGroupById(resourceGroupId);
        Directory.CreateDirectory(resourceGroupPath);
        return resourceGroupId;
    }

    public static void DeleteResourceGroupById(string resourceGroupId)
    {
        Directory.Delete($"{Root}/{resourceGroupId}", true);
    }

    public static string GetResourceGroupById(string resourceGroupId)
    {
        return $"{Root}/{resourceGroupId}";
    }

    public static string GetResourcePathById(string resourceGroupId, string resourceId)
    {
        return $"{GetResourceGroupById(resourceGroupId)}/{resourceId}";
    }

    public static string GetResource(string resourceGroupId, string resourceId)
    {
        var resourceGroupPath = GetResourceGroupById(resourceGroupId);
        return Directory.GetFiles(resourceGroupPath).First(file => file.Contains(resourceId));
    }
}