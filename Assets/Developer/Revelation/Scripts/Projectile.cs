using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D), typeof (Collider2D))]
public class Projectile : MonoBehaviour {

  [Header("Projectile")]
  public float projectileSpeed = 10;

  public virtual void Initiate(Vector2 direction) {
    GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
  }

}
