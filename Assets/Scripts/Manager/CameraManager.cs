using UnityEngine;
using System.Collections;

public class CameraManager : Singleton<CameraManager> 
{
    public float zoomMin;
    public float zoomMax;
    public float scrollSpeed;

    private Vector3 oldPos1 = Vector3.zero;
    private Vector3 oldPos2 = Vector3.zero;
    private float distance;
    private float distLerp;

    public GameObject target;

    void Start()
    {
        distance = (zoomMax + zoomMin) / 2.0f;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPos = CalDistance(gameObject, target);
            var isTweening = (gameObject.transform.position - targetPos).magnitude > 0.02;
            if (isTweening)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPos, 0.06f);
            }
        }
    }

    public void LookTarget(Vector3 value)
    {
        target = null;
        Vector3 targetPos = CalDistance(gameObject, value);
        gameObject.transform.position = targetPos;
    }

    public void LookTarget(GameObject value)
    {
        target = null;
        Vector3 targetPos = CalDistance(gameObject, target);
        gameObject.transform.position = targetPos;
    }

    public void FollowTarget(GameObject value)
    {
        target = value;
    }

    public void RotationCamera(Quaternion rotation)
    {
        gameObject.transform.rotation = rotation;
    }

    public void MoveCamera(Vector3 pos, float time = 1.0f)
    {
        target = null;
        if (time == 0)
        {
            gameObject.transform.position = pos;
        }
        else
        {
            TweenPosition.Begin(gameObject, time, pos);
        }
    }

    public void ZoomCamera(float dis)
    {
        distance = ScrollLimit(dis, zoomMin, zoomMax);
    }

    Vector3 CalDistance(GameObject cameraObj, GameObject tar)
    {
        if (target == null) return Vector3.zero;
        return CalDistance(cameraObj, tar.transform.position);
    }

    Vector3 CalDistance(GameObject cameraObj, Vector3 tar_position)
    {
        distLerp = distance;
        Vector3 calPos = new Vector3(0, 0, -distLerp);
        Vector3 position = cameraObj.transform.rotation * calPos + tar_position;
        return position;
    }

    float ScrollLimit(float dist, float min, float max)
    {
        if (dist < min)
            dist = min;
        if (dist > max)
            dist = max;
        return dist;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
