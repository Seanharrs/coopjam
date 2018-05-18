using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeathCollider), typeof(AudioSource))]
public class DeadlyLaser : MonoBehaviour
{
    public void TurnOn()
    {
        GetComponent<DeathCollider>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void TurnOff()
    {
        GetComponent<DeathCollider>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
