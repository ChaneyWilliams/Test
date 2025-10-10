using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 5;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().health -= damage;
        }
    }
}
