﻿using System.Collections.Generic;
using System.Windows.Controls;

namespace IndiaTango.Models
{
    public interface IDetectionMethod
    {
        /// <summary>
        /// The Name of the Detection Method
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Detection Method itself (must go deeper)
        /// </summary>
        IDetectionMethod This { get; }

        /// <summary>
        /// The list of erroneous values the method finds
        /// </summary>
        /// <param name="sensorToCheck">The sensor to look within</param>
        /// <returns>The list of erroneous values</returns>
        List<ErroneousValue> GetDetectedValues(Sensor sensorToCheck);

        /// <summary>
        /// Does this method have additional settings?
        /// </summary>
        bool HasSettings { get; }

        /// <summary>
        /// The layout Grid for the methods settings
        /// </summary>
        Grid SettingsGrid { get; }

        /// <summary>
        /// Does this method give anything to graph?
        /// </summary>
        bool HasGraphableSeries { get; }
    }
}
