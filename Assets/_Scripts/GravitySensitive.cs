using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class GravitySensitive : MonoBehaviour
    {

        [SerializeField, Tooltip("Event triggered when primary weapon projectile hits this object.")]
        private ProjectileEvent OnReverseGravityHit = new ProjectileEvent();
        [SerializeField, Tooltip("Event triggered when secondary weapon projectile hits this object.")]
        private ProjectileEvent OnReduceGravityHit = new ProjectileEvent();

        [SerializeField, Tooltip("Gravity reversal works on this object. Defaults to true.")]
        internal bool m_canReverseGravity = true;
        [SerializeField, Tooltip("Gravity reduction works on this object. Defaults to true.")]
        internal bool m_canReduceGravity = true;

        private Rigidbody2D m_rb2D;

        [SerializeField]
        private float m_ReverseLength = 3f;

        [SerializeField]
        private float m_ReduceLength = 5f;

        private void Awake() { m_rb2D = GetComponent<Rigidbody2D>(); }

        internal bool ChangeGravity(Gun sourceGun, WhichWeapon type)
        {

            Debug.Log(name + " changing with " + type);
            if(type == WhichWeapon.Primary && m_canReverseGravity)
            {
                StartCoroutine(Reverse());
                OnReverseGravityHit.Invoke(sourceGun, type);
                return true;
            }
            else if(type == WhichWeapon.Secondary && m_canReduceGravity)
            {
                StartCoroutine(Reduce());
                OnReduceGravityHit.Invoke(sourceGun, type);
                return true;
            }
            else
              return false;
        }

        private IEnumerator Reverse()
        {
            PlatformerCharacter2D pc2D = GetComponent<PlatformerCharacter2D>();
            if(pc2D)
                pc2D.NormalGravity *= -1;
            else
                m_rb2D.gravityScale *= -1;

            Vector3 rot = transform.rotation.eulerAngles;
            Vector3 average;
            
            // Rotate to accommodate reversed gravity
            average = GetAverageCenter(transform);
            transform.RotateAround(average, Vector3.forward, 180);

            yield return new WaitForSeconds(m_ReverseLength);

            if(pc2D)
                pc2D.NormalGravity *= -1;
            else
                m_rb2D.gravityScale *= -1;
              
            // Rotate to accommodate reversed gravity
            average = GetAverageCenter(transform);
            transform.RotateAround(average, Vector3.forward, 180);
        }

        private Vector3 GetAverageCenter(Transform t)
        {
            List<Vector3> allCenters = t.GetComponents<Collider2D>().Select(c => c.bounds.center).ToList();
            allCenters.AddRange(t.GetComponents<SpriteRenderer>().Select(s => s.bounds.center));
            return allCenters.Aggregate((c, next) => { return next + c; } ) / allCenters.Count();
        }

        private IEnumerator Reduce()
        {
            PlatformerCharacter2D pc2D = GetComponent<PlatformerCharacter2D>();
            if(pc2D)
                pc2D.NormalGravity /= 2;
            else
                m_rb2D.gravityScale /= 2;

            yield return new WaitForSeconds(m_ReduceLength);

            if(pc2D)
                pc2D.NormalGravity *= 2;
            else
                m_rb2D.gravityScale *= 2;
        }
    }
}
