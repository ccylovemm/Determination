using UnityEngine;
using System.Collections.Generic;

public class WindowManager : Singleton<WindowManager> 
{
    public Transform parent;

    private Dictionary<string, GameObject> windowDic = new Dictionary<string, GameObject>();

    public void OpenWindow(string key, params object[] args)
    {
        GameObject wind;
        if (windowDic.TryGetValue(key, out wind))
        {
            wind.SetActive(true);
            wind.BroadcastMessage("Call", args, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            ResourceManager.Instance.LoadAsset(key, obj => 
            {
                obj.transform.parent = parent;
                obj.transform.localPosition = new Vector3(0, 0, wind.transform.position.z);
                obj.transform.localScale = new Vector3(1, 1, 1);
                windowDic[key] = obj;
                obj.BroadcastMessage("Call", args, SendMessageOptions.DontRequireReceiver);
            });
        }
    }

    public void CloseWindow(string key)
    {
        GameObject o;
        if (windowDic.TryGetValue(key, out o))
        {
            o.SetActive(false);
        }
    }

    public void CloseAllWindow()
    {
        foreach (GameObject o in windowDic.Values)
        {
            o.gameObject.SetActive(false);
        }
    }
}
