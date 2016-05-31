﻿using System;
using UnityEngine;
using LenchScripter.Blocks;

namespace AdvancedControls.Controls
{
    public class AngleControl : Control
    {
        public override string Name { get; set; } = "ANGLE";

        private Steering steering;

        public AngleControl(Guid guid) : base(guid)
        {
            Min = -45;
            Center = 0;
            Max = 45;
        }

        public override Block Block
        {
            get
            {
                if (steering != null) return steering;
                return null;
            }
            set
            {
                steering = value as Steering;
            }
        }

        public override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            steering?.SetAngle(value);
        }
    }
}
