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
    public GameObject strongPowPrefab;
    public Vector2 punchVector = new Vector2();
    public Rigidbody2D RB { get; set; }

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Store the original color of the barrel
        RB = GetComponent<Rigidbody2D>();
    }

    public void TakeKick(int damage, Vector2 force)
    {
        BarrelHit(force);
    }

    public void TakePunch(int damage, float airStunTime)
    {
        BarrelHit(PlayerHitKnockBackVectorNormalized());
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
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.Play("BarrelGone");

        // Instantiate explosion effect
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Destroy the barrel game object
        Destroy(gameObject);
    }


    private void SpawnDamageVFX(float xForce)
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * 1.5f;
        Vector3 spawnPosition = new Vector3(randomPoint.x, randomPoint.y, 0) + transform.position;
        GameObject newVFX = Instantiate(strongPowPrefab, spawnPosition, Quaternion.identity);
        // make vfx 'weaker' if kick not strong enough
        // newVFX.GetComponent<SpriteRenderer>().color = newVFX.GetComponent<SpriteRenderer>().color * 0.5f;

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



