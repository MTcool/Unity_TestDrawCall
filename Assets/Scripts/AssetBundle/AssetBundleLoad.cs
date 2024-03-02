using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

    /// <summary>
    /// AB包资源加载工具类
    /// </summary>
    public class AssetBundleLoad
    {
        static Dictionary<string, string[]> _dependencyDic = new (); // 依赖对照字典

        /// <summary>
        /// 根据资源路径加载资源预制体
        /// </summary>
        public static T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            T t = null;
            string[] dependencies = null;
            AssetBundle assetBundle = null;
            
            try
            {
                /*----------[从文件读取(快)]----------*/
                // 加载依赖
                dependencies = GetDependencies(name);

                if (dependencies != null)
                {
                    for (var i = 0; i < dependencies.Length; i++)
                    {
                        AssetBundle.LoadFromFile(AssetBundleConst.GetABPathWithoutVariant(dependencies[i]));
                    }
                }

                // 加载资源
                assetBundle = AssetBundle.LoadFromFile(AssetBundleConst.GetABPath(name));

                t = assetBundle.LoadAsset<T>(name);
            }
            catch (Exception e)
            {
                throw new Exception("### 获取AB资源异常: " + e.ToString());
            }
            finally
            {
                // 目前来看只有这样释放资源才是最合适的
                AssetBundle.UnloadAllAssetBundles(false);
            }

            return t;
        }

        /// <summary>
        /// 获取指定资源的依赖
        /// </summary>
        /// <param name="name"></param>
        public static string[] GetDependencies(string name)
        {
            try
            {
                if (_dependencyDic == null || _dependencyDic.Count == 0)
                    _dependencyDic = InitAssetDependencies();

                return _dependencyDic != null && _dependencyDic.TryGetValue(name.ToLower(), out var val)
                    ? val : new string[0];
            }
            catch (Exception e)
            {
                throw new Exception("### 获取资源依赖异常: " + e.ToString());
            }
        }

        /// <summary>
        /// 初始化所有资源的依赖
        /// </summary>
        public static Dictionary<string, string[]> InitAssetDependencies()
        {
            string[] assets = null;
            AssetBundle assetBundle = null;
            AssetBundleManifest mani = null;
            Dictionary<string, string[]> dic = null;
            try
            {
                dic = new Dictionary<string, string[]>();

                // 加载AssetBundleManifest
                assetBundle = AssetBundle.LoadFromFile(AssetBundleConst.MANI_PATH);
                mani = assetBundle.LoadAsset<AssetBundleManifest>(AssetBundleConst.MANI_NAME);

                // 获取所有的AB资源名称
                assets = mani.GetAllAssetBundles();

                // 循环遍历初始化AB资源与其依赖的对照字典
                foreach (var asset in assets)
                {
                    dic.Add(asset.Replace(".u3d", ""), mani.GetAllDependencies(asset));
                }
            }
            catch (Exception e)
            {
                throw new Exception("### 资源依赖初始化异常: " + e.ToString());
            }
            finally
            {
                AssetBundleUtility.Release(assetBundle);
            }

            return dic;
        }

    }
