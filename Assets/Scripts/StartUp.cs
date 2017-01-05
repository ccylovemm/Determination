using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(0.1f);
        #if UNITY_EDITOR
                CheckVersionFinish();
        #else
                VersionManager.Inst.CheckVersion(CheckVersionFinish);
        #endif
    }

    void CheckVersionFinish()
    {
        ResourceManager.Instance.LoadScene("MianScene", LoadSceneComplete);
    }

    void LoadSceneComplete(GameObject o)
    {
        ResourceManager.Instance.LoadAsset("AiXi" , delegate(GameObject go)
        {
            go.transform.position = new Vector3(0 , 0 , 0);
            CameraManager.Instance.FollowTarget(go);
        });
    }
}
