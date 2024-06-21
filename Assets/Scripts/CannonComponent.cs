using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AimingComponent))]
[RequireComponent(typeof(LineRenderer))]
public class CannonComponent : MonoBehaviour
{

    public int CurrentTilt
    {
        set
        {
            currentTilt = value;
            aimingComponent.generatePreviewSpline(currentTilt, lineRenderer, ballPreview.gameObject, 10);
        }
    }

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
    private MeshFilter ballPreview;

    private AimingComponent aimingComponent;

    private LineRenderer lineRenderer;

    private int currentTilt;

    private bool canFire = false;

    private Sprite ballTexture;

    // Start is called before the first frame update
    void Start()
    {
        aimingComponent = GetComponent<AimingComponent>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void StartLoading()
    {
        canFire = true;
    }

    public void ChargeCannon(Sprite texture)
    {
        ballTexture = texture;
    }

    public void FireCannon()
    {
        if(!canFire)
        {
            return;
        }

        CannonBallComponent cannonBall = Instantiate<CannonBallComponent>(cannonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        cannonBall.SplineData = aimingComponent.getSplineData(currentTilt);
        cannonBall.gameObject.transform.localScale *= ballTexture ? 1f : 0.5f;
        cannonBall.BallRenderer.sprite = ballTexture ? ballTexture : defaultImage;

        if (ballTexture)
        {
            ballTexture = null;
        }
    }
}
