using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTestScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOwned)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Hello world");
        }
    }

    public override void OnStartAuthority()
    {
        Debug.Log("?");
        base.OnStartAuthority();
    }
}
