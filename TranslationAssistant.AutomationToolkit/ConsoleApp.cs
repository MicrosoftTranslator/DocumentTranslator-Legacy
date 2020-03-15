// // ----------------------------------------------------------------------
// // <copyright file="Program.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>Program.cs</summary>
// // ----------------------------------------------------------------------

using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Reflection;

using TranslationAssistant.AutomationToolkit.BasePlugin;

namespace TranslationAssistant.AutomationToolkit
{
    /// <summary>
    ///     The console app.
    /// </summary>
    internal class ConsoleApp
    {
        #region Static Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private static readonly ILogger Logger = new ConsoleLogger();

        /// <summary>
        ///     The plugin mappings.
        /// </summary>
        private static Hashtable pluginMappings = new Hashtable();

        #endregion

        #region Methods

        /// <summary>
        ///     The get plugin hash.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool GetPluginHash()
        {
            var registeredPlugins = (IDictionary)ConfigurationManager.GetSection("RegisteredPlugins");

            // ConfigurationSettings.GetConfig("PluginDependencies");
            pluginMappings = new Hashtable();
            if (registeredPlugins == null)
            {
                Logger.WriteLine(LogLevel.Error, "ERROR: Cannot locate application configuration data.");
                return false;
            }

            foreach (string pluginName in registeredPlugins.Keys)
            {
                if (!pluginMappings.ContainsKey(pluginName.ToLower(CultureInfo.InvariantCulture).Trim()))
                {
                    pluginMappings.Add(
                        pluginName.ToLower(CultureInfo.InvariantCulture).Trim(),
                        registeredPlugins[pluginName]);
                }
                else
                {
                    Logger.WriteLine(
                        LogLevel.Debug,
                        "WARN: Duplicated XML entry for {0}. New value will be ignored.",
                        pluginName);
                }
            }

            return true;
        }

        /// <summary>
        ///     The initialize plugin.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="BasePlugIn" />.
        /// </returns>
        private static BasePlugIn InitializePlugin(string name)
        {
            if (pluginMappings.ContainsKey(name.ToLower(CultureInfo.InvariantCulture).Trim()))
            {
                if (pluginMappings[name.ToLower(CultureInfo.InvariantCulture).Trim()] is string)
                {
                    string path = (string)pluginMappings[name.ToLower(CultureInfo.InvariantCulture).Trim()];
                    try
                    {
                        //Assembly assembly = Assembly.LoadFrom(path);
                        Assembly assembly = Assembly.UnsafeLoadFrom(path);
                        Type pluginType =
                            assembly.GetType(
                                "TranslationAssistant.AutomationToolkit.TranslationPlugins." + name,
                                true,
                                true);
                        var plugin =
                            (BasePlugIn)
                            assembly.CreateInstance(
                                pluginType.FullName,
                                true,
                                BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance
                                | BindingFlags.InvokeMethod,
                                null,
                                new object[] { Logger },
                                null,
                                null);
                        if (plugin != null)
                        {
                            pluginMappings[name.ToLower(CultureInfo.InvariantCulture).Trim()] = plugin;
                            return plugin;
                        }

                        Logger.WriteLine(LogLevel.Error, "ERROR: Failed to load plugin {0}.", name);
                        Logger.WriteLine(LogLevel.Error, "CreateInstance( ) return null.");
                    }
                    catch (DivideByZeroException fe)
                    {
                        Logger.WriteLine(LogLevel.Error, "ERROR: Failed to load plugin {0}.", name);
                        Logger.WriteException(fe);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LogLevel.Error, "ERROR: Failed to load plugin {0}.", name);
                        Logger.WriteException(e);
                    }
                }
                else
                {
                    BasePlugIn basePlugIn =
                        pluginMappings[name.ToLower(CultureInfo.InvariantCulture).Trim()] as BasePlugIn;
                    if (basePlugIn != null)
                    {
                        // plugin is already initialized.
                        return basePlugIn;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     The main.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void Main(string[] args)
        {
            bool returnValue = false;
            if (!GetPluginHash())
            {
                return;
            }

            if (args.Length <= 0)
            {
                Logger.WriteLine(LogLevel.Error, "ERROR: At least 1 argument is required.");
                Usage();
            }
            else if (args.Length == 1
                     && (args[0].ToLower(CultureInfo.InvariantCulture).Trim().Equals("/?")
                         || args[0].ToLower(CultureInfo.InvariantCulture).Trim().Equals("/h")
                         || args[0].ToLower(CultureInfo.InvariantCulture).Trim().Equals("/help")))
            {
                Usage();
            }
            else
            {
                BasePlugIn plugin = InitializePlugin(args[0]);
                if (plugin == null)
                {
                    Logger.WriteLine(LogLevel.Error, "ERROR: Unknown functionality.");
                    Usage();
                }
                else
                {
                    var functionalArguments = new string[args.Length - 1];
                    bool helpInvoked = false;
                    for (int i = 1; i < args.Length; i++)
                    {
                        string arg = args[i].ToLower(CultureInfo.InvariantCulture).Trim();
                        if (arg != "/h" && arg != "-h" && arg != "\\h" && arg != "/?" && arg != "-?" && arg != "\\?"
                            && arg != "/help" && arg != "-help" && arg != "\\help")
                        {
                            functionalArguments[i - 1] = args[i];
                        }
                        else
                        {
                            Logger.WriteLine(LogLevel.Debug, "User activated plug-in help menu.");
                            Logger.WriteLine(LogLevel.Msg, "Usage details for [{0}]", plugin.Name);
                            Logger.WriteLine(LogLevel.Msg, string.Empty);
                            plugin.Usage();
                            Logger.WriteLine(LogLevel.Msg, string.Empty);

                            // Usage();
                            helpInvoked = true;
                            break;
                        }
                    }

                    if (!helpInvoked)
                    {
                        if (plugin.Parse(functionalArguments))
                        {
                            returnValue = plugin.Execute();
                        }
                        else
                        {
                            Logger.WriteLine(LogLevel.Error, "ERROR: Error in arguments.");
                        }
                    }
                }
            }

            Environment.ExitCode = returnValue ? 0 : 1;
        }

        /// <summary>
        ///     The usage.
        /// </summary>
        private static void Usage()
        {
            Logger.WriteLine(LogLevel.Msg, "General Usage:");
            Logger.WriteLine(LogLevel.Msg, string.Empty);
            Logger.WriteLine(LogLevel.Msg, "  DocumentTranslatorCmd.exe [plugin name] [plugin parameters]");
            Logger.WriteLine(LogLevel.Msg, string.Empty);
            Logger.WriteLine(LogLevel.Msg, "  Available plugins are:");
            Logger.WriteLine(LogLevel.Msg, string.Empty);
            foreach (string name in pluginMappings.Keys)
            {
                Logger.WriteLine(LogLevel.Msg, "      {0}", name);
            }

            Logger.WriteLine(LogLevel.Msg, string.Empty);
            Logger.WriteLine(
                LogLevel.Msg,
                "  Type \"DocumentTranslatorCmd.exe [plugin name] /?\" for detail help for that plugin.");
        }

        #endregion
    }
}