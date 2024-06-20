using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AimingComponent))]
public class CannonComponent : MonoBehaviour
{
    public int CurrentTilt
    {
        set
        {
            currentTilt = value;
        }
    }

    [SerializeField]
    private GameObject cannonPrefab;

    private AimingComponent aimingComponent;

    private int currentTilt;

    // Start is called before the first frame update
    void Start()
    {
        aimingComponent = GetComponent<AimingComponent>();
    }

    public void FireCannon(Texture2D texture)
    {
        GameObject cannonBall = Instantiate(cannonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        aimingComponent.AttachObject(new AimingComponent.AttachableObject(cannonBall, currentTilt));
    }
}
