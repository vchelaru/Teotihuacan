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
    public abstract class InputControls
    {
        public const Xbox360GamePad.Button Xbox360GamePad_Button_Join = Xbox360GamePad.Button.Start;
        public const Xbox360GamePad.Button Xbox360GamePad_Button_Leave = Xbox360GamePad.Button.Back;
        public const Xbox360GamePad.Button Xbox360GamePad_Button_Pause = Xbox360GamePad.Button.Start;
        public const Xbox360GamePad.Button Xbox360GamePad_Button_FrimaryFire = Xbox360GamePad.Button.RightTrigger;
        public const Xbox360GamePad.Button Xbox360GamePad_Button_SwapWeaponsBack = Xbox360GamePad.Button.LeftShoulder;
        public const Xbox360GamePad.Button Xbox360GamePad_Button_SwapWeaponsForward = Xbox360GamePad.Button.RightShoulder;

        public const XnaInput.Keys KeyboardAndMouse_Button_Join = XnaInput.Keys.Insert;
        public const XnaInput.Keys KeyboardAndMouse_Button_Leave = XnaInput.Keys.Delete;
        public const XnaInput.Keys KeyboardAndMouse_Button_Pause = XnaInput.Keys.Escape;
        public const Mouse.MouseButtons KeyboardAndMouse_Button_FrimaryFire = Mouse.MouseButtons.LeftButton;
        public const XnaInput.Keys KeyboardAndMouse_Button_SwapWeaponsBack = XnaInput.Keys.Q;
        public const XnaInput.Keys KeyboardAndMouse_Button_SwapWeaponsForward = XnaInput.Keys.E;

        public const int KeyboardAndMouse_ControlsID = 255;


        public abstract IInputDevice PrimaryInputDevice { get; }

        public abstract Mouse SecondaryInputDevice { get; }

        public readonly int ControlsID;

        public abstract bool AreConnected { get; }


        protected IPressableInput _PrimaryFireInput;
        public bool IsPrimaryFireDown
        {
            get { return _PrimaryFireInput.IsDown; }
        }

        protected IPressableInput _SwapWeaponsBackInput;
        public bool WasSwapWeaponsBackJustPressed
        {
            get { return _SwapWeaponsBackInput.WasJustPressed; }
        }

        protected IPressableInput _SwapWeaponsForwardInput;
        public bool WasSwapWeaponsForwardJustPressed
        {
            get { return _SwapWeaponsForwardInput.WasJustPressed; }
        }

        protected IPressableInput _PauseInput;
        public bool WasPauseJustPressed
        {
            get { return _PauseInput.WasJustPressed; }
        }

        //protected IPressableInput _JoinInput;
        //public override bool WasJoinJustPressed
        //{
        //    get { return _JoinInput.WasJustPressed; }
        //}

        protected IPressableInput _LeaveInput;
        public bool WasLeaveJustPressed
        {
            get { return _LeaveInput.WasJustPressed; }
        }



        protected InputControls(int controlsID)
        {
            ControlsID = controlsID;
        }



        /*/// <summary>
        /// 
        /// </summary>
        /// <param name="origin">Coordinates of origin (PlayerCharacter position) for aiming vector calculation from Mouse pointer position.</param>
        /// <param name="aimingVector">Normalized Vector2</param>
        /// <returns>False if aiming vector magnitude is 0.</returns>
        bool TryGetAimingVector(ref Vector3 origin, out Vector2 aimingVector);*/

        /// <summary></summary>
        /// <param name="origin">Coordinates of origin (PlayerCharacter position) for aiming vector calculation from Mouse pointer position.</param>
        /// <returns>Normalized Vector2 or null if aiming vector magnitude is 0.</returns>
        public abstract Vector2? TryGetAimingVector(ref Vector3 origin);
    }
}
