using FlatRedBall.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.Models
{
    public class Xbox360GamePadControls : InputControls
    {
        private readonly Xbox360GamePad _GamePad;

        public override IInputDevice PrimaryInputDevice
        {
            get { return _GamePad; }
        }

        public override Mouse SecondaryInputDevice { get { return null; } }

        private I2DInput _AimInput;

        public override bool AreConnected
        {
            get { return _GamePad.IsConnected; }
        }



        public Xbox360GamePadControls(Xbox360GamePad gamePad, int gamePadIndex) : base(gamePadIndex)
        {
            _GamePad = gamePad;

            _PrimaryFireInput = gamePad.GetButton(InputControls.Xbox360GamePad_Button_FrimaryFire);
            _SwapWeaponsBackInput = gamePad.GetButton(InputControls.Xbox360GamePad_Button_SwapWeaponsBack);
            _SwapWeaponsForwardInput = gamePad.GetButton(InputControls.Xbox360GamePad_Button_SwapWeaponsForward);
            //_JoinButton =
            _PauseInput = gamePad.GetButton(InputControls.Xbox360GamePad_Button_Pause);
            _LeaveInput = gamePad.GetButton(InputControls.Xbox360GamePad_Button_Leave);

            _AimInput = gamePad.RightStick;
        }


        public override Vector2? TryGetAimingVector(ref Vector3 origin)
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


        public override string ToString()
        {
            return "Xbox360GamePadControls ID: " + ControlsID;
        }
        
    }
}
