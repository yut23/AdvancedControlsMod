﻿using Lench.AdvancedControls.Axes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lench.AdvancedControls
{
    internal static class Configuration
    {
        internal static void Load()
        {
            try
            {
                // load mod configuration
                ACM.Instance.ModEnabled = spaar.ModLoader.Configuration.GetBool("acm-enabled", true);
                ACM.Instance.ModUpdaterEnabled = spaar.ModLoader.Configuration.GetBool("mod-updater-enabled", true);
                ACM.Instance.DBUpdaterEnabled = spaar.ModLoader.Configuration.GetBool("db-updater-enabled", true);

                // read input axes
                int count = spaar.ModLoader.Configuration.GetInt("number-of-axes", 0);
                for (int i = 0; i < count; i++)
                {
                    string name = spaar.ModLoader.Configuration.GetString("axis-" + i + "-name", null);
                    InputAxis axis = null;
                    if (name != null)
                    {
                        var type = spaar.ModLoader.Configuration.GetString("axis-" + name + "-type", null);
                        if (type == AxisType.Chain.ToString())
                            axis = new ChainAxis(name);
                        if (type == AxisType.Controller.ToString())
                            axis = new ControllerAxis(name);
                        if (type == AxisType.Custom.ToString())
                            axis = new CustomAxis(name);
                        if (type == AxisType.Standard.ToString() || // backwards compatibility
                            type == AxisType.Inertial.ToString() || // backwards compatibility
                            type == AxisType.Key.ToString())
                            axis = new KeyAxis(name);
                        if (type == AxisType.Mouse.ToString())
                            axis = new MouseAxis(name);
                    }
                    if (axis != null)
                    {
                        axis?.Load();
                        AxisManager.AddLocalAxis(axis);
                    }
                }

                // refresh chain axis links
                foreach (var entry in AxisManager.LocalAxes)
                {
                    if (entry.Value.Type == AxisType.Chain)
                        (entry.Value as ChainAxis).RefreshLinks();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error loading saved axes:");
                Debug.LogException(e);
            }
        }

        internal static void Save()
        {
            string log = "";
            try
            {
                spaar.ModLoader.Configuration.SetBool("acm-enabled", ACM.Instance.ModEnabled);
                spaar.ModLoader.Configuration.SetBool("mod-updater-enabled", ACM.Instance.ModUpdaterEnabled);
                spaar.ModLoader.Configuration.SetBool("db-updater-enabled", ACM.Instance.DBUpdaterEnabled);

                int count = spaar.ModLoader.Configuration.GetInt("number-of-axes", 0);
                log += "Attempting to clear " + count + " existing axes.\n";
                for (int i = 0; i < count; i++)
                    spaar.ModLoader.Configuration.RemoveKey("axis-" + i + "-name");

                log += "\tExisting axis list removed.\n\n";

                List<string> axis_names = new List<string>();

                foreach (KeyValuePair<string, InputAxis> entry in AxisManager.LocalAxes)
                {
                    log += "Attempting to save axis '"+entry.Key+ "'.\n";
                    axis_names.Add(entry.Key);
                    entry.Value.Save();
                    log += "\tSuccessfully saved axis '" + entry.Key + "'.\n";
                }

                spaar.ModLoader.Configuration.SetInt("number-of-axes", AxisManager.LocalAxes.Count);
                log += "\nWrote new number of axes: " + AxisManager.LocalAxes.Count + ".\n";
                for (int i = 0; i < axis_names.Count; i++)
                    spaar.ModLoader.Configuration.SetString("axis-" + i + "-name", axis_names[i]);
                log += "Successfully wrote axis list.\n";

                spaar.ModLoader.Configuration.Save();
                log += "Successfully saved configuration.";
            }
            catch (Exception e)
            {
                Debug.Log("[ACM]: Error saving axes:");
                Debug.LogException(e);
                log += "\nException thrown:\n";
                log += e.Message + "\n";
                log += e.StackTrace;
            }
            System.IO.File.WriteAllText(Application.dataPath + "/Mods/Debug/ACM_Log.txt", log);
        }
    }
}
