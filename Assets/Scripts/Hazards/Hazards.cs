using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    protected GameManager GM;

    void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 KnockbackForce(Transform collider, float multiplier)
    {
        // Determine the direction for the force
        int dir = (collider.transform.position.x - transform.position.x) > 0 ? 1 : -1;
        float distance = Vector2.Distance(transform.position, collider.transform.position);
        float distanceFactor = Mathf.Clamp(1 / (distance + 0.5f), 0.1f, 10.5f); // Limit force scaling for very close/very far

        Vector2 force = new Vector2(dir * distanceFactor * multiplier, multiplier / 6); // Increase multiplier if you want a larger effect

        return force;
    }

    //public IEnumerator TemporaryPrefab(GameObject prefab, Vector2 position, float time)
    //{
    //    GameObject temp = Instantiate(prefab, position, Quaternion.identity);
    //    Debug.Log("bruh" + time);
    //    yield return new WaitForSeconds(time);
    //    Debug.Log("zamn");
    //    Destroy(temp);
    //}

    public IEnumerator TemporaryPrefab(GameObject prefab, Vector2 position, float time)
    {
        GameObject temp = Instantiate(prefab, position, Quaternion.identity);
        SpriteRenderer spriteRenderer = temp.GetComponent<SpriteRenderer>();



        Debug.Log("numbah 1");

        if (spriteRenderer != null)
        {
            float fadeDuration = 1.0f; // Adjust the fade duration as needed
            float startAlpha = spriteRenderer.color.a;
            float elapsed = 0f;

            // Gradually reduce alpha over `fadeDuration` seconds
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, 0, elapsed / fadeDuration);
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
                yield return null;
            }
        }
        Debug.Log("numbah 2");

        // Wait for the remaining time (if fade duration is less than `time`)
        yield return new WaitForSeconds(Mathf.Max(0, time));

        Debug.Log("numbah 3");

        // Destroy the prefab after the fade-out effect
        Destroy(temp);

        Debug.Log("numbah 4 is crazy");
    }
}
