﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndiaTango.Models
{
    public class Dataset
    {
        private Buoy _buoy;
        private readonly DateTime _startTimeStamp;
        private readonly DateTime _endTimeStamp;
        private List<Sensor> _sensors;

        /// <summary>
        /// Creates a new dataset
        /// </summary>
        /// <param name="buoy">The buoy that the dataset came from</param>
        /// <param name="startTimeStamp">The start time of the dataset</param>
        /// <param name="endTimeStamp">The end time of the dataset</param>
        public Dataset(Buoy buoy, DateTime startTimeStamp, DateTime endTimeStamp)
        {
            if (buoy == null)
                throw new ArgumentException("Please provide a buoy this dataset came from");
            if (DateTime.Compare(startTimeStamp, endTimeStamp) >= 0)
                throw new ArgumentException("End time stamp must be after start");
            _buoy = buoy;
            _startTimeStamp = startTimeStamp;
            _endTimeStamp = endTimeStamp;
            _sensors = new List<Sensor>();
        }

        /// <summary>
        /// Gets and sets the buoy that this dataset came from
        /// </summary>
        public Buoy Buoy
        {
            get { return _buoy; }
            set
            {
                if (value == null)
                    throw new FormatException("Buoy must not be null");
                _buoy = value;
            }
        }

        /// <summary>
        /// Returns the start time stamp for this dataset
        /// </summary>
        public DateTime StartTimeStamp
        {
            get { return _startTimeStamp; }
        }

        /// <summary>
        /// Returns the end time stamp for this dataset
        /// </summary>
        public DateTime EndTimeStamp
        {
            get { return _endTimeStamp; }
        }

        /// <summary>
        /// Returns the list of sensors for this dataset
        /// </summary>
        public List<Sensor> Sensors
        {
        	get { return _sensors; }
        	set { _sensors = value; }
        }

    	/// <summary>
        /// Adds a sensor to the list of sensors
        /// </summary>
        /// <param name="sensor">The sensor to be added to the list of sensors</param>
        public void AddSensor(Sensor sensor)
        {
            if (sensor == null)
                throw new ArgumentException("Sensor cannot be null");
            _sensors.Add(sensor);
        }

        public int DataPointCount
        {
            get
            { 
                var diff = EndTimeStamp.Subtract(StartTimeStamp);
                return (int)Math.Floor(diff.TotalMinutes/15);
            }
        }

        public object GetMissingTimes(int timeDiff)
        {
            var missing = new List<DataValue>();
            foreach (var sensor in Sensors)
            {
                for (int i = 1;i<sensor.CurrentState.Values.Count;i++)
                {
                    if(sensor.CurrentState.Values[i-1].Timestamp.AddMinutes(timeDiff)!=sensor.CurrentState.Values[i].Timestamp)
                    {
                        var tmpTime = sensor.CurrentState.Values[i - 1].Timestamp.AddMinutes(timeDiff);
                        while(tmpTime < sensor.CurrentState.Values[i].Timestamp)
                        {
                            missing.Add(new DataValue(tmpTime,0));
                            tmpTime = tmpTime.AddMinutes(timeDiff);
                        }
                    }
                        
                }
            }
            return missing;
        }
    }
}