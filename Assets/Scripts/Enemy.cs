using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb2d;
    public float delay = 0.15f;
    public float health = 10;
    public float speed = 10.0f;
    public Transform player;
    public ParticleSystem particle;
    void FixedUpdate()
    {
        if (health <= 0)
        {
            Debug.Log("dead");
            Destroy(gameObject);
        }
        else
        {
            if (Vector3.SqrMagnitude(transform.position - player.position) > 10.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * speed);
            }
        }
    }
    public void StartAttackSFX()
    {
        particle.gameObject.SetActive(true);
        particle.Play();
        Debug.Log("start");
    }
    public void EndAttackSFX()
    {
        Debug.Log("stop");
        particle.Stop();
        particle.gameObject.SetActive(false);
    }
}
