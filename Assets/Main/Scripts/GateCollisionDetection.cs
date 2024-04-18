using Mirror;
using UnityEngine;


public class GateCollisionDetection : NetworkBehaviour
{
    [SerializeField] private GameObject particlePrefab;

    private Rigidbody rb;
    private float _pushForce = 10f;
    private byte _ownerID;

    public event System.Action<byte, byte> OnPlayerScored;


    [Server]
    public void Initialize(byte ownerID)
    {
        // Cashe rb on server Start
        rb = GetComponent<Rigidbody>();
        _ownerID = ownerID;
        PushBox((new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized));
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        BallBase ball = collision.transform.GetComponent<BallBase>();
        if (ball)
        {
            //NetworkServer.Destroy(collision.gameObject); //Pooled
            OnPlayerScored?.Invoke(ball.GetOwnerID(), _ownerID);
            RpcParticleExplosion(collision.contacts[0].point);
            PushBox(collision.relativeVelocity);
            return;
        }

        PushBox(collision.contacts[0].normal);
    }

    private void PushBox(Vector3 direction)
    {
        rb.AddForce(direction * _pushForce, ForceMode.VelocityChange);
    }

    [ClientRpc]
    public void RpcParticleExplosion(Vector3 position)
    {
        Destroy(Instantiate(particlePrefab, position, Quaternion.identity), 2);
    }
}