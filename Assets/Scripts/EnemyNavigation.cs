using UnityEngine;

public class EnemyNavigation : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 0.5f; // Radar units per second
    public float impactDistance = 0.2f; // How close to center is a "hit"?

    void Update()
    {
        // Move towards the center (0,0,0)
        transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, speed * Time.deltaTime);

        // Check for Impact (Game Over / Damage condition)
        if (Vector3.Distance(transform.position, Vector3.zero) < impactDistance)
        {
            Impact();
        }
    }

    void Impact()
    {
        Debug.Log("<color=red><b>IMPACT!</b> We took a hit!</color>");
        // TODO: Subtract health or trigger Game Over
        Destroy(gameObject);
    }
}