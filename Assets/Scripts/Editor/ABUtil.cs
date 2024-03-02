using System.IO;

/// <summary>
/// <para>AssetBundle打包工具类</para>
/// </summary>
public class ABUtil
{
    /// <summary>
    /// 判断是否是文件夹
    /// </summary>
    /// <param name="dir">目标文件路径</param>
    public static bool IsDirectory(string dir)
        => !"".Equals(dir) && Directory.Exists(dir);


    /// <summary>
    /// 确保文件夹路径存在，如不存在，则创建
    /// </summary>
    /// <param name="dir">目标文件路径</param>
    public static void EnsureDirectory(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
}
