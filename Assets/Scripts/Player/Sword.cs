using UnityEditor.Callbacks;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 5;
    public float strength = 5;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision != null)
        {
            collision.GetComponent<Enemy>().health -= damage;
            CombatManager.instance.Hit(transform.position, strength, collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }
}
