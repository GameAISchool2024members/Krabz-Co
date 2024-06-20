using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CannonComponent))]
public class AimTestComponent : MonoBehaviour
{

    private CannonComponent cannon;

    // Start is called before the first frame update
    void Start()
    {
        cannon = GetComponent<CannonComponent>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            cannon.CurrentTilt = 0;
            cannon.FireCannon(null);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            cannon.CurrentTilt = 1;
            cannon.FireCannon(null);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            cannon.CurrentTilt = 2;
            cannon.FireCannon(null);
        }
    }
}
