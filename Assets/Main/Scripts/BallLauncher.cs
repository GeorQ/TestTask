using Mirror;
using Mirror.Examples.BilliardsPredicted;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class BallLauncher : NetworkBehaviour, ILaunch
{
    [SerializeField] private BallBase ballReference;
    [SerializeField] private Transform shootPoint;
    
    private byte _ownerID;
    private float forceAmmount = 40f;
    private Coroutine currentCoroutine;
    private float attackRate = 5f; //Attacks per second
    private float cdTime => 1.0f / attackRate; //Cooldown time 

    public Transform StartPoint => shootPoint;

    public event Action<float> OnReloadEvent;


    public void Initialize(byte ownerID)
    {
        _ownerID = ownerID;
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
        CmdSpawnBall(holdAmplification, shootPoint.forward);
    }

    [Command]
    public void CmdSpawnBall(float holdTime, Vector3 dir)
    {
        float holdAmplification = Mathf.Clamp(holdTime, 0.2f, 3.0f);
        //BallBase ball = Instantiate(ballReference, shootPoint.position, Quaternion.identity);
        //NetworkServer.Spawn(ball.gameObject);
        BallBase ball = PoolSystem.GetBallFromPool();
        RpcActivateBall(ball.gameObject);
        ball.ResetBall(shootPoint.position);
        ball.Push(dir, forceAmmount * holdAmplification, _ownerID);
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


    [ClientRpc]
    public void RpcActivateBall(GameObject objectToActivate)
    {
        objectToActivate.SetActive(true);
    }
}