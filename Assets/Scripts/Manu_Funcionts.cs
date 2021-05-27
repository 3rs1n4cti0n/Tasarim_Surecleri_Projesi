using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manu_Funcionts : MonoBehaviour
{
    int play_game_scene;

    public void Play_Game()
    {
        // TODO: CREATE NEW SAVE FILE
        // curently just switching to a scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Continue_Game()
    {
        // TODO: LOAD EXISTING SAVE FILE
    }

    public void Exit_Game()
    {
        Application.Quit();
    }
}
