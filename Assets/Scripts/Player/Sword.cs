using UnityEditor.Callbacks;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 5;
    public float strength = 5;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")) && !collision.gameObject.GetComponent<SpriteFlasher>().isFlashing)
        {
            CombatManager.instance.Hit(transform.position, strength, damage, collision.gameObject);
        }

    }

}
