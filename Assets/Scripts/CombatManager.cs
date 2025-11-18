using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;
public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    void Awake()
    {
        if (instance != this && instance == null)
        {
            instance = this;
        }
    }
    public void Hit(Vector3 sender, float strength, int damage, GameObject reciever)
    {
        //check if the player is being hit
        if (reciever.gameObject.CompareTag("Player"))
        {
            if (Player.instance.isParrying)
            {
                SoundEfffectManager.Play("Parry");
                return;
            }
            //Player.instance.health -= damage;
            Debug.Log(Player.instance.health);
        }
        //check if enemy is being hit
        else if (reciever.CompareTag("Enemy"))
        {
            //reciever.GetComponent<Enemy>().health -= damage;
            Debug.Log(reciever.GetComponent<Enemy>().health);
        }

        Debug.Log($"HIT the {reciever.gameObject.tag}");

        SoundEfffectManager.Play("OOF");
        Rigidbody2D rb = reciever.GetComponent<Rigidbody2D>();
        Vector2 direction = (reciever.transform.position - sender).normalized;

        rb.AddForceX(direction.x * strength, ForceMode2D.Impulse);
        rb.AddForceY(strength, ForceMode2D.Impulse);

        
        StartCoroutine(ResetVelocity(rb, 0.5f));
    }

    IEnumerator ResetVelocity(Rigidbody2D rigid, float delay)
    {
        //begin sprite spritFlashing / I-Frames
        SpriteFlasher spriteFlasher = rigid.GetComponent<SpriteFlasher>();
        //insuring that the velocity won't get overwritten in an movement update
        spriteFlasher.hit = true;

        StartCoroutine(rigid.GetComponent<SpriteFlasher>().Flash(1, Color.black, 3));

        yield return new WaitForSeconds(delay);
        //can now move
        spriteFlasher.hit = false;
        if (rigid == null) yield break;

        rigid.linearVelocity = Vector3.zero;
    }
}
