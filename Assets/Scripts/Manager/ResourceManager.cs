using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ResourceManager : Singleton<ResourceManager>
{
    public delegate void LoadComplete(GameObject o);

    public byte[] ReadByte(string fileName)
    {
        return System.IO.File.ReadAllBytes(VersionManager.GetConfigPath(fileName));
    }

    public string ReadText(string fileName)
    {
        return System.IO.File.ReadAllText(VersionManager.GetConfigPath(fileName));
    }

    public void LoadScene(string strName, LoadComplete callBack)
    {
        StartCoroutine(LoadAssets(strName, callBack, true));
    }

    public void LoadAsset(string strName, LoadComplete callBack)
    {
        StartCoroutine(LoadAssets(strName, callBack, false));
    }

    IEnumerator LoadAssets(string strName, LoadComplete callBack , bool isScene)
    {
        WWW www = WWW.LoadFromCacheOrDownload(VersionManager.GetResPath("AssetBundle"), 0);
        yield return www;
        List<AssetBundle> assetBundleList = new List<AssetBundle>();
        AssetBundle assetBundle = www.assetBundle;
        assetBundleList.Add(assetBundle);
        AssetBundleManifest abManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        foreach (var dependAsset in abManifest.GetAllDependencies(strName.ToLower()))
        {
            WWW dependWWW = WWW.LoadFromCacheOrDownload(VersionManager.GetResPath(dependAsset), 0);
            yield return dependWWW;
            assetBundleList.Add(dependWWW.assetBundle);
        }

        WWW asset = WWW.LoadFromCacheOrDownload(VersionManager.GetResPath(strName.ToLower()), 0);
        yield return asset;
        AssetBundle assetAB = asset.assetBundle;
        assetBundleList.Add(assetAB);
        if (isScene)
        {
            SceneManager.LoadScene(strName);
            callBack(null);
        }
        else
        {
            GameObject go = assetAB.LoadAsset<GameObject>(strName);
            callBack(GameObject.Instantiate(go));
        }
        foreach (var ab in assetBundleList)
        {
            if (ab != null)
            {
                ab.Unload(false);
            }
        }
    }
}
