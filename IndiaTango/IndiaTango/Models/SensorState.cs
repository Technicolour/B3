﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiaTango.Models
{
    /// <summary>
    /// A class representing a sensor state; that is, the values a sensor held at a given instance in time.
    /// </summary>
    public class SensorState
    {
        private DateTime _editTimestamp;
        private Dictionary<DateTime, float> _valueList;
        private string _reason = "";

        public SensorState Clone()
        {
            var d = _valueList.ToDictionary(v => v.Key, v => v.Value);
            var s = new SensorState(DateTime.Now,d);
            return s;
        }

        /// <summary>
        /// Creates a new sensor state with the specified timestamp representing the date it was last edited.
        /// </summary>
        /// <param name="editTimestamp">A DateTime object representing the last edit date and time for this sensor state.</param>
        public SensorState(DateTime editTimestamp)
            : this(editTimestamp, new Dictionary<DateTime, float>())
        {
        }

        /// <summary>
        /// Creates a new sensor state with the specified timestamp representing the date it was last edited, and a list of values representing data values recorded in this state.
        /// </summary>
        /// <param name="editTimestamp">A DateTime object representing the last edit date and time for this sensor state.</param>
        /// <param name="valueList">A list of data values, representing values recorded in this sensor state.</param>
        public SensorState(DateTime editTimestamp, Dictionary<DateTime, float> valueList) : this(editTimestamp, valueList, "") {}

        /// <summary>
        /// Creates a new sensor state with the specified timestamp representing the date it was last edited, a list of values representing data values recorded in this state, and a reason for the changes stored in this state.
        /// </summary>
        /// <param name="editTimestamp">A DateTime object representing the last edit date and time for this sensor state.</param>
        /// <param name="valueList">A list of data values, representing values recorded in this sensor state.</param>
        /// <param name="reason">A string indicating the reason for the changes made in this state.</param>
        public SensorState(DateTime editTimestamp, Dictionary<DateTime, float> valueList, string reason)
        {
            if (valueList == null)
                throw new ArgumentNullException("The list of values in this state cannot be null.");

            _reason = reason;
            _editTimestamp = editTimestamp;
            _valueList = valueList;
        }

        /// <summary>
        /// Gets or sets the timestamp of the last edit to this sensor state.
        /// </summary>
        public DateTime EditTimestamp
        {
            get { return _editTimestamp; }
            set { _editTimestamp = value; }
        }

        /// <summary>
        /// Gets or sets the list of values this sensor state holds.
        /// </summary>
        public Dictionary<DateTime, float> Values
        {
            get { return _valueList; }
            set { _valueList = value; }
        }

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        /// <summary>
        /// Determines whether a given object is equal to this SensorState object.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Whether or not the given object, and this SensorState, are equal.</returns>
        public override bool Equals(object obj) // TODO: test this
        {
            SensorState s = null;

            if (!(obj is SensorState))
                return false;

            s = obj as SensorState;

            if (!(s.EditTimestamp == EditTimestamp))
                return false;

            if (s.Values.Count != Values.Count)
                return false;

            foreach (var f in _valueList)
            {
                if (!s.Values[f.Key].Equals(f.Value))
                    return false;
            }

            return true;
        }

        public List<DateTime> GetMissingTimes(int timeGap, DateTime start, DateTime end)
        {
            var missing = new List<DateTime>();
            for (var time = start; time <= end; time = time.AddMinutes(timeGap))
            {
                if (!Values.ContainsKey(time))
                {
                    missing.Add(time);
                }
            }
            return missing;
        }

        public List<DateTime> GetOutliers(int timeGap,DateTime start, DateTime end,float upperLimit, float lowerLimit, float maxRateChange)
        {
            var outliers = new List<DateTime>();
            var prev = 0f;
            for(var time = start;time<=end;time = time.AddMinutes(timeGap))
            {
                var value = 0f;
                if (!Values.TryGetValue(time, out value)) continue;
                if (value < lowerLimit || value > upperLimit)
                    outliers.Add(time);
                else if (time != start && Math.Abs(value - prev) > maxRateChange)
                    outliers.Add(time);
                prev = value;
            }
            return outliers;
        }

        public override string ToString()
        {
            return _editTimestamp.ToString() + " " + Values.First().Key + " " + Values.First().Value;
        }
    }
}
