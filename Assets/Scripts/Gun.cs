using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public GameObject BulletPrefab;
    
    public override void Fire(GameObject by)
    {
        var go = GameObject.Instantiate(BulletPrefab);
        go.transform.position = transform.position + transform.right * 0.35f;
        go.transform.rotation = transform.rotation;
        
        go.GetComponent<Rigidbody2D>().AddForce(transform.right * 20, ForceMode2D.Impulse);
    }
}
