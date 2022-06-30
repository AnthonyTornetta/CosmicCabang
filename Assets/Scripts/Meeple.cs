using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Meeple : MonoBehaviour
{
    [SerializeField]
    private GameObject gunHand;

    [SerializeField]
    private GameObject gunArm;
    
    private bool floating = true;

    private Rigidbody2D body;
    
    public GameObject GunHand
    {
        get
        {
            return gunHand;
        }
    }

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();

        Floating = floating;
        
        if (heldItem != null)
        {
            heldItem.transform.parent = GunHand.transform;
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
                
            heldItem.GetComponent<AttractedToPlanets>().enabled = false;
            heldItem.GetComponent<Rigidbody2D>().simulated = false;
            heldItem.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.isTrigger)
        {
            Floating = false;

            List<ContactPoint2D> contacts = new List<ContactPoint2D>();
            col.GetContacts(contacts);
            foreach(var contact in contacts)
            {
                transform.rotation = Quaternion.FromToRotation (Vector3.up, ((Vector2)transform.position - contact.point).normalized);

                // transform.LookAt(transform.position + (Vector3)contact.normal);
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        if (!col.collider.isTrigger)
        {
            Floating = true;
        }
    }

    public bool Floating
    {
        get
        {
            return floating;
        }
        set
        {
            if (floating)
            {
                body.constraints = RigidbodyConstraints2D.None;
            }
            else
            {
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            floating = value;
        }
    }

    [SerializeField]
    private GameObject heldItem;
    
    // Null if holding no item
    public GameObject HeldItem
    {
        get
        {
            return heldItem;
        }
        set
        {
            if (value == heldItem)
                return;
            
            if (value != null)
            {
                value.transform.parent = GunHand.transform;
                value.transform.localPosition = Vector3.zero;
                value.transform.localRotation = Quaternion.identity;
                
                value.GetComponent<AttractedToPlanets>().enabled = false;
                value.GetComponent<Rigidbody2D>().simulated = false;
                value.GetComponent<Collider2D>().enabled = false;
            }

            if (heldItem != null)
            {
                heldItem.transform.parent = null;
                
                heldItem.GetComponent<Collider2D>().enabled = true;
                heldItem.GetComponent<Rigidbody2D>().simulated = true;
                heldItem.GetComponent<AttractedToPlanets>().enabled = true;
            }
            
            heldItem = value;
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gunArm.transform.position;
        var angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        gunArm.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public bool HasItem()
    {
        return HeldItem != null;
    }
}
