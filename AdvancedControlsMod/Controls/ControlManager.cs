﻿using System;
using System.Collections.Generic;
using Lench.AdvancedControls.Axes;

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Control manager handles all controls bound to the machine.
    /// </summary>
    public static class ControlManager
    {
        internal static Dictionary<Guid, List<Control>> Blocks = new Dictionary<Guid, List<Control>>();

        /// <summary>
        /// Gets controls for a block of type BlockID and given GUID.
        /// </summary>
        /// <param name="BlockID">BlockType enumerator value.</param>
        /// <param name="GUID">GUID of the block.</param>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetBlockControls(int BlockID, Guid GUID)
        {
            if (Blocks.ContainsKey(GUID)) return Blocks[GUID];
            var controls = CreateBlockControls(BlockID, GUID);
            Blocks.Add(GUID, controls);
            return controls;
        }

        /// <summary>
        /// Gets controls for a given BlockBehaviour object.
        /// </summary>
        /// <param name="block">BlockBehaviour of the block.</param>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetBlockControls(BlockBehaviour block)
        {
            if (Blocks.ContainsKey(block.Guid)) return Blocks[block.Guid];
            var controls = CreateBlockControls(block.GetBlockID(), block.Guid);
            Blocks.Add(block.Guid, controls);
            return controls;
        }

        /// <summary>
        /// Returns a control with given name.
        /// If such a control is not found, returns null.
        /// </summary>
        /// <param name="BlockID">BlockType enumerator value.</param>
        /// <param name="GUID">GUID of the block.</param>
        /// <param name="name">Name of the control.</param>
        /// <returns>Returns a Control object.</returns>
        public static Control GetBlockControl(int BlockID, Guid GUID, string name)
        {
            foreach (Control c in GetBlockControls(BlockID, GUID))
                if (c.Name == name) return c;
            return null;
        }

        /// <summary>
        /// Copies the controls from a block with GUID source_block to a block with GUID destination_block.
        /// Ignores mismatching controls.
        /// </summary>
        /// <param name="source_block">GUID of the source block.</param>
        /// <param name="destination_block">GUID of the target block.</param>
        public static void CopyBlockControls(Guid source_block, Guid destination_block)
        {
            if (!Blocks.ContainsKey(source_block) || !Blocks.ContainsKey(destination_block)) return;

            foreach (Control src in Blocks[source_block])
                foreach (Control tgt in Blocks[destination_block])
                    if (src.Name == tgt.Name)
                    {
                        tgt.Enabled = src.Enabled;
                        tgt.Axis = src.Axis;
                        tgt.Min = src.Min;
                        tgt.Center = src.Center;
                        tgt.Max = src.Max;
                    }
        }

        private static List<Control> CreateBlockControls(int BlockID, Guid GUID)
        {
            if (BlockID == (int)BlockType.Wheel ||
                BlockID == (int)BlockType.LargeWheel ||
                BlockID == (int)BlockType.CogMediumPowered || 
                BlockID == (int)BlockType.Drill)
            {
                return new List<Control>()
                {
                    new InputControl(GUID),
                    new SliderControl(GUID)
                    {
                        Slider = "SPEED"
                    }
                };
            }

            if (BlockID == (int)BlockType.Piston)
            {
                return new List<Control>()
                {
                    new PositionControl(GUID),
                    new SliderControl(GUID)
                    {
                        Slider = "SPEED"
                    }
                };
            }

            if (BlockID == (int)BlockType.SteeringBlock ||
                BlockID == (int)BlockType.SteeringHinge)
            {
                return new List<Control>()
                {
                    new AngleControl(GUID),
                    new InputControl(GUID),
                    new SliderControl(GUID)
                    {
                        Slider = "ROTATION SPEED"
                    }
                };
            }

            if (BlockID == (int)BlockType.Spring)
            {
                return new List<Control>()
                {
                    new InputControl(GUID)
                    {
                        PositiveOnly = true
                    },
                    new SliderControl(GUID)
                    {
                        Slider = "STRENGTH"
                    }
                };
            }

            if (BlockID == (int)BlockType.RopeWinch)
            {
                return new List<Control>()
                {
                    new InputControl(GUID),
                    new SliderControl(GUID)
                    {
                        Slider = "SPEED"
                    }
                };
            }

            if (BlockID == (int)BlockType.Suspension)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "SPRING"
                    }
                };
            }

            if (BlockID == (int)BlockType.SpinningBlock)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "SPEED"
                    }
                };
            }

            if (BlockID == (int)BlockType.CircularSaw)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "SPEED"
                    }
                };
            }
            
            if (BlockID == (int)BlockType.Flamethrower)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "RANGE",
                        PositiveOnly = true
                    }
                };
            }

            if (BlockID == (int)BlockType.FlyingBlock)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "FLYING SPEED",
                        PositiveOnly = true
                    }
                };
            }

            if (BlockID == (int)BlockType.WaterCannon)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "POWER",
                        PositiveOnly = true
                    }
                };
            }

            if (BlockID == 59) //Rocket
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "THRUST",
                        PositiveOnly = true
                    },
                    new SliderControl(GUID)
                    {
                        Slider = "FLIGHT DURATION",
                        PositiveOnly = true
                    },
                    new SliderControl(GUID){
                        Slider = "EXPLOSIVE CHARGE",
                        PositiveOnly = true
                    },
                };
            }

            if (BlockID == (int)BlockType.Balloon)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "BUOYANCY",
                        PositiveOnly = true
                    },
                    new SliderControl(GUID)
                    {
                        Slider = "STRING LENGTH",
                        PositiveOnly = true
                    }
                };
            }

            if (BlockID == (int)BlockType.Ballast)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "MASS",
                        PositiveOnly = true
                    }
                };
            }

            if (BlockID == (int)BlockType.CameraBlock)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID)
                    {
                        Slider = "DISTANCE",
                        PositiveOnly = true,
                        Min = 40, Center = 60, Max = 80
                    },
                    new SliderControl(GUID)
                    {
                        Slider = "HEIGHT",
                        Min = 0, Center = 30, Max = 60
                    },
                    new SliderControl(GUID)
                    {
                        Slider = "ROTATION",
                        Min = -60, Center = 0, Max = 60
                    }
                };
            }

            if (BlockID == 790)
            {
                return new List<Control>()
                {
                    new VectorControl(GUID, Axis.X),
                    new VectorControl(GUID, Axis.Y),
                    new VectorControl(GUID, Axis.Z)
                    {
                        Min = -1.25f,
                        Max = 1.25f
                    },
                };
            }

            return new List<Control>();
        }

        /// <summary>
        /// Returns a list of all active controls on a block.
        /// A control is active if it is enabled and has bound an axis.
        /// </summary>
        /// <param name="GUID">GUID of the block.</param>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetActiveBlockControls(Guid GUID)
        {
            var list = new List<Control>();
            if (Blocks.ContainsKey(GUID))
            {
                foreach (Control c in Blocks[GUID])
                {
                    if (c.Enabled && AxisManager.Get(c.Axis) != null)
                    {
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Returns a list of all active controls in the machine.
        /// </summary>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetActiveControls()
        {
            var list = new List<Control>();
            foreach(Guid guid in Blocks.Keys)
            {
                list.AddRange(GetActiveBlockControls(guid));
            }
            return list;
        }

    }
}
