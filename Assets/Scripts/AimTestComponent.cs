using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTestComponent : MonoBehaviour
{
    public AimingComponent aiming;

    public GameObject test;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            test.transform.localScale = new Vector3(1, 1, 1);
            aiming.AttachObject(new AimingComponent.AttachableObject(test, 0));
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            test.transform.localScale = new Vector3(1, 1, 1);
            aiming.AttachObject(new AimingComponent.AttachableObject(test, 1));
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            test.transform.localScale = new Vector3(1, 1, 1);
            aiming.AttachObject(new AimingComponent.AttachableObject(test, 2));
        }
    }
}
