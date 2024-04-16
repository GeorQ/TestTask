using Mirror;
using UnityEngine;


public class GateCollisionDetection : NetworkBehaviour
{
    private Rigidbody rb;
    private float _pushForce = 20f;


    public override void OnStartServer()
    {
        // Cashe rb on server Start
        rb = GetComponent<Rigidbody>();
        PushBox((new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized));
    }

    private void OnCollisionEnter(Collision collision)
    {
        //This objects should be in rigid sync with server, so better idea to process collision on server
        if (isServer)
        {
            //Only destroy balls on collision, so better to check tag rather than use INTERFACES, since getcomponent is not cheap
            if (collision.transform.CompareTag("Ball"))
            {
                NetworkServer.Destroy(collision.gameObject); //Pooled
                //Point Logic
                return;
            }

            PushBox(collision.contacts[0].normal);
        }
    }

    private void PushBox(Vector3 direction)
    {
        rb.AddForce(direction * _pushForce, ForceMode.VelocityChange);
    }
}