using FlatRedBall.Instructions;
using Gum.DataTypes.Variables;
using Gum.Wireframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teotihuacan.UiControllers
{
    public class BarController
    {
        public float MaxHeight { get; set; }

        public GraphicalUiElement Bar { get; set; }

        FlatRedBall.Screens.Screen CurrentScreen => FlatRedBall.Screens.ScreenManager.CurrentScreen;

        public void InterpolateToRatio(float ratio, bool isLevelUp)
        {
            var currentRatio = Bar.Height / MaxHeight;

            var currentState = new StateSave();
            currentState.SetValue(nameof(Bar.Height), Bar.Height, "float");

            float newHeight;

            if(!isLevelUp)
            {
                newHeight = ratio * MaxHeight;
            }
            else
            {
                newHeight = MaxHeight;
            }

            var newState = new StateSave();
            newState.SetValue(nameof(Bar.Height), newHeight, "float");

            Bar.InterpolateTo(currentState, newState, 1, FlatRedBall.Glue.StateInterpolation.InterpolationType.Exponential, FlatRedBall.Glue.StateInterpolation.Easing.Out);

            if(isLevelUp)
            {
                CurrentScreen.Call(() =>
                {
                    Bar.Height = 0;
                    if(ratio > 0)
                    {
                        InterpolateToRatio(ratio, false);
                    }
                })
                    .After(1.1f);
            }

        }
    }
}
