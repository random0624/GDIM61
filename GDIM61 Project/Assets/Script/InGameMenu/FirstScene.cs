using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
    private void Update()
    {
        bool keyInput = Input.anyKeyDown;
        bool mouseInput = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if (keyInput || mouseInput)
        {
            SceneManager.LoadScene("Map");
        }
    }
}
