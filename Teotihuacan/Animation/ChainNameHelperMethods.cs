﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teotihuacan.Entities;

namespace Teotihuacan.Animation
{
    public static class ChainNameHelperMethods
    {
        public static string GenerateChainName(PrimaryActions primaryAction, SecondaryActions secondaryAction, TopDownDirection direction)
        {
            return $"{primaryAction.ToFriendlyString()}{secondaryAction.ToFriendlyString()}{direction.ToFriendlyString()}";
        }

        public static string GenerateChainName(PrimaryActions primaryAction, TopDownDirection direction)
        {
            return $"{primaryAction.ToFriendlyString()}{direction.ToFriendlyString()}";
        }
    }
}