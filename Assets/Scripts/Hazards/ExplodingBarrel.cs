using System.Collections;
using UnityEngine;

public class ExplodingBarrel : Hazards, IDamageable
{
    public int maxKicks = 3; // Number of kicks required to explode
    public GameObject explosionPrefab; // Prefab for explosion effect
    private int currentKicks = 0; // Counter for received kicks
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Vector2 punchVector = new Vector2();
    public Rigidbody2D RB { get; set; }
    //private GameObject Explosion;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Store the original color of the barrel
        RB = GetComponent<Rigidbody2D>();
        //Explosion = this.transform.Find("Explosion")?.gameObject;
    }

    public void TakeKick(int damage, Vector2 force)
    {
        BarrelHit(force);
    }

    public void TakePunch(int damage, float airStunTime)
    {
        BarrelHit(KnockbackForce(GM.Player.transform, 1f));
    }

    public void BarrelHit(Vector2 force)
    {
        RB.velocity = Vector2.zero; // so previous velocity doesn't interfere
        RB.AddForce(force, ForceMode2D.Impulse);
        Debug.Log("RAHHHH ");
        currentKicks++;

        // Flash red briefly to show the barrel has been hit
        StartCoroutine(FlashRed());

        // Check if the barrel has received enough kicks to explode
        if (currentKicks >= maxKicks)
            StartCoroutine(ExplodeSequence());


        // SpawnDamageVFX(force.x);
    }

    private IEnumerator FlashRed()
    {
        // Change color to red
        spriteRenderer.color = Color.red;

        // Wait for a brief moment
        yield return new WaitForSeconds(0.1f);

        // Change back to the original color
        spriteRenderer.color = originalColor;
    }

    private IEnumerator ExplodeSequence()
    {
        float flashInterval = 0.5f;  // Initial time between flashes
        float minimumInterval = 0.05f;  // Minimum interval for rapid flashing
        float flashReductionRate = 0.85f; // Factor to reduce the interval each time
        int flashCount = 10;  // Number of flashes before exploding

        for (int i = 0; i < flashCount; i++)
        {
            // Flash red briefly
            StartCoroutine(FlashRed());

            // Wait for the current interval before the next flash
            yield return new WaitForSeconds(flashInterval);

            // Reduce the interval to create a faster flashing effect
            flashInterval *= flashReductionRate;
            if (flashInterval < minimumInterval)
            {
                flashInterval = minimumInterval;  // Clamp to minimum interval
            }
        }

        // Play explosion animation
        anim.Play("BarrelExplosion");

        // Wait for the animation to complete
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - 0.55f);
        //Explosion.SetActive(true);
        yield return StartCoroutine(TemporaryPrefab(explosionPrefab, transform.position, 0.1f));
        
        anim.Play("BarrelGone");

        // Destroy the barrel game object
        Destroy(gameObject);
    }


    #region Not Implemented
    [HideInInspector] public int MaxHealth { get; set; }
    [HideInInspector] public int CurrentHealth { get; set; }
    [HideInInspector] public Animator Anim { get; set; }
    [HideInInspector] public object DeathLock { get; set; }
    [HideInInspector] public bool IsDead { get; set; }
    [HideInInspector] public bool InHitStun { get; set; }
    [HideInInspector] public float HitStunTime { get; set; }
    [HideInInspector] public bool inDownSlam { get; set; }
    [HideInInspector] public bool inAirStun { get; set; }

    public void Damage(int damage, float hitStunTime)
    {
        return;
    }

    public void Die()
    {
        return;
    }

    public void StopAttack()
    {
        return;
    }

    public void TakeUppercut(int damage, Vector2 force)
    {
        return;
    }
    #endregion 

}



