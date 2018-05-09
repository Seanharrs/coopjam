using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(EntitySounds))]
public class Health : MonoBehaviour
{
    [SerializeField]
    private int m_InitialAndMaxHealth = 100;
    public int healthRemaining { get; private set; }

    public float timeSinceDamageTaken { get; private set; }
    private bool m_WasAttackedRecently { get { return timeSinceDamageTaken < 0.5f; } }

    [SerializeField]
    private bool m_CanRespawn;
    public bool canRespawn { get { return m_CanRespawn; } }

    public bool isInvulnerable;

    private AudioSource m_Audio;

    //public GUIHealthBar healthBar;

    private void Awake()
    {
        healthRemaining = m_InitialAndMaxHealth;

        m_Audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        timeSinceDamageTaken += Time.deltaTime;
    }

    private void SetHealth(int newHp)
    {
        if(healthRemaining > newHp)
        {
            timeSinceDamageTaken = 0f;

            //if(!m_Audio.isPlaying)
            //{
            //    AudioClip[] hurtSounds = GetComponent<EntitySounds>().hurtSounds;
            //    if(hurtSounds.Length > 0)
            //    {
            //        m_Audio.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
            //        m_Audio.Play();
            //    }
            //}
        }

        healthRemaining = newHp;
        //if(healthBar)
        //    healthBar.UpdateGUI(HealthRemaining, InitialAndMaxHealth);

        if(healthRemaining <= 0)
        {
            gameObject.AddComponent<DeathAnimation>();
            enabled = false;
        }
    }

    /// <summary>Deals damage to the entity, reducing its health.</summary>
    /// <param name="amount">The amount of damage to deal to the entity.</param>
    public void TakeDamage(int amount)
    {
        if(m_WasAttackedRecently || isInvulnerable)
            return;
        
        SetHealth(Mathf.Clamp(healthRemaining - amount, 0, m_InitialAndMaxHealth));
    }

    /// <summary>Forcibly kills the entity, regardless of whether it is invulnerable or has been attacked recently.</summary>
    public void ForceKill()
    {
        SetHealth(0);
    }

    /// <summary>Heals the entity, increasing its health.</summary>
    /// <param name="amount">The amount of health to recover.</param>
    public void RecoverHealth(int amount)
    {
        SetHealth(Mathf.Clamp(healthRemaining + amount, 0, m_InitialAndMaxHealth));
    }

    /// <summary>Resets the entity's health to full.</summary>
    public void ResetHealth()
    {
        SetHealth(m_InitialAndMaxHealth);
    }
}
