using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScoreTimer : MonoBehaviour
{
    public Text ScoreText;
    public static int score;

    // Start is called before the first frame update
    void Start()
    {
       score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = score.ToString() + "POINTS";
    }
}
