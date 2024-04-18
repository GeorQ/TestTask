using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolSystem : NetworkBehaviour
{
    [SerializeField] private BallBase ballPrefab;
    private int _poolSize = 30;
    private static Queue<BallBase> _pool = new Queue<BallBase>();
        

    public override void OnStartServer()
    {
        StartCoroutine(FillPool());
    }

    private IEnumerator FillPool()
    {
        yield return null;
        for (int i = 0; i < _poolSize; i++)
        {
            BallBase ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(ball.gameObject);
            _pool.Enqueue(ball);
        }
    }

    public static BallBase GetBallFromPool()
    {
        if (_pool.Count == 0)
        {
            //Pizdec impossible task
        }
        BallBase temp = _pool.Dequeue();
        _pool.Enqueue(temp);
        return temp;
    }
}