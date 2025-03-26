using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift)) return;

        if (Input.GetKeyDown(KeyCode.F1)) Application.targetFrameRate = 10;
        if (Input.GetKeyDown(KeyCode.F2)) Application.targetFrameRate = 20;
        if (Input.GetKeyDown(KeyCode.F3)) Application.targetFrameRate = 30;
        if (Input.GetKeyDown(KeyCode.F4)) Application.targetFrameRate = 60;
        if (Input.GetKeyDown(KeyCode.F5)) Application.targetFrameRate = 900;
    }
}