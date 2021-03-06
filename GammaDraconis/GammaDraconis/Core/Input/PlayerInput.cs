using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GammaDraconis.Core.Input
{
    /// <summary>
    /// The input manager handles translating the Xna keyboard, mouse,
    /// and gamepad data into game-usable state.
    /// </summary>
    class PlayerInput : Input
    {
        public class Commands
        {
            public static String Up = "Up";
            public static String Down = "Down";
            public static String Left = "Left";
            public static String Right = "Right";
            public static String RollLeft = "RollLeft";
            public static String RollRight = "RollRight";
            public static String ThrottleUp = "ThrottleUp";
            public static String ThrottleDown = "ThrottleDown";
            public static String Fire1 = "Fire1";
            public static String Fire2 = "Fire2";
            public static String Pause = "Pause";
            public static String Menu = "Menu";
            public static String Reset = "Reset";

            public static String Yaw = "Yaw";
            public static String Pitch = "Pitch";
            public static String Roll = "Roll";
            public static String Turn = "Turn"; 
            public static String Throttle = "Throttle";

            public static String CameraX = "CameraX";
            public static String CameraY = "CameraY";

            public static String Join = "Join";
            public static String Leave = "Leave";
            public static String MenuUp = "MenuUp";
            public static String MenuDown = "MenuDown";
            public static String MenuLeft = "MenuLeft";
            public static String MenuRight = "MenuRight";
            public static String MenuSelect = "MenuSelect";

            public static String GameStart = "GameStart";
        }

        private InputManager.ControlScheme controlScheme;
        public InputManager.ControlScheme ControlScheme { get { return controlScheme; } }
        /// <summary>
        /// Set each player's key bindings depending on what control scheme they are using.
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="controlScheme"></param>
        public PlayerInput(PlayerIndex playerIndex, InputManager.ControlScheme controlScheme)
            : base(playerIndex)
        {
            this.controlScheme = controlScheme;

            if ((playerIndex == PlayerIndex.One) && !GamePad.GetCapabilities(PlayerIndex.One).IsConnected)
            {
                if (Properties.Settings.Default.PlayerOneUseMouse)
                {
                    inputAxis.Add(Commands.Pitch, "MouseY");
                    inputAxis.Add(Commands.Yaw, "MouseX");
                }
            }
            
            if (controlScheme == InputManager.ControlScheme.GamePad)
            {
                inputAxis.Add(Commands.Yaw, "LeftX");
                inputAxis.Add(Commands.Pitch, "LeftY");
                inputAxis.Add(Commands.Throttle, "Triggers");
                inputAxis.Add(Commands.CameraX, "RightX");
                inputAxis.Add(Commands.CameraY, "RightY");

                inputKeys.Add(Commands.RollLeft, "PadLB");
                inputKeys.Add(Commands.RollRight, "PadRB");

                inputKeys.Add(Commands.Fire1, "PadA");
                inputKeys.Add(Commands.Fire2, "PadB");
                inputKeys.Add(Commands.Pause, "PadStart");
                inputKeys.Add(Commands.Menu, "PadBack");
                inputKeys.Add(Commands.Reset, "PadY");

                inputKeys.Add(Commands.MenuUp, "PadUp");
                inputKeys.Add(Commands.MenuDown, "PadDown");
                inputKeys.Add(Commands.MenuLeft, "PadLeft");
                inputKeys.Add(Commands.MenuRight, "PadRight");

                inputKeys.Add(Commands.GameStart, "PadStart");
            }
            else if (controlScheme == InputManager.ControlScheme.KeyboardWASD)
            {
                inputKeys.Add(Commands.Up, "up");
                inputKeys.Add(Commands.Down, "down");
                inputKeys.Add(Commands.Left, "left");
                inputKeys.Add(Commands.Right, "right");
                inputKeys.Add(Commands.RollLeft, "a");
                inputKeys.Add(Commands.RollRight, "d");
                inputKeys.Add(Commands.ThrottleUp, "w");
                inputKeys.Add(Commands.ThrottleDown, "s");

                inputKeys.Add(Commands.Fire1, "space");
                inputKeys.Add(Commands.Fire2, "enter");
                inputKeys.Add(Commands.Pause, "p");
                inputKeys.Add(Commands.Menu, "escape");
                inputKeys.Add(Commands.Reset, "q");

                inputKeys.Add(Commands.MenuUp, "up");
                inputKeys.Add(Commands.MenuDown, "down");
                inputKeys.Add(Commands.MenuLeft, "left");
                inputKeys.Add(Commands.MenuRight, "right");

                inputKeys.Add(Commands.GameStart, "enter");
            }
            else if (controlScheme == InputManager.ControlScheme.KeyboardNumPad)
            {
                inputKeys.Add(Commands.Up, "numpad8");
                inputKeys.Add(Commands.Down, "numpad2");
                inputKeys.Add(Commands.Left, "numpad4");
                inputKeys.Add(Commands.Right, "numpad6");
                inputKeys.Add(Commands.RollLeft, "numpad7");
                inputKeys.Add(Commands.RollRight, "numpad9");
                inputKeys.Add(Commands.ThrottleUp, "numpad0");
                inputKeys.Add(Commands.ThrottleDown, "numpad5");

                inputKeys.Add(Commands.Fire1, "numpad1");
                inputKeys.Add(Commands.Fire2, "numpad3");
                inputKeys.Add(Commands.Pause, "p");
                inputKeys.Add(Commands.Menu, "escape");
                inputKeys.Add(Commands.Reset, "decimal");

                inputKeys.Add(Commands.MenuUp, "numpad8");
                inputKeys.Add(Commands.MenuDown, "numpad2");
                inputKeys.Add(Commands.MenuLeft, "numpad4");
                inputKeys.Add(Commands.MenuRight, "numpad6");

                inputKeys.Add(Commands.GameStart, "enter");
            }

            if (controlScheme != InputManager.ControlScheme.None)
            {
                inputKeys.Add(Commands.Join, inputKeys[Commands.Fire1]);
                inputKeys.Add(Commands.Leave, inputKeys[Commands.Menu] + "|" + inputKeys[Commands.Fire2]);
                inputKeys.Add(Commands.MenuSelect, inputKeys[Commands.Fire1]);
            }
        }
    }
}
