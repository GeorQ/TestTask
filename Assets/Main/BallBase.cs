using UnityEngine;

public class BallBase : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    private byte _ownerID;


    //private void Start()
    //{
    //    gameObject.SetActive(false); 
    //}

    public byte GetOwnerID()
    {
        return _ownerID;
    }
    
    public virtual void Push(Vector3 direction, float pushForce, byte ownerID)
    {
        _ownerID = ownerID;
        _rb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
    }

    public virtual void ResetBall(Vector3 startPos)
    {
        _rb.isKinematic = false;
        _rb.velocity = Vector3.zero;
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
    }
}