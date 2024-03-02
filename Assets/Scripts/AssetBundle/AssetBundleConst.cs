using UnityEngine;

/// <summary>
/// AssetBundle 常量
/// </summary>
public class AssetBundleConst
{
    /// <summary>
    /// 需要打包的预制体存放路径
    /// </summary>
    public static readonly string PATH_PREFAB = "Assets/Prefabs/AssetBundle";
    
    /// <summary>
    /// AB资源-预制体-打包之后的存放路径
    /// </summary>
    public static readonly string PATH_PREFAB_AB = Application.streamingAssetsPath + "/Prefabs/AssetBundle";

    /// <summary>
    /// manifest文件路径
    /// </summary>
    public static readonly string MANI_PATH = PATH_PREFAB_AB + "/AssetBundle";
    
    /// <summary>
    /// AssetBundleManifest 名称
    /// </summary>
    public static readonly string MANI_NAME = "AssetBundleManifest";

    /// <summary>
    /// 打包方式
    /// </summary>
    /// <value></value>
    public static readonly string[] BUILD_TYPE = new string[] { "None", "ChunkBasedCompression" };

    /// <summary>
    /// 根据文件名称获取打包资源的路径-带后缀
    /// </summary>
    /// <returns></returns>
    public static string GetABPath(string name) => $"{PATH_PREFAB_AB}/{name}.u3d";

    /// <summary>
    /// 根据文件名称获取打包资源的路径-不带后缀
    /// </summary>
    /// <returns></returns>
    public static string GetABPathWithoutVariant(string name) => $"{PATH_PREFAB_AB}/{name}";

}
