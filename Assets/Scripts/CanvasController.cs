using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasController : MonoBehaviour
{
    public GameObject endScreen;
    public GameObject pauseScreen;

    void Awake()
    {
        if(endScreen == null)
            endScreen = GameObject.Find("EndScreen");
        if(pauseScreen == null)
            pauseScreen = GameObject.Find("PauseScreen");

        endScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    public void OnGameEnd(char winner)
    {
        endScreen.SetActive(true);
        endScreen.transform.Find("Winner").GetComponent<TextMeshProUGUI>().text = winner == 'R' ? "Red Team" : "Blue Team";
    }

    public bool OnPause()
    {
        pauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        return true;
    }

    public bool OnUnPause()
    {
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        return false;
    }
}
