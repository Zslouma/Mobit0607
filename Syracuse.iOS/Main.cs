﻿using UIKit;

namespace Syracuse.Mobitheque.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here

            Rg.Plugins.Popup.Popup.Init();
            UIApplication.Main(args, null, "AppDelegate");
        }

    }   
}