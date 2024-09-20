using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToSand : MonoBehaviour
{
    public void OpenDialogue()
    {
        GameManager.talkedToSand = true;
    }
}
