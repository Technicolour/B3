﻿using System;
using System.ComponentModel;
using ProtoBuf;

namespace IndiaTango.Models
{
    [ProtoContract]
    public class SensorMetaData : INotifyPropertyChanged
    {
        #region Constructors
        /// <summary>
        /// Protobuf constructor
        /// </summary>
        public SensorMetaData() { }

        /// <summary>
        /// Creates a new sensor metadata
        /// </summary>
        /// <param name="serialNumber">The serial number of the meta sensor</param>
        /// <param name="manufacturer">The meta sensor's manufacturer</param>
        /// <param name="dateOfInstallation">The date the meta sensor was installed</param>
        /// <param name="accuracy">The accuracy of the meta sensor's readings</param>
        public SensorMetaData(string serialNumber, string manufacturer, DateTime dateOfInstallation, float accuracy)
        {
            SerialNumber = serialNumber;
            Manufacturer = manufacturer;
            DateOfInstallation = dateOfInstallation;
            Accuracy = accuracy;
        }

        /// <summary>
        /// Creates a new sensor with the given serial number
        /// </summary>
        /// <param name="serialNumber">The serial number of the meta sensor</param>
        public SensorMetaData(string serialNumber) : this(serialNumber, "", DateTime.Now, 0) { }
        #endregion

        #region Private Variables
        [ProtoMember(1)]
        private float _accuracy;
        [ProtoMember(2)]
        private DateTime _dateOfInstallation;
        [ProtoMember(3)]
        private string _serialNumber;
        [ProtoMember(4)]
        private string _manufacturer;
        #endregion

        #region Public Properties
        public float Accuracy
        {
            get { return _accuracy; }
            set
            {
                _accuracy = value;
                FirePropertyChanged("Accuracy");
            }
        }

        public DateTime DateOfInstallation
        {
            get { return _dateOfInstallation; }
            set
            {
                _dateOfInstallation = value;
                FirePropertyChanged("DateOfInstallation");
            }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                FirePropertyChanged("SerialNumber");
            }
        }

        public string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                _manufacturer = value;
                FirePropertyChanged("Manufacturer");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(SensorMetaData) && GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return (string.Format("{0}{1}{2}{3}", Accuracy, Manufacturer, DateOfInstallation, SerialNumber)).GetHashCode();
        }

        public override string ToString()
        {
            return SerialNumber;
        }
    }
}
