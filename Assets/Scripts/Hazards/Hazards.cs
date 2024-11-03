using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    private GameManager GM;

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

    public Vector2 PlayerHitKnockBackVectorNormalized()
    {
        return (GM.Player.transform.position - this.transform.position).normalized;
    }

    public IEnumerator TemporaryPrefab(GameObject prefab, Vector2 position, float time)
    {
        GameObject temp = Instantiate(prefab, position, Quaternion.identity);
        Debug.Log("bruh" + time);
        yield return new WaitForSeconds(time);
        Debug.Log("zamn");
        Destroy(temp);
    }
}
