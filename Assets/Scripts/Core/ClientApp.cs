using UnityEngine;
using System.Collections;

public class ClientApp : MonoBehaviour {

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
        CheckVersionFinish();
#else
        VersionManager.Inst.CheckVersion(CheckVersionFinish);
#endif
    }

    void CheckVersionFinish()
    {
        SocketClient.Connect();
    }
}
