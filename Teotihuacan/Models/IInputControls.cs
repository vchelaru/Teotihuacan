using FlatRedBall.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.Models
{
    public interface IInputControls
    {
        IInputDevice PrimaryInputDevice { get; }
        Mouse SecondaryInputDevice { get; }

        bool IsPrimaryFireDown { get; }

        bool WasSwapWeaponsBackJustPressed { get; }
        bool WasSwapWeaponsForwardJustPressed { get; }

        bool WasPauseJustPressed { get; }

        bool WasJoinJustPressed { get; }

        bool WasLeaveJustPressed { get; }

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
        Vector2? TryGetAimingVector(ref Vector3 origin);
    }
}
