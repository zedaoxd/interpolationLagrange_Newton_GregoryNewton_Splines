using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [SerializeField] private IScreen[] _screens;
    private IScreen previousScreen;
    
    private void Awake()
    {
        DisableAllScreens();
        ShowScreen<ScreenInitial>();
    }

    public void ShowScreenLagrange()
    {
        ShowScreen<ScreenLagrange>();
    }

    public void ShowScreenInitial()
    {
        ShowScreen<ScreenInitial>();
    }

    public void ShowScreenNewton()
    {
        ShowScreen<ScreenNewton>();
    }

    public void ShowScreenGregoryNewton()
    {
        ShowScreen<ScreenGregoryNewton>();
    }
    
    private  void ShowScreen<T>() where T : IScreen
    {
        foreach (var screen in _screens)
        {
            if (screen is T)
            {
                if (previousScreen != null)
                {
                    previousScreen.gameObject.SetActive(false);
                }
                screen.gameObject.SetActive(true);
                previousScreen = screen;
            }
        }
    }

    private void DisableAllScreens()
    {
        foreach (var screen in _screens)
        {
            screen.gameObject.SetActive(false);
        }
    } 
    
}
