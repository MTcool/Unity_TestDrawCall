using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// AB包工具类
/// </summary>
public class AssetBundleUtility
{
    /*--------------------[释放资源]--------------------*/

    /// <summary>
    /// 释放AssetBundle资源
    /// </summary>
    /// <param name="assetBundle"></param>
    public static void Release(AssetBundle assetBundle)
    {
        try
        {
            if (assetBundle)
                assetBundle.Unload(false);
        }
        catch (Exception e)
        {
            throw new Exception("### 释放资源异常: " + e.ToString());
        }
    }
}
