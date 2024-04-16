using Mirror;
using Mirror.Examples.BilliardsPredicted;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class BallLauncher : NetworkBehaviour, ILaunch
{
    [SerializeField] private GameObject ballReference;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Image reloadImage;

    private Camera mainCamera;
    private float forceAmmount = 40f;
    private Coroutine currentCoroutine;
    private float attackRate = 5f; //Attacks per second
    private float cdTime => 1.0f / attackRate; //Cooldown time 

    public event Action<float> OnReloadEvent;


    public override void OnStartLocalPlayer()
    {
        mainCamera = Camera.main;
    }

    public void Launch(float holdTime)
    {
        //Can not shoot since on cool down
        if (currentCoroutine != null)
        {
            return;
        }
        
        currentCoroutine = StartCoroutine(Reload());
        float holdAmplification = Mathf.Clamp(holdTime, 0.2f, 3.0f);
        //GameObject ball = Instantiate(ballReference, shootPoint.position, UnityEngine.Random.rotation);
        //ball.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * forceAmmount * holdAmplification, ForceMode.Impulse);
        CmdSpawnBall(holdAmplification, shootPoint.forward);
    }

    [Command]
    public void CmdSpawnBall(float holdTime, Vector3 dir)
    {
        float holdAmplification = Mathf.Clamp(holdTime, 0.2f, 3.0f);
        GameObject ball = Instantiate(ballReference, shootPoint.position, Quaternion.identity);
        NetworkServer.Spawn(ball);
        ball.GetComponent<Rigidbody>().AddForce(dir * 40 * holdAmplification, ForceMode.Impulse);
    }

    private IEnumerator Reload()
    {
        float startReloadTime = Time.time;

        while (Time.time - startReloadTime < cdTime)
        {
            float reloadProgress = (Time.time - startReloadTime) / cdTime;
            OnReloadEvent?.Invoke(reloadProgress);
            yield return null;
        }
        
        currentCoroutine = null;
    }
}