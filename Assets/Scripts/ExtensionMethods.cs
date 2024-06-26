using System;
using UnityEngine;

public static class ExtensionMethod
{
    public static CannonBallComponent Instantiate(this CannonBallComponent thisObj, AimingComponent.SplineData splineData, Sprite ballTexture)
    {
        CannonBallComponent cannonBall = GameObject.Instantiate<CannonBallComponent>(thisObj, new Vector3(0, 0, 0), Quaternion.identity);
        cannonBall.SplineData = splineData;
        cannonBall.gameObject.transform.localScale *= ballTexture ? 1.5f : 0.8f;

        if(ballTexture)
        {
            cannonBall.BallRenderer.sprite = ballTexture;
        }

        return cannonBall;
    }
}