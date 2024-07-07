using System;
using UnityEngine;

public static class ExtensionMethod
{
    public static CannonBallComponent Instantiate(this CannonBallComponent thisObj, AimingComponent.SplineData splineData, Sprite ballTexture)
    {
        CannonBallComponent cannonBall = GameObject.Instantiate<CannonBallComponent>(thisObj, new Vector3(0, 0, 0), Quaternion.identity);
        cannonBall.SplineData = splineData;
        cannonBall.SetBallSprite(ballTexture);

        return cannonBall;
    }
}