using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIRaycast : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Check();
        }
    }
    private void Check()
    {
        Camera.main.cullingMask = 1 << LayerMask.NameToLayer("ScreenShotUI");
    }
}
