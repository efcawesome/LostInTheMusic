using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TromboneGame : MonoBehaviour
{
    public TMP_Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = SlideController.score.ToString();
    }
}
