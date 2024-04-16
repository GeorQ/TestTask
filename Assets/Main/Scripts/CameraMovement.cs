using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    public void CameraInit(Transform playerTransform)
    {
        transform.SetParent(playerTransform);

        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }
}




    //private void Awake()
    //{
    //    //if (!isLocalPlayer) { return; }
    //    Debug.Log("Awake");
    //    //playerInput = GetComponent<IInput>();
    //    //ballLauncher = GetComponent<ILaunch>();

    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;

    //    initialRotationX = transformGun.localRotation.eulerAngles.x;
    //    initialRotationZ = transformBase.localRotation.eulerAngles.z;
    //}
