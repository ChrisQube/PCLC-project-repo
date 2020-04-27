using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMovement : MonoBehaviour
{
    public LeanTweenType easeType;
    public float fYMovement;
    public float fYMoveDuration;

    private void OnEnable()
    {
        LeanTween.moveLocalY(gameObject, fYMovement, fYMoveDuration).setLoopPingPong().setEase(easeType);
    }
}
