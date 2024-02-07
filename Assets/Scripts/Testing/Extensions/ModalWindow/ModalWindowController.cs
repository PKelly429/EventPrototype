using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModalWindowController
{
    public static bool modalWindowEnabled;
    
    private static ModalWindow modalWindow;

    public static void RegisterModalWindow(ModalWindow window)
    {
        modalWindow = window;
    }

    public static void ShowModalWindow(ModalWindowSettings settings)
    {
        if (modalWindow == null)
        {
            return;
        }
        
        modalWindow.Show(settings);
    }
}
