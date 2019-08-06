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
			FlatRedBall.Screens.ScreenManager.Start(typeof(Teotihuacan.Screens.Level1));

            base.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {
            FlatRedBallServices.Update(gameTime);

            FlatRedBall.Screens.ScreenManager.Activity();

            GlobalActivity();

            base.Update(gameTime);
        }

        bool dbgIsOnTop = false;
        private void GlobalActivity()
        {
            if(InputManager.Keyboard.IsAltDown && InputManager.Keyboard.KeyPushed(Keys.Enter))
            {
                if (CameraSetup.Data.IsFullScreen)
                //if (dbgIsOnTop)
                {
                    //Debug.WriteLine("Game1.GlobalActivity(): IsOnTop = true   =>   switching off OnTop");

                    CameraSetup.Data.IsFullScreen = false;
                    CameraSetup.Data.AllowWidowResizing = true;
                    var xnaWindow = FlatRedBall.FlatRedBallServices.Game.Window;
                    xnaWindow.AllowUserResizing = true;
                    //xnaWindow.IsBorderless = false;

                    dbgIsOnTop = false;

                    WinApi.SetWindowPos(
                        this.Window.Handle,
                        WinApi.HWND_NOTOPMOST,
                        0, 0,
                        FlatRedBallServices.GraphicsOptions.ResolutionWidth, FlatRedBallServices.GraphicsOptions.ResolutionHeight,
                        0 //SetWindowPosFlags.ShowWindow //SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize
                    );

                    //User32.UnsetWindowOnTop(
                    //    this.Window.Handle,
                    //    FlatRedBallServices.GraphicsOptions.ResolutionWidth,
                    //    FlatRedBallServices.GraphicsOptions.ResolutionHeight
                    //);

                    CameraSetup.ResetWindow();
                }
                else
                {
                    //Debug.WriteLine("Game1.GlobalActivity(): IsOnTop = false   =>   switching to OnTop");

                    CameraSetup.Data.IsFullScreen = true;
                    CameraSetup.Data.AllowWidowResizing = false;
                    var xnaWindow = FlatRedBall.FlatRedBallServices.Game.Window;
                    xnaWindow.AllowUserResizing = false;
                    //xnaWindow.IsBorderless = true;

                    dbgIsOnTop = true;

                    WinApi.SetWindowPos(
                        this.Window.Handle, WinApi.HWND_TOPMOST,
                        0, 0,
                        FlatRedBallServices.GraphicsOptions.ResolutionWidth, FlatRedBallServices.GraphicsOptions.ResolutionHeight,
                        0 //SetWindowPosFlags.ShowWindow
                    );

                    //User32.SetWindowOnTop(
                    //    this.Window.Handle,
                    //    FlatRedBallServices.GraphicsOptions.ResolutionWidth, 
                    //    FlatRedBallServices.GraphicsOptions.ResolutionHeight
                    //);

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
