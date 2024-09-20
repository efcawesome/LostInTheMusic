using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableFloor : MonoBehaviour
{
    public PolygonCollider2D postBreakSceneBounds;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Equals("Player"))
        {
            BreakFloor();
        }
    }

    private void BreakFloor()
    {
        // Play noise + animation
        GameObject.Find("Virtual Camera").GetComponent<Cinemachine.CinemachineConfiner2D>().m_BoundingShape2D = postBreakSceneBounds;
        Destroy(gameObject);
    }
}
