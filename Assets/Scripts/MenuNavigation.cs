using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public void LoadSinglePlayer()
    {
        SceneManager.LoadScene("Scene_Loading");
    }

    public void LoadMultiPlayerMenu()
    {
        SceneManager.LoadScene("Scene_Multiplayer");
    }

    public void LoadMultiPlayer()
    {
        SceneManager.LoadScene("Scene_Loading");
    }

    public void LoadStudentInfo()
    {
        SceneManager.LoadScene("Scene_StudentInfo");
    }

    public void LoadOptions()
    {
        SceneManager.LoadScene("Scene_Options");
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("Scene_Menu");
    }
}
