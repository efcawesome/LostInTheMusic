using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatManager : MonoBehaviour
{
    public List<Vector3> hatPositions;

    public string currHat;

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Animator>().GetInteger("HatNum") != GetHatIndex(currHat))
        {
            transform.localPosition = hatPositions[GetHatIndex(currHat)];
            GetComponent<Animator>().SetInteger("HatNum", GetHatIndex(currHat));
        }


        if (GetHatIndex(currHat) > 0)
        {
            var pos = transform.localPosition;
            pos.x = GameObject.Find("Player").GetComponent<SpriteRenderer>().flipX ? -hatPositions[GetHatIndex(currHat)].x : hatPositions[GetHatIndex(currHat)].x;
            transform.localPosition = pos;
        }
        GetComponent<SpriteRenderer>().flipX = GameObject.Find("Player").GetComponent<SpriteRenderer>().flipX;
    }

    public static int GetHatIndex(string hatName)
    {
        switch(hatName)
        {
            case "propeller":
                return 1;
            case "party":
                return 2;
            case "crown":
                return 3;
            case "feather":
                return 4;
            case "cat":
                return 5;
            case "cloud":
                return 6;
            case "moon":
                return 7;
            default:
                return 0;
        }
    }

    public int GetCurrentHatIndex()
    {
        return GetHatIndex(currHat);
    }

    public void SetHat(int index)
    {
        switch(index)
        {
            case 1:
                currHat = "propeller";
                break;
            case 2:
                currHat = "party";
                break;
            case 3:
                currHat = "crown";
                break;
            case 4:
                currHat = "feather";
                break;
            case 5:
                currHat = "cat";
                break;
            case 6:
                currHat = "cloud";
                break;
            case 7:
                currHat = "moon";
                break;
            default:
                currHat = "";
                break;
        }
    }
}
