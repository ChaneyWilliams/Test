using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb2d;
    public float health = 10;
    public float speed = 10.0f;
    public Transform player;
    public bool hit = false;
    SpriteFlasher spriteFlasher;
    void Start()
    {
        spriteFlasher = GetComponent<SpriteFlasher>();
    }
    void FixedUpdate()
    {
        if (health <= 0)
        {
            Debug.Log("dead");
            Destroy(gameObject);
        }
        else
        {
            if (Vector3.SqrMagnitude(transform.position - player.position) > 20.0f && !spriteFlasher.hit)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * speed);
            }
        }
    }
}
