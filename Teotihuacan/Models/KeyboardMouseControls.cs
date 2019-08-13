using FlatRedBall.Gui;
using FlatRedBall.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace Teotihuacan.Models
{
    public class KeyboardMouseControls : InputControls
    {
        public override IInputDevice PrimaryInputDevice
        {
            get { return InputManager.Keyboard; }
        }

        public override Mouse SecondaryInputDevice { get { return InputManager.Mouse; } }

        public override bool AreConnected { get { return true; } }



        public KeyboardMouseControls() : base(KeyboardAndMouse_ControlsID)
        {
            var keyboard = InputManager.Keyboard;

            _PrimaryFireInput = InputManager.Mouse.GetButton(InputControls.KeyboardAndMouse_Button_FrimaryFire);
            _SwapWeaponsBackInput = keyboard.GetKey(InputControls.KeyboardAndMouse_Button_SwapWeaponsBack);
            _SwapWeaponsForwardInput = keyboard.GetKey(InputControls.KeyboardAndMouse_Button_SwapWeaponsForward);
            _PauseInput = keyboard.GetKey(InputControls.KeyboardAndMouse_Button_Pause);
            //_JoinKey = keyboard.GetKey();
            _LeaveInput = keyboard.GetKey(InputControls.KeyboardAndMouse_Button_Leave);
        }



        public override Vector2? TryGetAimingVector(ref Vector3 origin)
        {
            Vector2 cursorPosition = new Vector2(GuiManager.Cursor.WorldXAt(origin.Z), GuiManager.Cursor.WorldYAt(origin.Z));

            Vector2 newAimingVector = cursorPosition - new Vector2(origin.X, origin.Y);

            // in case the stick happens to report 0:
            if (newAimingVector.X == 0 && newAimingVector.Y == 0)
            {
                newAimingVector.X = 1;
            }
            else
            {

            }

            //Normalize at the end in case the right stick input is not at max magnitude
            newAimingVector.Normalize();

            return newAimingVector;
        }

        public override string ToString()
        {
            return "KeyboardMouseControls ID: " + KeyboardAndMouse_ControlsID;
        }
    }
}
