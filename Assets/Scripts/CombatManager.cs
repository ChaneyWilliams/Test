using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;
public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;
    public int parryCount = 0;
    int parryIndex = 0;
    public int hitsToUpgradeParry = 3;
    public float flashDuration;
    public int flashNum;

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
                parryIndex = (parryCount % hitsToUpgradeParry == 0) ? parryCount / hitsToUpgradeParry: parryIndex;
                Debug.Log(parryIndex);
                SoundEfffectManager.Play("Parry", parryIndex);
                Debug.Log(parryCount);
                // 4 is the number of Parry clips. (too lazy to find some way to measure the list and reference it so magic number)
                // -1 because the index starts at 0
                if (parryIndex < 3)
                {
                    parryCount++;
                }
                return;
            }
            flashDuration = 1.0f;
            flashNum = 3;
            parryCount = 0;
            //Player.instance.health -= damage;
            Debug.Log(Player.instance.health);
        }
        //check if enemy is being hit
        else if (reciever.CompareTag("Enemy"))
        {
            //reciever.GetComponent<Enemy>().health -= damage;
            Debug.Log(reciever.GetComponent<Enemy>().health);
            flashDuration = 0.2f;
            flashNum = 1;
        }

        Debug.Log($"HIT the {reciever.gameObject.tag}");

        SoundEfffectManager.Play("OOF", 0);
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

        StartCoroutine(rigid.GetComponent<SpriteFlasher>().Flash(flashDuration, Color.black, flashNum));

        yield return new WaitForSeconds(delay);
        //can now move
        spriteFlasher.hit = false;
        if (rigid == null) yield break;

        rigid.linearVelocity = Vector3.zero;
    }
}
