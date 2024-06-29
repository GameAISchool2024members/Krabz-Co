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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<RequestProcessor>().RequestImage("Furry lobster", cannon);
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            cannon.StartLoading();
            cannon.FireCannon();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            cannon.StartLoading();
            cannon.FireCannon();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            cannon.StartLoading();
            cannon.FireCannon();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            cannon.StartLoading();
            cannon.FireCannon();
        }
    }
}
