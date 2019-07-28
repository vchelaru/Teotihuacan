using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using Teotihuacan.Entities;
using Teotihuacan.Screens;
namespace Teotihuacan.Entities
{
    public partial class Enemy
    {
        void OnAfterCurrentDataCategoryStateSet (object sender, EventArgs e) 
        {
            if(CurrentDataCategoryState != null)
            {
                CurrentHP = MaxHp * StatMultipliers.EffectiveHealthMultiplier;
            }
        }

    }
}
