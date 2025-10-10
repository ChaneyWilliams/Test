using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 10;
    void Update()
    {
        if (health <= 0)
        {
            Debug.Log("dead");
            Destroy(gameObject);
        }
    }
}
