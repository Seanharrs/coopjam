//using UnityEngine;

//[RequireComponent(typeof(Animator), typeof(EntitySounds))]
//public class DeathAnimation : MonoBehaviour
//{
//    private Animator m_Anim;

//    private int m_DieHash;
//    private bool m_CanRespawn;
//    private bool m_Respawning;

//    private void Awake()
//    {
//		Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
//        if(rb2d)
//            rb2d.velocity = Vector3.zero;

//        BoxCollider2D col = GetComponent<BoxCollider2D>();
//        if(col)
//            col.enabled = false;

//        Coop.Platformer2DUserControl ctrl = GetComponent<Coop.Platformer2DUserControl>();
//        if(ctrl)
//            ctrl.enabled = false;

//        UnityStandardAssets._2D.PlatformerCharacter2D character = GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();
//        if(character)
//            character.enabled = false;

//        m_CanRespawn = GetComponent<Health>().canRespawn;

//        m_Anim = GetComponent<Animator>();
//        m_Anim.SetTrigger("Die");

//        m_DieHash = Animator.StringToHash("Base Layer.Die");

//        EntitySounds sounds = GetComponent<EntitySounds>();
//        if(sounds.deathSounds.Length == 0)
//            return;

//        AudioClip deathSound = sounds.deathSounds[Random.Range(0, sounds.deathSounds.Length)];
//        AudioSource audio = GetComponent<AudioSource>();
//        audio.clip = deathSound;
//        audio.Play();
//    }

//    private void Update()
//    {
//        AnimatorStateInfo state = m_Anim.GetCurrentAnimatorStateInfo(0);
//        if(state.fullPathHash == m_DieHash && state.normalizedTime > 1.5f)
//        {
//            gameObject.SetActive(false);

//            if(m_CanRespawn && !m_Respawning) //is a player character
//            {
//                Invoke("Respawn", 2f);
//                m_Respawning = true;
//            }
//        }
//    }

//    private void Respawn()
//    {
//        transform.position = Vector2.one;

//        BoxCollider2D col = GetComponent<BoxCollider2D>();
//        if(col)
//            col.enabled = true;

//        Coop.Platformer2DUserControl ctrl = GetComponent<Coop.Platformer2DUserControl>();
//        if(ctrl)
//            ctrl.enabled = true;

//        UnityStandardAssets._2D.PlatformerCharacter2D character = GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();
//        if(character)
//            character.enabled = true;

//        Health hp = GetComponent<Health>();
//        if(hp)
//            hp.enabled = true;

//        gameObject.SetActive(true);

//        m_Anim.SetTrigger("Respawn");
//        Destroy(this);
//    }
//}
