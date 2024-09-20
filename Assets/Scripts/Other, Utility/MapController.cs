using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public RectTransform map;
    public float moveDelta;

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && map.localPosition.x > -430f)
        {
            var pos = map.position;
            pos.x -= moveDelta * Time.deltaTime * 60;
            map.position = pos;
        }
        else if(Input.GetAxisRaw("Horizontal") < 0 && map.localPosition.x < 430f)
        {
            var pos = map.position;
            pos.x += moveDelta * Time.deltaTime * 60;
            map.position = pos;
        }

        if (Input.GetAxisRaw("Vertical") > 0 && map.localPosition.y > -300f)
        {
            var pos = map.position;
            pos.y -= moveDelta * Time.deltaTime * 60;
            map.position = pos;
        }
        else if (Input.GetAxisRaw("Vertical") < 0 && map.localPosition.y < 130f)
        {
            var pos = map.position;
            pos.y += moveDelta * Time.deltaTime * 60;
            map.position = pos;
        }
    }
}
