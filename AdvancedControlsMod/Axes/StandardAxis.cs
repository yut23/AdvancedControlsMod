﻿using UnityEngine;
using Lench.AdvancedControls.Input;
using System;

namespace Lench.AdvancedControls.Axes
{
    public class StandardAxis : InputAxis
    {
        public override string Name { get; set; } = "new standard axis";
        public override AxisType Type { get { return AxisType.Standard; } }
        public override bool Connected
        {
            get
            {
                return (PositiveBind == null || PositiveBind.Connected) && (NegativeBind == null || NegativeBind.Connected);
            }
        }
        public override AxisStatus Status
        {
            get
            {
                if (!Connected) return AxisStatus.Disconnected;
                return AxisStatus.OK;
            }
        }

        public float Gravity { get; set; }
        public float Sensitivity { get; set; }
        public bool Snap { get; set; }
        public bool Invert { get; set; }
        public Button PositiveBind { get; set; }
        public Button NegativeBind { get; set; }
        private float last = 0;

        public StandardAxis(string name) : base(name)
        {
            editor = new UI.TwoKeyAxisEditor(this);

            PositiveBind = null;
            NegativeBind = null;
            Sensitivity = 1;
            Gravity = 1;
            Snap = false;
            Invert = false;
        }

        public override float InputValue
        {
            get
            {
                float p = PositiveBind != null ? PositiveBind.Value : 0;
                float n = NegativeBind != null ? NegativeBind.Value * -1 : 0;
                return (p + n) * (Invert ? -1 : 1);
            }
        }

        protected override void Initialise()
        {
            OutputValue = 0;
        }

        protected override void Update()
        {
            float g_force = OutputValue > 0 ? -Gravity : Gravity;
            float force = InputValue * Sensitivity + (1 - Mathf.Abs(InputValue)) * g_force;
            OutputValue = Mathf.Clamp(OutputValue + force * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(OutputValue - InputValue) > 1)
                OutputValue = 0;
            if (InputValue == 0 && (last > 0 != OutputValue > 0))
                OutputValue = 0;
            last = OutputValue;
        }

        internal override InputAxis Clone()
        {
            var clone = new StandardAxis(Name);
            clone.PositiveBind = PositiveBind;
            clone.NegativeBind = NegativeBind;
            clone.Sensitivity = Sensitivity;
            clone.Gravity = Gravity;
            clone.Snap = Snap;
            clone.Invert = Invert;
            return clone;
        }

        internal override void Load()
        {
            Sensitivity = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-sensitivity", Sensitivity);
            Gravity = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-gravity", Gravity);
            Snap = spaar.ModLoader.Configuration.GetBool("axis-" + Name + "-snap", Snap);
            Invert = spaar.ModLoader.Configuration.GetBool("axis-" + Name + "-invert", Invert);
            PositiveBind = ParseButtonID(spaar.ModLoader.Configuration.GetString("axis-" + Name + "-positive", null));
            NegativeBind = ParseButtonID(spaar.ModLoader.Configuration.GetString("axis-" + Name + "-negative", null));
        }

        internal override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-sensitivity", Sensitivity);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-gravity", Gravity);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-snap", Snap);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-invert", Invert);
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-positive", PositiveBind != null ? PositiveBind.ID : "None");
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-negative", NegativeBind != null ? NegativeBind.ID : "None");
        }

        internal override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-sensitivity");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-gravity");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-snap");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-invert");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-positive");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-negative");
            Dispose();
        }

        private Button ParseButtonID(string id)
        {
            if (id == null)
                return null;
            try
            {
                Button b = null;
                if (id.StartsWith("key"))
                    b = new Key(id);
                if (id.StartsWith("hat"))
                    b = new HatButton(id);
                if (id.StartsWith("joy"))
                    b = new JoystickButton(id);
                if (b != null)
                    return b;
                return null;
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error while loading a button:");
                Debug.LogException(e);
                return null;
            }
        }
    }
}