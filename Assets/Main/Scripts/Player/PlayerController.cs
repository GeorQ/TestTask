using Mirror;
using UnityEngine;


[RequireComponent(typeof(IInput), typeof(ILaunch))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform transformGun;
    [SerializeField] private Transform transformBase;
    [SerializeField] private Transform cameraSpot;

    private GunRotator gunRotator;
    private IInput playerInput;
    private ILaunch ballLauncher;
    private TimeCounter _timeCounter;


    public void Initialize(IInput input, TimeCounter timeCounter, GunRotator gunRotator)
    {
        Camera.main.GetComponent<CameraMovement>().CameraInit(cameraSpot);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput = input;
        _timeCounter = timeCounter;
        ballLauncher = GetComponent<ILaunch>();
        _timeCounter.TimerEnd += Shoot;
        this.gunRotator = gunRotator;
    }

    private void Update()
    {
        if (!isOwned) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Hello world");
        }

        gunRotator.Rotate(playerInput.GetPointerDelta());
    }

    private void Shoot(float holdTime)
    {
        Debug.Log(holdTime);
        if (holdTime > 0)
        {
            ballLauncher.Launch(holdTime);
        }
    }

    private void OnDestroy()
    {
        //_timeCounter.TimerEnd -= Shoot;
    }
}