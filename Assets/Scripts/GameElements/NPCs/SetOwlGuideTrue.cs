using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOwlGuideTrue : MonoBehaviour
{
    public void OnOpen()
    {
        GameManager.talkedToOwl = true;
    }
}
