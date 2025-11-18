using UnityEditor.Callbacks;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 5;
    public float strength = 5;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        CombatManager.instance.Hit(transform.position, strength, rb);
    }

}
