using System.Collections;
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
    public void Hit(Vector3 sender, float strength, Rigidbody2D recieverRB2D)
    {

        Debug.Log("HIT");
        StopAllCoroutines();

        Vector2 direction = -(transform.position - sender).normalized;
        recieverRB2D.AddForceX(direction.x * strength, ForceMode2D.Impulse);
        StartCoroutine(ResetVelocity(recieverRB2D, 0.5f));
    }
    IEnumerator ResetVelocity(Rigidbody2D rigid, float delay)
    {

        Debug.Log("Reset");
        yield return new WaitForSeconds(delay);

        if (rigid == null) yield break;

        rigid.linearVelocity = Vector3.zero;
    }
}
