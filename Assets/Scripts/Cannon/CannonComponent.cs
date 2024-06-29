using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AimingComponent))]
public class CannonComponent : MonoBehaviour, RequestProcessor.IImageReceiver, RequestProcessor.IFireReceiver
{
    public bool CanFire
    {
        get
        {
            return canFire;
        }
    }

    [SerializeField]
    private CannonBallComponent cannonPrefab;

    [SerializeField]
    private Sprite defaultImage;

    [SerializeField]
    private AudioClip cannonBallReadySound;

    private AudioSource audioSource;

    private AimingComponent aimingComponent;

    private bool canFire = false;

    private Sprite ballTexture;

    // Start is called before the first frame update
    private void Start()
    {
        aimingComponent = GetComponent<AimingComponent>();
        audioSource = GetComponent<AudioSource>();
    }

    public void StartLoading()
    {
        canFire = true;
    }

    public void Fire()
    {
        StartLoading();
        FireCannon();
    }

    public void SetImage(Sprite texture)
    {
        ballTexture = texture;
        EventManager.ImageGenerated();

        audioSource.PlayOneShot(cannonBallReadySound);
    }

    public void FireCannon()
    {
        if(!canFire)
        {
            return;
        }

        CannonBallComponent cannonBall = cannonPrefab.Instantiate(aimingComponent.getSplineData(), ballTexture);

        if (ballTexture)
        {
            //ballTexture = null;
        }
    }
}
