using System.Collections;
using UnityEngine;

namespace Coop
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class GravitySensitive : MonoBehaviour
    {
        private Rigidbody2D m_rb2D;

        [SerializeField]
        private float m_ReverseLength = 3f;

        [SerializeField]
        private float m_ReduceLength = 5f;

        private void Awake() { m_rb2D = GetComponent<Rigidbody2D>(); }

        public void ChangeGravity(WhichWeapon type)
        {

            Debug.Log(name + " changing with " + type);
            if(type == WhichWeapon.Primary)
                StartCoroutine(Reverse());
            else
                StartCoroutine(Reduce());
        }

        private IEnumerator Reverse()
        {
            PlatformerCharacter2D pc2D = GetComponent<PlatformerCharacter2D>();
            if(pc2D)
                pc2D.NormalGravity *= -1;
            else
                m_rb2D.gravityScale *= -1;

            Vector3 rot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, 180, 180);

            yield return new WaitForSeconds(m_ReverseLength);

            if(pc2D)
                pc2D.NormalGravity *= -1;
            else
                m_rb2D.gravityScale *= -1;

            transform.rotation = Quaternion.Euler(rot);
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
