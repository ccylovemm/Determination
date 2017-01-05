using UnityEngine;
using System.Collections.Generic;

public class AnimationManager : MonoBehaviour 
{
    private Animation animation;

    void Awake()
    {
        animation = GetComponent<Animation>();
    }

    public void PlayAnimation(string animationName)
    {
        animation.CrossFade(animationName);
    }

    public void SetSpeed(string animationName , float speed)
    {
        animation[animationName].speed = speed;
    }
}