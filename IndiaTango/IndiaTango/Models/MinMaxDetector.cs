﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Visiblox.Charts;

namespace IndiaTango.Models
{
    public class MinMaxDetector : IDetectionMethod
    {
        private readonly AboveMaxValueDetector _aboveMaxValue;
        private readonly BelowMinValueDetector _belowMinValue;
        private bool _showMaxMinLines;
        private Sensor _selectedSensor;

        private ComboBox _sensorsCombo;

        private Grid _settings;

        public event UpdateGraph GraphUpdateNeeded;

        public MinMaxDetector()
        {
            _aboveMaxValue = new AboveMaxValueDetector(this);
            _belowMinValue = new BelowMinValueDetector(this);
        }

        public string Name
        {
            get { return "Upper & Lower Limits"; }
        }

        public string Abbreviation
        {
            get { return "Limits"; }
        }

        public IDetectionMethod This
        {
            get { return this; }
        }

        public List<ErroneousValue> GetDetectedValues(Sensor sensorToCheck)
        {
            var detectedValues = new List<ErroneousValue>();

            foreach (var value in sensorToCheck.CurrentState.Values)
            {
                if (value.Value < sensorToCheck.LowerLimit)
                    detectedValues.Add(new ErroneousValue(value.Key, _belowMinValue, sensorToCheck));
                else if (value.Value > sensorToCheck.UpperLimit)
                    detectedValues.Add(new ErroneousValue(value.Key, _aboveMaxValue, sensorToCheck));
            }

            return detectedValues;
        }

        public bool HasSettings
        {
            get { return true; }
        }

        public Grid SettingsGrid
        {
            get
            {
                if (_settings == null)
                {
                    var wrapperGrid = new Grid();
                    var stackPanel = new StackPanel();
                    var checkBox = new CheckBox
                                       {
                                           Content = new TextBlock { Text = "Graph Upper and Lower Limits" },
                                           IsChecked = _showMaxMinLines
                                       };
                    checkBox.Checked += (o, e) =>
                                            {
                                                _showMaxMinLines = true;
                                                if (IsEnabled)
                                                    GraphUpdateNeeded();
                                            };
                    checkBox.Unchecked += (o, e) =>
                                              {
                                                  _showMaxMinLines = false;
                                                  if (IsEnabled)
                                                      GraphUpdateNeeded();
                                              };

                    stackPanel.Children.Add(checkBox);

                    var graphOptions = new StackPanel { Orientation = Orientation.Horizontal };

                    graphOptions.Children.Add(new TextBlock
                                                  {
                                                      Text = "What sensor to use when graphing lines",
                                                      Margin = new Thickness(0, 0, 10, 0)
                                                  });

                    _sensorsCombo = new ComboBox { Width = 100 };

                    _sensorsCombo.SelectionChanged += (o, e) =>
                                                          {
                                                              if (e.AddedItems.Count < 1)
                                                                  return;

                                                              if (_selectedSensor != null)
                                                                  _selectedSensor.PropertyChanged -=
                                                                      PropertyChangedInSelectedSensor;

                                                              _selectedSensor = e.AddedItems[0] as Sensor;

                                                              if (_selectedSensor != null)
                                                                  _selectedSensor.PropertyChanged +=
                                                                      PropertyChangedInSelectedSensor;


                                                              if (IsEnabled)
                                                                  GraphUpdateNeeded();
                                                          };

                    graphOptions.Children.Add(_sensorsCombo);

                    stackPanel.Children.Add(graphOptions);

                    wrapperGrid.Children.Add(stackPanel);
                    _settings = wrapperGrid;
                }
                return _settings;
            }
        }

        private void PropertyChangedInSelectedSensor(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "UpperLimit" || e.PropertyName == "LowerLimit") && IsEnabled)
                GraphUpdateNeeded();
        }

        public bool HasGraphableSeries
        {
            get { return _showMaxMinLines; }
        }

        public bool CheckIndividualValue(Sensor sensor, DateTime timeStamp)
        {
            if (sensor.CurrentState.Values.ContainsKey(timeStamp))
                return false;
            var value = sensor.CurrentState.Values[timeStamp];
            return value > sensor.UpperLimit || value < sensor.LowerLimit || Math.Abs(value - sensor.CurrentState.Values[sensor.CurrentState.FindPrevValue(timeStamp)]) > sensor.MaxRateOfChange;
        }

        public List<LineSeries> GraphableSeries(Sensor sensorToBaseOn, DateTime startDate, DateTime endDate)
        {
            return new List<LineSeries> { new LineSeries { DataSeries = new DataSeries<DateTime, float>("Upper Limit") { new DataPoint<DateTime, float>(startDate, sensorToBaseOn.UpperLimit), new DataPoint<DateTime, float>(endDate, sensorToBaseOn.UpperLimit) }, LineStroke = Brushes.OrangeRed }, new LineSeries { DataSeries = new DataSeries<DateTime, float>("Lower Limit") { new DataPoint<DateTime, float>(startDate, sensorToBaseOn.LowerLimit), new DataPoint<DateTime, float>(endDate, sensorToBaseOn.LowerLimit) }, LineStroke = Brushes.OrangeRed } };
        }

        public List<LineSeries> GraphableSeries(DateTime startDate, DateTime endDate)
        {
            return (_sensorsCombo.SelectedIndex == -1) ? new List<LineSeries>() : new List<LineSeries> { new LineSeries { DataSeries = new DataSeries<DateTime, float>("Upper Limit") { new DataPoint<DateTime, float>(startDate, _selectedSensor.UpperLimit), new DataPoint<DateTime, float>(endDate, _selectedSensor.UpperLimit) }, LineStroke = Brushes.OrangeRed }, new LineSeries { DataSeries = new DataSeries<DateTime, float>("Lower Limit") { new DataPoint<DateTime, float>(startDate, _selectedSensor.LowerLimit), new DataPoint<DateTime, float>(endDate, _selectedSensor.LowerLimit) }, LineStroke = Brushes.OrangeRed } };
        }

        public List<IDetectionMethod> Children
        {
            get { return new List<IDetectionMethod> { _aboveMaxValue, _belowMinValue }; }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public bool IsEnabled { get; set; }

        public ListBox ListBox { get; set; }

        public Sensor[] SensorOptions
        {
            set
            {
                _sensorsCombo.Items.Clear();
                foreach (var sensor in value)
                {
                    _sensorsCombo.Items.Add(sensor);
                }

                if (_sensorsCombo.Items.Count == 1)
                    _sensorsCombo.SelectedIndex = 0;
                else if (_selectedSensor != null && !_sensorsCombo.Items.Contains(_selectedSensor) && IsEnabled)
                    GraphUpdateNeeded();
            }
        }
    }

    public delegate void UpdateGraph();
}