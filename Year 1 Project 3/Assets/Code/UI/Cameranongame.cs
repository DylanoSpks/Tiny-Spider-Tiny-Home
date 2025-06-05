using UnityEngine;

public class Cameranongame : MonoBehaviour
{
    private void Update()
    {
     Cursor.lockState = CursorLockMode.None;
     Cursor.visible = true;
    }
}
