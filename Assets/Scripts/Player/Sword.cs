using UnityEditor.Callbacks;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 5;
    public float strength = 5;
    void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Enemy"))
    {
        Debug.Log("ReKt");
        var rb = collision.attachedRigidbody;
        collision.GetComponent<Enemy>().health -= damage;
        CombatManager.instance.Hit(transform.position, strength, rb);
    }

    if (collision.CompareTag("Player"))
    {
        var rb = collision.attachedRigidbody;
        CombatManager.instance.Hit(transform.position, strength, rb);
    }
}

}
