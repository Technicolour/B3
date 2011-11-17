﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace IndiaTango.Models
{
    public enum SummaryType { Average = 0, Sum = 1 }
    /// <summary>
    /// Represents a Sensor, which resembles a sensor attached to a buoy, measuring a given water quality parameter.
    /// </summary>
    [Serializable]
    [DataContract]
    public class Sensor : INotifyPropertyChanged
    {
        #region Private Members
        private Stack<SensorState> _undoStack;
        private Stack<SensorState> _redoStack;
        private List<DateTime> _calibrationDates;
        private string _name;
        private string _description;
        private float _depth;
        private string _unit;
        private float _maxRateOfChange;
        private string _manufacturer;
        private int _errorThreshold;
        private string _serialNumber;
        private Colour _colour;
        private float _lowerLimit;
        private float _upperLimit;
        private SummaryType _summaryType;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new sensor, with the specified sensor name and measurement unit.
        /// </summary>
        /// <param name="name">The name of the sensor.</param>
        /// <param name="unit">The unit used to report values given by this sensor.</param>
        public Sensor(string name, string unit) : this(name, "", 100, 0, unit, 0, "", "", null) { }


        /// <summary>
        /// Creates a new sensor, with the specified sensor name and measurement unit.
        /// </summary>
        /// <param name="name">The name of the sensor.</param>
        /// <param name="unit">The unit used to report values given by this sensor.</param>
        /// <param name="owner">The owner of the sensor</param>
        public Sensor(string name, string unit, Dataset owner) : this(name, "", 100, 0, unit, 0, "", "", owner) { }

        /// <summary>
        /// Creates a new sensor, using default values for Undo/Redo stacks, calibration dates, error threshold and a failure-indicating value.
        /// </summary>
        /// <param name="name">The name of the sensor.</param>
        /// <param name="description">A description of the sensor's function or purpose.</param>
        /// <param name="upperLimit">The upper limit for values reported by this sensor.</param>
        /// <param name="lowerLimit">The lower limit for values reported by this sensor.</param>
        /// <param name="unit">The unit used to report values given by this sensor.</param>
        /// <param name="maxRateOfChange">The maximum rate of change allowed by this sensor.</param>
        /// <param name="manufacturer">The manufacturer of this sensor.</param>
        /// <param name="serial">The serial number associated with this sensor.</param>
        /// <param name="owner">The dataset owner of the sensor</param>
        public Sensor(string name, string description, float upperLimit, float lowerLimit, string unit, float maxRateOfChange, string manufacturer, string serial, Dataset owner) : this(name, description, upperLimit, lowerLimit, unit, maxRateOfChange, manufacturer, serial, new Stack<SensorState>(), new Stack<SensorState>(), new List<DateTime>(), owner) { }

        /// <summary>
        /// Creates a new sensor, using default values for error threshold and failure-indicating value. 
        /// </summary>
        /// <param name="name">The name of the sensor.</param>
        /// <param name="description">A description of the sensor's function or purpose.</param>
        /// <param name="upperLimit">The upper limit for values reported by this sensor.</param>
        /// <param name="lowerLimit">The lower limit for values reported by this sensor.</param>
        /// <param name="unit">The unit used to report values given by this sensor.</param>
        /// <param name="maxRateOfChange">The maximum rate of change allowed by this sensor.</param>
        /// <param name="manufacturer">The manufacturer of this sensor.</param>
        /// <param name="serial">The serial number associated with this sensor.</param>
        /// <param name="undoStack">A stack containing previous sensor states.</param>
        /// <param name="redoStack">A stack containing sensor states created after the modifications of the current state.</param>
        /// <param name="calibrationDates">A list of dates, on which calibration was performed.</param>
        /// <param name="owner">The dataset owner of the sensor</param>
        public Sensor(string name, string description, float upperLimit, float lowerLimit, string unit, float maxRateOfChange, string manufacturer, string serial, Stack<SensorState> undoStack, Stack<SensorState> redoStack, List<DateTime> calibrationDates, Dataset owner) : this(name, description, upperLimit, lowerLimit, unit, maxRateOfChange, manufacturer, serial, undoStack, redoStack, calibrationDates, Properties.Settings.Default.DefaultErrorThreshold, owner) { }

        /// <summary>
        /// Creates a new sensor.
        /// </summary>
        /// <param name="name">The name of the sensor.</param>
        /// <param name="description">A description of the sensor's function or purpose.</param>
        /// <param name="upperLimit">The upper limit for values reported by this sensor.</param>
        /// <param name="lowerLimit">The lower limit for values reported by this sensor.</param>
        /// <param name="unit">The unit used to report values given by this sensor.</param>
        /// <param name="maxRateOfChange">The maximum rate of change allowed by this sensor.</param>
        /// <param name="manufacturer">The manufacturer of this sensor.</param>
        /// <param name="serial">The serial number associated with this sensor.</param>
        /// <param name="undoStack">A stack containing previous sensor states.</param>
        /// <param name="redoStack">A stack containing sensor states created after the modifications of the current state.</param>
        /// <param name="calibrationDates">A list of dates, on which calibration was performed.</param>
        /// <param name="errorThreshold">The number of times a failure-indicating value can occur before this sensor is flagged as failing.</param>
        /// <param name="owner">The dataset that owns the sensor</param>
        public Sensor(string name, string description, float upperLimit, float lowerLimit, string unit, float maxRateOfChange, string manufacturer, string serial, Stack<SensorState> undoStack, Stack<SensorState> redoStack, List<DateTime> calibrationDates, int errorThreshold, Dataset owner) : this(name, description, upperLimit, lowerLimit, unit, maxRateOfChange, manufacturer, serial, undoStack, redoStack, calibrationDates, errorThreshold, owner, SummaryType.Average) { }

        /// <summary>
        /// Creates a new sensor.
        /// </summary>
        /// <param name="name">The name of the sensor.</param>
        /// <param name="description">A description of the sensor's function or purpose.</param>
        /// <param name="upperLimit">The upper limit for values reported by this sensor.</param>
        /// <param name="lowerLimit">The lower limit for values reported by this sensor.</param>
        /// <param name="unit">The unit used to report values given by this sensor.</param>
        /// <param name="maxRateOfChange">The maximum rate of change allowed by this sensor.</param>
        /// <param name="manufacturer">The manufacturer of this sensor.</param>
        /// <param name="serial">The serial number associated with this sensor.</param>
        /// <param name="undoStack">A stack containing previous sensor states.</param>
        /// <param name="redoStack">A stack containing sensor states created after the modifications of the current state.</param>
        /// <param name="calibrationDates">A list of dates, on which calibration was performed.</param>
        /// <param name="errorThreshold">The number of times a failure-indicating value can occur before this sensor is flagged as failing.</param>
        /// <param name="owner">The dataset that owns the sensor</param>
        /// <param name="sType">Indicates whether the sensor's values should be averaged or summed when summarised</param>
        public Sensor(string name, string description, float upperLimit, float lowerLimit, string unit, float maxRateOfChange, string manufacturer, string serial, Stack<SensorState> undoStack, Stack<SensorState> redoStack, List<DateTime> calibrationDates, int errorThreshold, Dataset owner, SummaryType sType)
        {
            if (name == "")
                throw new ArgumentNullException("Name");

            if (unit == "")
                throw new ArgumentNullException("Unit");

            if (serial == null)
                throw new ArgumentNullException("Serial");

            if (calibrationDates == null)
                throw new ArgumentNullException("CalibrationDates");

            if (undoStack == null)
                throw new ArgumentNullException("UndoStack");

            if (redoStack == null)
                throw new ArgumentNullException("RedoStack");

            if (upperLimit <= lowerLimit)
                throw new ArgumentOutOfRangeException("UpperLimit");

            _name = name;
            RawName = name;
            Description = description;
            UpperLimit = upperLimit;
            LowerLimit = lowerLimit;
            _unit = unit;
            MaxRateOfChange = maxRateOfChange;
            Manufacturer = manufacturer;
            _undoStack = undoStack;
            _redoStack = redoStack;
            _calibrationDates = calibrationDates;
            SerialNumber = serial;

            ErrorThreshold = errorThreshold;
            Owner = owner;
            _summaryType = sType;

            Colour = Color.FromRgb((byte)(Common.Generator.Next()), (byte)(Common.Generator.Next()), (byte)(Common.Generator.Next()));
        }

        #endregion

        #region Properties
        public ReadOnlyCollection<SensorState> UndoStates
        {
            get
            {
                // Return a stack that cannot be modified externally
                // Since it going to be iterated over in order anyway (and there'll only be approx. 5 times at any one time)...
                return _undoStack.ToList().AsReadOnly();
            }
        }

        public ReadOnlyCollection<SensorState> RedoStates
        {
            get
            {
                // Return a stack that cannot be modified externally
                // Since it going to be iterated over in order anyway (and there'll only be approx. 5 times at any one time)...
                return _redoStack.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the undo stack for this sensor. The undo stack contains a list of previous states this sensor was in before its current state. This stack cannot be null.
        /// </summary>
        [DataMember]
        private Stack<SensorState> UndoStack
        {
            get { return _undoStack; }
            set
            {
                if (value == null)
                    throw new FormatException("The undo stack cannot be null.");

                _undoStack = value;
            }
        }

        /// <summary>
        /// Gets the redo stack for this sensor. The redo stack contains a list of previous states this sensor can be in after the current state. This stack cannot be null.
        /// </summary>
        [DataMember]
        private Stack<SensorState> RedoStack
        {
            get { return _redoStack; }
            set
            {
                if (value == null)
                    throw new FormatException("The redo stack cannot be null.");

                _redoStack = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of dates this sensor was calibrated on. The list of calibration dates cannot be null.
        /// </summary>
        [DataMember]
        public List<DateTime> CalibrationDates
        {
            get { return _calibrationDates; }
            set
            {
                if (value == null)
                    throw new FormatException("The list of calibration dates cannot be null.");

                _calibrationDates = value;
            }
        }

        /// <summary>
        /// Gets the first name this sensor was given (i.e. the name used to import)
        /// </summary>
        [DataMember]
        public string RawName { get; private set; }

        /// <summary>
        /// Gets or sets the name of this sensor. The sensor name cannot be null.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != "")
                    _name = value;
                FirePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets the description of this sensor's purpose or function.
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                FirePropertyChanged("Description");
            }
        }

        /// <summary>
        /// Gets or sets the depth of the sensor
        /// </summary>
        [DataMember]
        public float Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
                FirePropertyChanged("Depth");
            }
        }

        /// <summary>
        /// Gets or sets the lower limit for values reported by this sensor.
        /// </summary>
        [DataMember]
        public float LowerLimit
        {
            get { return _lowerLimit; }
            set
            {
                if (value > UpperLimit)
                {
                    _lowerLimit = UpperLimit;
                    UpperLimit = value;
                    EventLogger.LogSensorInfo(Owner, Name, "Swapping Upper and Lower Limit as Lower Limit > Upper Limit");
                }
                else
                    _lowerLimit = value;

                FirePropertyChanged("LowerLimit");
            }
        }

        /// <summary>
        /// Gets or sets the upper limit for values reported by this sensor.
        /// </summary>
        [DataMember]
        public float UpperLimit
        {
            get { return _upperLimit; }
            set
            {
                if (value < LowerLimit)
                {
                    _upperLimit = LowerLimit;
                    LowerLimit = value;
                    EventLogger.LogSensorInfo(Owner, Name, "Swapping Upper and Lower Limit as Upper Limit < Lower Limit");
                }
                else
                    _upperLimit = value;
                FirePropertyChanged("UpperLimit");
            }
        }

        /// <summary>
        /// Gets or sets the measurement unit reported with values collected by this sensor.
        /// </summary>
        [DataMember]
        public string Unit
        {
            get { return _unit; }
            set
            {
                if (value != "")
                    _unit = value;
                FirePropertyChanged("Unit");
            }
        }

        /// <summary>
        /// Gets or sets the maximum rate of change allowed for values reported by this sensor.
        /// </summary>
        [DataMember]
        public float MaxRateOfChange
        {
            get
            {
                return _maxRateOfChange;
            }
            set
            {
                _maxRateOfChange = value;
                FirePropertyChanged("MaxRateOfChange");
            }
        }

        /// <summary>
        /// Gets or sets the name of the manufacturer of this sensor.
        /// </summary>
        [DataMember]
        public string Manufacturer
        {
            get { return _manufacturer; }

            set
            {
                _manufacturer = value;
                FirePropertyChanged("Manufacturer");
            }
        }

        /// <summary>
        /// Gets or sets the serial number of this sensor
        /// </summary>
        [DataMember]
        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(string.Format("{0} cannot be null", "Serial Number"));

                _serialNumber = value;
                FirePropertyChanged("SerialNumber");
            }
        }

        public SensorState CurrentState
        {
            get { return (UndoStack.Count != 0) ? UndoStack.Peek() : RawData; }
        }

        /// <summary>
        /// The colour to use when graphing the sensor
        /// </summary>
        [DataMember]
        public Colour Colour
        {
            get { return _colour; }
            set
            {
                _colour = value;
                FirePropertyChanged("Colour");
            }
        }

        [DataMember]
        public int ErrorThreshold
        {
            get { return _errorThreshold; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Error threshold for any given sensor must be at least 1.");

                _errorThreshold = value;
            }
        }

        public Dataset Owner { get; private set; }

        private SensorState _rawData;

        public SensorState RawData
        {
            get { return _rawData ?? (_rawData = new SensorState(this, DateTime.Now, new Dictionary<DateTime, float>(), "", true, null)); }
        }

        /// <summary>
        /// Gets a value indicating whether or not this sensor shows signs of physical failure.
        /// </summary>
        /// <param name="dataset">The dataset, containing information about the data interval for this sensor.</param>
        /// <returns>A value indicating whether or not this sensor is failing.</returns>
        public bool IsFailing(Dataset dataset)
        {
            if (Properties.Settings.Default.IgnoreSensorErrorDetection)
                return false;

            if (dataset == null)
                throw new NullReferenceException("You must provide a non-null dataset.");

            if (CurrentState == null)
                throw new NullReferenceException("No active sensor state exists for this sensor, so you can't detect whether it is failing or not.");

            if (CurrentState.Values.Count == 0)
                return false;

            var baseTime = CurrentState.Values.ElementAt(0).Key;
            var incidence = 0;
            var time = 0;

            for (int i = 0; i < dataset.ExpectedDataPointCount; i++)
            {
                var key = baseTime.AddMinutes(time);

                if (CurrentState.Values.ContainsKey(key))
                    incidence = 0;
                else
                    incidence++;

                if (incidence == ErrorThreshold)
                    return true;

                time += dataset.DataInterval;
            }

            return false;
        }

        public SummaryType SummaryType
        {
            get { return _summaryType; }
            set
            {
                _summaryType = value;
                FirePropertyChanged("SummaryType");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Revert data values for this sensor to the previous state held.
        /// </summary>
        public void Undo()
        {
            // This is because the undo stack has to have at least one item on it - the current state
            if (UndoStack.Count < 1)
                throw new InvalidOperationException("Undo is not possible at this stage. There are no more possible states to undo to.");

            RedoStack.Push(UndoStack.Pop());
        }

        /// <summary>
        /// Advance data values for this sensor to the next state stored.
        /// </summary>
        public void Redo()
        {
            if (RedoStack.Count == 0)
                throw new InvalidOperationException("Redo is not possible at this stage. There are no more possible states to redo to.");

            UndoStack.Push(RedoStack.Pop());
        }

        /// <summary>
        /// Update the active state for data values, clearing the redo stack in the process.
        /// </summary>
        /// <param name="newState"></param>
        public void AddState(SensorState newState)
        {
            if (UndoStack.Count == 5)
            {
                // Remove from the bottom, so reverse and pop, then reverse again
                var reverse = new Stack<SensorState>();

                while (_undoStack.Count > 0)
                    reverse.Push(_undoStack.Pop());

                reverse.Pop();

                while (reverse.Count > 0)
                    UndoStack.Push(reverse.Pop());
            }

            UndoStack.Push(newState);
            RedoStack.Clear();
        }

        public void RevertToRaw()
        {
            while (UndoStack.Count > 0)
                RedoStack.Push(UndoStack.Pop());

            UndoStack.Clear();
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion

        /// <summary>
        /// Undoes this sensor's values so that the state represented by the timestamp specified is the current state (i.e. top of the Undo stack). If the timestamp does not exist, does nothing.
        /// </summary>
        /// <param name="dateTime">The timestamp representing the state to make the current state.</param>
        public void Undo(DateTime dateTime)
        {
            var sensor = (from selectedSensor in UndoStack where selectedSensor.EditTimestamp == dateTime select selectedSensor).DefaultIfEmpty(null).FirstOrDefault();

            if (sensor != null)
            {
                // Exists
                while (UndoStack.Count > 0)
                {
                    // Keep undoing until at desired state
                    if (UndoStack.Peek() != sensor)
                        Undo();
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// Advances this sensor's values so that the state represented by the timestamp specified is the current state (i.e. top of the Undo stack). If the timestamp does not exist, does nothing.
        /// </summary>
        /// <param name="dateTime">The timestamp representing the state to make the current state.</param>
        public void Redo(DateTime dateTime)
        {
            var sensor = (from selectedSensor in RedoStack where selectedSensor.EditTimestamp == dateTime select selectedSensor).DefaultIfEmpty(null).FirstOrDefault();

            if (sensor != null)
            {
                Redo();

                // Exists
                while (RedoStack.Count > 0)
                {
                    // Keep undoing until at desired state
                    if (UndoStack.Peek() != sensor)
                        Redo();
                    else
                        break;
                }
            }
        }

        public string GuessConventionalNameForSensor()
        {
            // We will guess the name of the sensor, following convention,
            // by getting the first two words (even if each word is only a single
            // capital letter, and return them
            // The experts can then specify the depth
            var r = new Regex("(([A-Z])[a-z]*)");

            var mc = r.Matches(Name);

            if (mc.Count > 0)
            {
                if (mc.Count == 1)
                    return mc[0].Captures[0].Value;
                return mc[0].Captures[0].Value + mc[1].Captures[0].Value;
            }

            return Name;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChanged(string propertyThatChanged)
        {
            if (PropertyChanged != null && !string.IsNullOrWhiteSpace(propertyThatChanged))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyThatChanged));
            }
        }
    }
}
