using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHat : MonoBehaviour
{
    public string hatToUnlock;

    private void Start()
    {
        if(GameObject.Find("GameManager").GetComponent<GameManager>().hatUnlocked[HatManager.GetHatIndex(hatToUnlock)]) Destroy(gameObject);
    }

    public void OnPickup()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().hatUnlocked[HatManager.GetHatIndex(hatToUnlock)] = true;
        GameObject.Find("GameManager").GetComponent<GameManager>().UnlockHat();
        // Unlock screen
        Destroy(gameObject);
    }
}
