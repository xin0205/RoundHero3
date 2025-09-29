using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TraverseAssets
{
    public static string path = "Assets/GameMain/Entities/Enemies/";

    [MenuItem("搜索工具/遍历预制体")]
    public static void TraversePrefab()
    {
        var allfiles = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);

        foreach (var file in allfiles)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            var r = FindDeepChild(go.transform, "WeaponR");
            //var c = r.GetComponentInChildren<Transform>();
            foreach (Transform cc in r)
            {
                Debug.Log(cc.name);
                cc.gameObject.SetActive(false);
            }
            PrefabUtility.SavePrefabAsset(go);
        }
        AssetDatabase.Refresh();
    }
    
    public static Transform FindDeepChild(Transform parent, string name)
    {
        // 首先检查当前父对象下是否有直接子对象匹配
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
        }
 
        // 如果没有找到，递归检查每个子对象
        foreach (Transform child in parent)
        {
            Transform found = FindDeepChild(child, name);
            if (found != null)
                return found;
        }
 
        // 如果没有找到，返回null
        return null;
    }
}

