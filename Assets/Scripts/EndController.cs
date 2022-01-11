using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndController : MonoBehaviour
{
    public GameObject endScreen;

    void Awake()
    {
        if(endScreen == null)
        {
            endScreen = GameObject.Find("EndScreen");
        }
        endScreen.SetActive(false);
    }

    public void OnGameEnd(char winner)
    {
        endScreen.SetActive(true);
        endScreen.transform.Find("Winner").GetComponent<TextMeshProUGUI>().text = winner == 'R' ? "Red Team" : "Blue Team";
    }
}
