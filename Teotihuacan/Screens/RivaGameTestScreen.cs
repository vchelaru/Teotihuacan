using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;

using Teotihuacan.Models;
using Teotihuacan.Managers;

namespace Teotihuacan.Screens
{
	public partial class RivaGameTestScreen
	{

		void CustomInitialize()
		{
			PlayerData slotPlayerData;
			if (PlayerManager.TryAssignSlotToPlayerInMainMenu(new KeyboardMouseControls(), out slotPlayerData))
			{
				// join success
				JoinWith(slotPlayerData);
			}
			else
			{
				throw new Exception("R: unexpected");
			}

			foreach (var player in PlayerList)
			{
				SetPlayerHudOnJoin(player);
			}

			InitializeCameraController();
		}

		void CustomActivity(bool firstTimeCalled)
		{


		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
