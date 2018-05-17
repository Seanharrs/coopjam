﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Projectile))]
    public class GravityProjectile : MonoBehaviour
    {
        [SerializeField]
        private Projectile.ProjectileType m_Type;

        [SerializeField]
        private float m_EffectLength;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Rigidbody2D rb2d = collision.GetComponent<Rigidbody2D>();
            if(rb2d == null)
            { Debug.Log("Didn't hit a rigidbody.");
                Destroy(gameObject);
                return;
            } else {
              Debug.Log("Hit an RB. applying gravity.");
            }

            if(m_Type == Projectile.ProjectileType.Primary)
                StartCoroutine(ReverseGravity(rb2d));
            else
                StartCoroutine(ReduceGravity(rb2d));

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }

        private IEnumerator ReverseGravity(Rigidbody2D ctrl)
        {
          var pc2D = ctrl.GetComponentInParent<PlatformerCharacter2D>();
          if(pc2D) pc2D.NormalGravity *= -1;
            ctrl.gravityScale *= -1;
            yield return new WaitForSeconds(m_EffectLength);
          if(pc2D) pc2D.NormalGravity *= -1;
            ctrl.gravityScale *= -1;
            Destroy(gameObject);
        }

        private IEnumerator ReduceGravity(Rigidbody2D ctrl)
        {
            ctrl.gravityScale /= 2;
            yield return new WaitForSeconds(m_EffectLength);
            ctrl.gravityScale *= 2;
            Destroy(gameObject);
        }
    }
}
