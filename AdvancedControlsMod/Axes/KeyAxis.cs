﻿using UnityEngine;
using Lench.AdvancedControls.Input;
using System;

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Standard axis takes input from one or two keys and moves the output value at constant speed.
    /// </summary>
    public class KeyAxis : InputAxis
    {
        /// <summary>
        /// Are both bound keys associated devices connected.
        /// </summary>
        public override bool Connected
        {
            get
            {
                return (PositiveBind == null || PositiveBind.Connected) && (NegativeBind == null || NegativeBind.Connected);
            }
        }

        /// <summary>
        /// Status of the axis.
        /// </summary>
        public override AxisStatus Status
        {
            get
            {
                if (!Connected) return AxisStatus.Disconnected;
                return AxisStatus.OK;
            }
        }

        /// <summary>
        /// Gravity value determines how fast the output value returns to the center.
        /// </summary>
        public float Gravity { get; set; }

        /// <summary>
        /// Sensitivity value determines how fast the output value responds to input.
        /// </summary>
        public float Sensitivity { get; set; }

        /// <summary>
        /// Momentum value determines how fast the speed of the value changes.
        /// </summary>
        public float Momentum { get; set; }

        /// <summary>
        /// Snap toggle causes the axis to snap across the center when opposite key is pressed.
        /// </summary>
        public bool Snap { get; set; }

        /// <summary>
        /// Inverts the key binds.
        /// </summary>
        public bool Raw { get; set; }

        /// <summary>
        /// Positive (Right) button bind.
        /// </summary>
        public Button PositiveBind { get; set; }

        /// <summary>
        /// Negative (Left) button bind.
        /// </summary>
        public Button NegativeBind { get; set; }

        private float last = 0;
        private float speed = 0;
        private float value = 0;

        /// <summary>
        /// Creates a standard axis with given name.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public KeyAxis(string name) : base(name)
        {
            Type = AxisType.Key;
            editor = new UI.KeyAxisEditor(this);

            PositiveBind = null;
            NegativeBind = null;
            Sensitivity = 1;
            Gravity = 1;
            Snap = false;
            Raw = false;
        }

        /// <summary>
        /// Input value is 1 if PositiveBind is pressed, -1 if NegativeBind is pressed and 0 if both or none are.
        /// </summary>
        public override float InputValue
        {
            get
            {
                float p = PositiveBind != null ? PositiveBind.Value : 0;
                float n = NegativeBind != null ? NegativeBind.Value * -1 : 0;
                return p + n;
            }
        }

        /// <summary>
        /// Returns output value of the axis.
        /// </summary>
        public override float OutputValue
        {
            get
            {
                return Raw ? InputValue : value;
            }

            protected set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Returns the output value to zero.
        /// </summary>
        protected override void Initialise()
        {
            speed = 0;
            value = 0;
        }

        /// <summary>
        /// Reads input value and updates output value depending on pressed keys and settings.
        /// </summary>
        protected override void Update()
        {
            float g_force = OutputValue > 0 ? -Gravity : Gravity;
            float force = InputValue * Sensitivity + (1 - Mathf.Abs(InputValue)) * g_force;
            if (Momentum == 0)
                speed = force;
            else
                speed += force * Time.deltaTime / Momentum;
            OutputValue = Mathf.Clamp(OutputValue + speed * Time.deltaTime, -1, 1);
            if (Snap && Mathf.Abs(OutputValue - InputValue) > 1)
            {
                speed = 0;
                OutputValue = 0;
            }
            if (InputValue == 0 && Gravity != 0 && (last > 0 != OutputValue > 0))
            {
                speed = 0;
                OutputValue = 0;
            }
            last = OutputValue;
            if (OutputValue == -1 || OutputValue == 1)
                speed = 0;
        }

        internal override InputAxis Clone()
        {
            var clone = new KeyAxis(Name);
            clone.PositiveBind = PositiveBind;
            clone.NegativeBind = NegativeBind;
            clone.Sensitivity = Sensitivity;
            clone.Gravity = Gravity;
            clone.Snap = Snap;
            clone.Raw = Raw;
            clone.Momentum = Momentum;
            return clone;
        }

        internal override void Load()
        {
            Sensitivity = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-sensitivity", Sensitivity);
            Gravity = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-gravity", Gravity);
            Momentum = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-momentum", Momentum);
            Snap = spaar.ModLoader.Configuration.GetBool("axis-" + Name + "-snap", Snap);
            Raw = spaar.ModLoader.Configuration.GetBool("axis-" + Name + "-raw", Raw);
            PositiveBind = ParseButtonID(spaar.ModLoader.Configuration.GetString("axis-" + Name + "-positive", null));
            NegativeBind = ParseButtonID(spaar.ModLoader.Configuration.GetString("axis-" + Name + "-negative", null));
        }

        internal override void Load(MachineInfo machineInfo)
        {
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-sensitivity"))
                Sensitivity = machineInfo.MachineData.ReadFloat("axis-" + Name + "-sensitivity");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-gravity"))
                Gravity = machineInfo.MachineData.ReadFloat("axis-" + Name + "-gravity");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-momentum"))
                Momentum = machineInfo.MachineData.ReadFloat("axis-" + Name + "-momentum");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-snap"))
                Snap = machineInfo.MachineData.ReadBool("axis-" + Name + "-snap");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-raw"))
                Raw = machineInfo.MachineData.ReadBool("axis-" + Name + "-raw");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-positive"))
                PositiveBind = ParseButtonID(machineInfo.MachineData.ReadString("axis-" + Name + "-positive"));
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-negative"))
                NegativeBind = ParseButtonID(machineInfo.MachineData.ReadString("axis-" + Name + "-negative"));
        }

        internal override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-sensitivity", Sensitivity);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-gravity", Gravity);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-momentum", Momentum);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-snap", Snap);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-raw", Raw);
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-positive", PositiveBind != null ? PositiveBind.ID : "None");
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-negative", NegativeBind != null ? NegativeBind.ID : "None");
        }

        internal override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("axis-" + Name + "-type", Type.ToString());
            machineInfo.MachineData.Write("axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("axis-" + Name + "-gravity", Gravity);
            machineInfo.MachineData.Write("axis-" + Name + "-momentum", Momentum);
            machineInfo.MachineData.Write("axis-" + Name + "-snap", Snap);
            machineInfo.MachineData.Write("axis-" + Name + "-raw", Raw);
            machineInfo.MachineData.Write("axis-" + Name + "-positive", PositiveBind != null ? PositiveBind.ID : "None");
            machineInfo.MachineData.Write("axis-" + Name + "-negative", NegativeBind != null ? NegativeBind.ID : "None");
        }

        internal override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-sensitivity");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-gravity");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-momentum");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-snap");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-raw");
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

        /// <summary>
        /// Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public override bool Equals(InputAxis other)
        {
            var cast = other as KeyAxis;
            if (cast == null) return false;
            return this.Name == cast.Name &&
                   this.Sensitivity == cast.Sensitivity &&
                   this.Gravity == cast.Gravity &&
                   this.Snap == cast.Snap &&
                   this.Raw == cast.Raw &&
                   this.PositiveBind == cast.PositiveBind &&
                   this.NegativeBind == cast.NegativeBind;
        }
    }
}