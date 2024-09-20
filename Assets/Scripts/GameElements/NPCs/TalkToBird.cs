using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToBird : MonoBehaviour
{
    public void OnOpen()
    {
        GameManager.talkedToHat = true;
    }
}
