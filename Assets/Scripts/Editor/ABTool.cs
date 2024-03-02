using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR

/// <summary>
/// AB编辑器工具包
/// </summary>
public class ABTool : EditorWindow
{
    private string _src = "Assets/Prefabs/AssetBundle";   // 源文件夹路径
    private string _target = Application.streamingAssetsPath + "/Prefabs/AssetBundle";   // AB包输出文件夹路径
    private int _buildType = 1;

    [MenuItem("MineMenu/ABTool")]
    static void Init()
    {
        ABTool window = (ABTool)EditorWindow.GetWindow<ABTool>("ABTool");
        window.Show();
    }

    void OnGUI()
    {
        AssetBundleBuilder();
    }

    /// <summary>
    /// 打包
    /// </summary>
    void AssetBundleBuilder()
    {
        // 确保打包路径存在
        ABUtil.EnsureDirectory(_target);

        DivByLabel(10, 10, "========== AssetBundle Pack ==========");

        _src = EditorGUILayout.TextField("源文件夹:", _src);
        _target = EditorGUILayout.TextField("目标文件夹:", _target);
        _buildType = EditorGUILayout.Popup("打包方式:", _buildType, new string[] { "None", "ChunkBasedCompression" });

        /*----------------------[重置AB信息并打包]---------------------*/
        DivByLabel(5, 5, "重置所有资源的AB信息后再打包");

        // 点击打包，将资源打包输出到目标路径
        if (GUILayout.Button("重置AB信息并打包"))
        {
            // 打包
            BuildAsset(_src, _target, _buildType);
            // 刷新资源文件夹
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// 标签分割
    /// </summary>
    /// <param name="topMargin">距离上方的边距</param>
    /// <param name="bottomMargin">距离下方的边距</param>
    /// <param name="label">标签</param>
    void DivByLabel(float topMargin, float bottomMargin, string label)
    {
        GUILayout.Space(topMargin);
        GUILayout.Label(label, EditorStyles.boldLabel);
        GUILayout.Space(bottomMargin);
    }

    /// <summary>
    /// 打包方式对照字典
    /// </summary>
    /// <value></value>
    public static readonly Dictionary<int, BuildAssetBundleOptions> BUILD_TYPE_OPTION = new()
        {
            {0, BuildAssetBundleOptions.None},// "None"
            {1, BuildAssetBundleOptions.ChunkBasedCompression},// ChunkBasedCompression
        };

    /// <summary>
    /// 获取打包方式
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static BuildAssetBundleOptions GetBuildOption(int val)
        => BUILD_TYPE_OPTION.TryGetValue(val, out var option) ? option : BuildAssetBundleOptions.None;

    /// <summary>
    /// <para>将目标文件夹下所有的资源都用 AssetBundle 打包到目标文件夹</para>
    /// </summary>
    public static void BuildAsset(string target, int buildType)
    {
        BuildAssetBundleOptions options = GetBuildOption(buildType);

        BuildPipeline.BuildAssetBundles(target,
            options, EditorUserBuildSettings.activeBuildTarget);
    }

    /// <summary>
    /// 清空目标路径下的所有所有资源的AssetBundle名称和变体(我理解成后缀，应该是)
    /// </summary>
    /// <param name="src"></param>
    public static void ClearAssetBundleNameAndVariant(string src)
    {
        foreach (string itemPath in Directory.GetFiles(src))
        {
            // 判断当前文件是否可以被打包
            if (NeedToHandle(itemPath))
                SetNameAndVariant(itemPath, "", "");
        }

        // 递归调用遍历执行该函数，将子文件夹下所有文件的打包资源名称和后缀清空
        foreach (string itemPath in Directory.GetDirectories(src))
        {
            ClearAssetBundleNameAndVariant(itemPath);
        }

        // 清空无用的名称
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    /// <summary>
    /// <para>重写打包资源的名称和后缀</para>
    /// <para>文件命名规则: 文件原名称.u3d</para>
    /// </summary>
    public static void ReWriteAssetNameAndVariant(string src, string target, int buildType)
    {
        // 如果不是文件夹，不执行以下操作
        if (!ABUtil.IsDirectory(src)) return;

        // 确保目标文件夹存在，如果不存在，自动创建
        ABUtil.EnsureDirectory(target);

        foreach (string itemPath in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
        {
            // 判断当前文件是否可以被打包，如果可以则设置打包资源名称(文件名.u3d)
            if (NeedToHandle(itemPath))
                SetNameAndVariant(itemPath, Path.GetFileNameWithoutExtension(itemPath), ABConst.SUFFIX_U3D);
        }

        // 递归调用遍历执行该函数，递归遍历子文件夹，将子文件夹下所有文件打包至目标文件夹
        foreach (string itemPath in Directory.GetDirectories(src))
            ReWriteAssetNameAndVariant(itemPath, target, buildType);
    }

    /// <summary>
    /// 设置打包资源名称和后缀
    /// </summary>
    public static void SetNameAndVariant(string path, string name, string variant)
    {
        AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(
            /* 如果资源名称是以Mat开头的，那么就视为材质，全都打包到Mat.u3d
                否则按照名称分开打包 */
            name.StartsWith(ABConst.PREFIX_MAT) ? ABConst.PREFIX_MAT : name,
            variant);
    }

    /// <summary>
    /// 确保这个是需要操作的文件
    /// <para>1: 不是 .meta 文件</para>
    /// <para>2: 不是文件夹</para>
    /// </summary>
    /// <param name="filePath"></param>
    public static bool NeedToHandle(string filePath)
        => !string.IsNullOrEmpty(filePath)
        && !filePath.EndsWith(ABConst.SUFFIX_META)
        && !filePath.EndsWith(ABConst.SUFFIX_DS_Store)
        && File.Exists(filePath);

    /// <summary>
    /// <para>将目标文件夹下所有的资源都用 AssetBundle 打包到目标文件夹</para>
    /// </summary>
    public static void BuildAsset(string src, string target, int buildType)
    {
        // 给打包资源统一命名
        ReWriteAssetNameAndVariant(src, target, buildType);

        // 将资源打包
        BuildAsset(target, buildType);
    }

}

#endif