using Mirror;
using UnityEngine;


[RequireComponent(typeof(IInput), typeof(ILaunch))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform cameraSpot;

    private GunRotator gunRotator;
    private IInput playerInput;
    private ILaunch _launcher;
    private TimeCounter _timeCounter;
    private GameObject scoreManager;

    public void Initialize(IInput input, TimeCounter timeCounter, GunRotator gunRotator, ILaunch launcher)
    {
        Camera.main.GetComponent<CameraMovement>().CameraInit(cameraSpot);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput = input;
        _timeCounter = timeCounter;
        _launcher = launcher;
        _timeCounter.TimerEnd += Shoot;
        this.gunRotator = gunRotator;

        scoreManager = FindFirstObjectByType<ScoreManager>().transform.GetChild(0).gameObject; //TODO  - remove and find better way to get obj
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreManager.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreManager.SetActive(false);
        }

        gunRotator.Rotate(playerInput.GetPointerDelta());
    }

    private void Shoot(float holdTime)
    {
        if (holdTime > 0)
        {
            _launcher.Launch(holdTime);
        }
    }

    private void OnDestroy()
    {
        //_timeCounter.TimerEnd -= Shoot;
    }
}