using FlatRedBall.Gui;
using FlatRedBall.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.Models
{
    public class KeyboardMouseControls : IInputControls
    {
        public IInputDevice PrimaryInputDevice
        {
            get { return InputManager.Keyboard; }
        }

        public Mouse SecondaryInputDevice
        {
            get { return InputManager.Mouse; }
        }

        public bool IsPrimaryFireDown
        {
            get { return InputManager.Mouse.ButtonDown(Mouse.MouseButtons.LeftButton); }
        }

        private readonly IPressableInput _SwapWeaponsBackKey;
        public bool WasSwapWeaponsBackJustPressed
        {
            get { return _SwapWeaponsBackKey.WasJustPressed; }
        }

        private readonly IPressableInput _SwapWeaponsForwardKey;
        public bool WasSwapWeaponsForwardJustPressed
        {
            get { return _SwapWeaponsForwardKey.WasJustPressed; }
        }

        private readonly IPressableInput _PauseKey;
        public bool WasPauseJustPressed
        {
            get { return _PauseKey.WasJustPressed; }
        }

        private readonly IPressableInput _JoinKey;
        public bool WasJoinJustPressed
        {
            get { return _JoinKey.WasJustPressed; }
        }

        private readonly IPressableInput _LeaveKey;
        public bool WasLeaveJustPressed
        {
            get { return _LeaveKey.WasJustPressed; }
        }



        public KeyboardMouseControls()
        {
            var keyboard = InputManager.Keyboard;

            _SwapWeaponsBackKey = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Q);
            _SwapWeaponsForwardKey = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.E);
            _PauseKey = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Escape);
            _JoinKey = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Insert);
            _LeaveKey = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Delete);
        }



        public Vector2? TryGetAimingVector(ref Vector3 origin)
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
    }
}
