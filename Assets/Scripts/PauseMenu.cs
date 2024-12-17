using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public bool activePauseMenu = true;
    public XRRayInteractor mandoDerechoRaycastLength;
    public XRRayInteractor mandoIzquierdoRaycastLength;
    public XRInteractorLineVisual mandoDerechoRaycastColor;
    public XRInteractorLineVisual mandoIzquierdoRaycastColor;

    // Start is called before the first frame update
    void Start()
    {
        DisplayPauseMenu();
    }

    public void PauseButtonPressed(InputAction.CallbackContext callbackContext)
    {
        
        if (callbackContext.performed)
        {
            DisplayPauseMenu();
        }
    }

    public void DisplayPauseMenu()
    {
        if (activePauseMenu)
        {
            pauseMenu.SetActive(false);
            activePauseMenu = false;
            Time.timeScale = 1;
            mandoDerechoRaycastLength.maxRaycastDistance = 0;
            mandoIzquierdoRaycastLength.maxRaycastDistance = 0;
            mandoDerechoRaycastColor.setLineColorGradient = false;
            mandoIzquierdoRaycastColor.setLineColorGradient = false;
        } else if (!activePauseMenu)
        {
            pauseMenu.SetActive(true);
            activePauseMenu = true;
            Time.timeScale = 0;
            mandoDerechoRaycastLength.maxRaycastDistance = 120;
            mandoIzquierdoRaycastLength.maxRaycastDistance = 120;
            mandoDerechoRaycastColor.setLineColorGradient = true;
            mandoIzquierdoRaycastColor.setLineColorGradient = true;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ResumeGame()
    {
        DisplayPauseMenu();
    }

}
