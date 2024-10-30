using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HazardType
{
    Spikes,
    MovingPlatform,
    BreakableSurface,
    ExplodingBarrel,
    ElectricityField
}

public class EnviromentHazard : MonoBehaviour
{
    public HazardType type;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (type)
        {
            case HazardType.Spikes:
                HandleSpikesCollision(other);
                break;

            case HazardType.MovingPlatform:
                HandleMovingPlatformCollision(other);
                break;

            case HazardType.BreakableSurface:
                HandleBreakableSurfaceCollision(other);
                break;

            case HazardType.ExplodingBarrel:
                HandleExplodingBarrelCollision(other);
                break;

            case HazardType.ElectricityField:
                HandleElectricityFieldCollision(other);
                break;

            default:
                Debug.LogWarning("Unhandled hazard type: " + type);
                break;
        }
    }

    private void HandleSpikesCollision(Collision2D other)
    {
        // Code to handle spike behavior (e.g., damage to player or enemy)
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            Vector2 AwayFromSpikesVector = other.gameObject.transform.position - this.transform.position;
            AwayFromSpikesVector = AwayFromSpikesVector * other.relativeVelocity;
            player.TakeDamage(10, AwayFromSpikesVector);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemyScript = other.gameObject.GetComponent<Enemy>();
            enemyScript.Damage(1000, 1f);
        }
    }

    private void HandleMovingPlatformCollision(Collision2D other)
    {
        // Code to handle moving platform behavior (e.g., movement pattern)
    }

    private void HandleBreakableSurfaceCollision(Collision2D other)
    {
        // Code to handle breakable surface behavior (e.g., trigger break on impact)
    }

    private void HandleExplodingBarrelCollision(Collision2D other)
    {
        // Code to handle exploding barrel behavior (e.g., explosion effect, damage radius)
    }

    private void HandleElectricityFieldCollision(Collision2D other)
    {
        // Code to handle electricity field behavior (e.g., apply knockback or stun)
    }
}
