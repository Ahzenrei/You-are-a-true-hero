using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndCinematic : MonoBehaviour
{

    Text endText = null;

    private void Start()
    {
        endText = GetComponentInChildren<Text>();
        endText.text += Mathf.RoundToInt(Score.GetScore()).ToString();
    }

    public void OnMenuPressed()
    {
        SceneManager.LoadScene("Menu");
    }
}
