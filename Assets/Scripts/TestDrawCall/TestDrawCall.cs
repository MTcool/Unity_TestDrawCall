using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 创建资源方式
/// </summary>
public enum CreateType
{
    /// <summary>
    /// AB 包加载资源
    /// </summary>
    AB = 0,

    /// <summary>
    /// GameObject.Instantiate
    /// </summary>
    Instantiate = 1,
}

/// <summary>
/// 测试 DrawCall
/// </summary>
public class TestDrawCall : MonoBehaviour
{
    [Tooltip("资源名")]
    [SerializeField] private string _name = "Capsule";

    [Tooltip("预制体")]
    [SerializeField] private GameObject _prefab;

    [Tooltip("生成资源区域尺寸")]
    [SerializeField] private Vector2 _scale = new(10, 10);

    [Tooltip("生成资源方式")]
    [SerializeField] private CreateType _createType;

    private void Start()
    {
        List<Vector3> pos = GetCreatePos();

        for (int i = 0; i < pos.Count; i++)
        {
            GameObject obj;

            if (_createType == CreateType.AB)
            {
                obj = Instantiate(AssetBundleLoad.LoadAsset<GameObject>(_name));
            }
            else
            {
                obj = Instantiate(_prefab);
            }

            obj.transform.position = pos[i];
        }
    }

    /// <summary>
    /// 获取创建资源的位置
    /// </summary>
    private List<Vector3> GetCreatePos()
    {
        List<Vector3> pos = new();

        int x = (int)_scale.x;
        int y = (int)_scale.y;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                pos.Add(new(i, 0, j));
            }
        }

        return pos;
    }


}
