using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AimingComponent))]
[RequireComponent(typeof(AudioSource))]
public class CannonComponent : MonoBehaviour
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
    private AudioClip cannonBallReadySound;

    private AudioSource audioSource;

    private AimingComponent aimingComponent;

    private Animator animatorComponent;

    private bool canFire = false;

    private Sprite ballTexture;

    private void OnEnable()
    {
        EventManager.OnFire += Fire;
        EventManager.OnImageGenerated += SetImage;
    }

    private void OnDisable()
    {
        EventManager.OnFire -= Fire;
        EventManager.OnImageGenerated -= SetImage;

    }
    // Start is called before the first frame update
    private void Start()
    {
        aimingComponent = GetComponent<AimingComponent>();
        animatorComponent = GetComponent<Animator>();
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

        audioSource.PlayOneShot(cannonBallReadySound);
    }

    public void FireCannon()
    {
        if(!canFire)
        {
            return;
        }

        animatorComponent.SetTrigger("Fire");
        CannonBallComponent cannonBall = cannonPrefab.Instantiate(aimingComponent.getSplineData(), ballTexture);

        if (ballTexture)
        {
            //ballTexture = null;
        }
    }
}
