﻿using System;
using System.Collections.Generic;
using UnityEngine;
using LenchScripter;
using LenchScripter.Blocks;
using AdvancedControls.UI;
using spaar.ModLoader.UI;

namespace AdvancedControls.Controls
{
    public class ControlGroup : Control
    {
        public ControlGroup(string guid) : base(guid) { }

        public Dictionary<string, Control> Controls { get; set; } = new Dictionary<string, Control>();

        private string enabled;
        public new string Enabled
        {
            get { return enabled; }
            set
            {
                foreach (KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.Enabled = c.Key == value;
                }
                enabled = value;
            }
        }

        public override Block Block
        {
            get { return base.Block; }
            set
            {
                foreach (KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.Block = Block;
                }
                base.Block = value;
            }
        }

        public override string BlockGUID
        {
            get { return base.BlockGUID; }
            set
            {
                foreach (KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.BlockGUID = value;
                }
                base.BlockGUID = value;
            }
        }

        public override void Apply(float value) { }

        public override void Draw()
        {
            Control enabled = null;
            GUILayout.BeginHorizontal();
            foreach (KeyValuePair<string, Control> c in Controls)
            {
                if (GUILayout.Button(c.Value.Name, c.Value.Enabled ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                {
                    Enabled = c.Key;
                }
                if (c.Value.Enabled) enabled = c.Value;
            }
            GUILayout.EndHorizontal();
            enabled?.Draw();
        }
    }
}