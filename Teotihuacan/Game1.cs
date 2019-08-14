using System;
using System.Collections.Generic;
using System.Reflection;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Screens;
using Microsoft.Xna.Framework;

using System.Linq;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FlatRedBall.Input;

using Teotihuacan.Utilities;
using static Teotihuacan.Utilities.WinApi;

using System.Diagnostics;

namespace Teotihuacan
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public Game1() : base()
        {
            graphics = new GraphicsDeviceManager(this);

#if WINDOWS_PHONE || ANDROID || IOS

            // Frame rate is 30 fps by default for Windows Phone,
            // so let's keep that for other phones too
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            graphics.IsFullScreen = true;
#elif WINDOWS || DESKTOP_GL
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
#endif


#if WINDOWS_8
            FlatRedBall.Instructions.Reflection.PropertyValuePair.TopLevelAssembly = 
                this.GetType().GetTypeInfo().Assembly;
#endif

        }

        protected override void Initialize()
        {
#if IOS
            var bounds = UIKit.UIScreen.MainScreen.Bounds;
            var nativeScale = UIKit.UIScreen.MainScreen.Scale;
            var screenWidth = (int)(bounds.Width * nativeScale);
            var screenHeight = (int)(bounds.Height * nativeScale);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
#endif

        
            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
            FlatRedBallServices.GraphicsOptions.TextureFilter = TextureFilter.Point;
            SpriteManager.OrderedSortType = SortType.ZSecondaryParentY;

			CameraSetup.SetupCamera(SpriteManager.Camera, graphics);
			GlobalContent.Initialize();
			FlatRedBall.Screens.ScreenManager.Start(typeof(Teotihuacan.Screens.MainMenuScreen));

            base.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {
            FlatRedBallServices.Update(gameTime);

            FlatRedBall.Screens.ScreenManager.Activity();

            GlobalActivity();

            base.Update(gameTime);
        }

        /// <summary>
        /// * Code for fullscreen / windowed toggle
        /// </summary>
        private void GlobalActivity()
        {
            if(InputManager.Keyboard.IsAltDown && InputManager.Keyboard.KeyPushed(Keys.Enter))
            {
                if (CameraSetup.Data.IsFullScreen)
                {
                    CameraSetup.Data.IsFullScreen = false;
                    CameraSetup.Data.AllowWidowResizing = true;
                    var xnaWindow = FlatRedBall.FlatRedBallServices.Game.Window;
                    xnaWindow.AllowUserResizing = true;
                    //xnaWindow.IsBorderless = false;

                    WinApi.UnsetWindowAlwaysOnTop(this.Window.Handle);

                    CameraSetup.ResetWindow();
                }
                else
                {
                    CameraSetup.Data.IsFullScreen = true;
                    CameraSetup.Data.AllowWidowResizing = false;
                    var xnaWindow = FlatRedBall.FlatRedBallServices.Game.Window;
                    xnaWindow.AllowUserResizing = false;
                    //xnaWindow.IsBorderless = true; // This for some reason breaks the OnTop behaviour. MG bug ?

                    WinApi.SetWindowAlwaysOnTop(this.Window.Handle);

                    CameraSetup.ResetWindow();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            FlatRedBallServices.Draw();

            base.Draw(gameTime);
        }
    }
}
