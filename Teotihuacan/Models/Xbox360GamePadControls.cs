using FlatRedBall.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.Models
{
    public class Xbox360GamePadControls : IInputControls
    {
        private readonly Xbox360GamePad _GamePad;

        public IInputDevice PrimaryInputDevice
        {
            get { return _GamePad; }
        }

        public Mouse SecondaryInputDevice
        {
            get { return null; }
        }

        public bool IsPrimaryFireDown
        {
            get { return _GamePad.RightTrigger.IsDown; }
        }

        private readonly IPressableInput _SwapWeaponsBackButton;
        public bool WasSwapWeaponsBackJustPressed
        {
            get { return _SwapWeaponsBackButton.WasJustPressed; }
        }

        private readonly IPressableInput _SwapWeaponsForwardButton;
        public bool WasSwapWeaponsForwardJustPressed
        {
            get { return _SwapWeaponsForwardButton.WasJustPressed; }
        }

        private readonly IPressableInput _PauseButton;
        public bool WasPauseJustPressed
        {
            get { return _PauseButton.WasJustPressed; }
        }

        private readonly IPressableInput _JoinButton;
        public bool WasJoinJustPressed
        {
            get { return _JoinButton.WasJustPressed; }
        }

        private readonly IPressableInput _LeaveButton;
        public bool WasLeaveJustPressed
        {
            get { return _LeaveButton.WasJustPressed; }
        }

        private I2DInput _AimInput;


        public Xbox360GamePadControls(Xbox360GamePad gamePad)
        {
            _GamePad = gamePad;

            _SwapWeaponsBackButton = gamePad.GetButton(Xbox360GamePad.Button.LeftShoulder);
            _SwapWeaponsForwardButton = gamePad.GetButton(Xbox360GamePad.Button.RightShoulder);
            _PauseButton = _JoinButton = gamePad.GetButton(Xbox360GamePad.Button.Start);
            _LeaveButton = gamePad.GetButton(Xbox360GamePad.Button.Back);

            _AimInput = gamePad.RightStick;
        }


        public Vector2? TryGetAimingVector(ref Vector3 origin)
        {
            if (_AimInput.Magnitude > 0)
            {
                Vector2 newAimingVector = new Vector2(_AimInput.X, _AimInput.Y);

                // In case the stick happens to report 0:
                if (newAimingVector.X == 0 && newAimingVector.Y == 0)
                {
                    newAimingVector.X = 1;
                }

                // Normalize at the end in case the right stick input is not at max magnitude
                newAimingVector.Normalize();

                return newAimingVector;
            }
            else
            {
                return null;
            }
        }
    }
}
