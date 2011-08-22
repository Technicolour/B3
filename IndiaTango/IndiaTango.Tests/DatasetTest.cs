﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using IndiaTango.Models;

namespace IndiaTango.Tests
{
    [TestFixture]
    class DatasetTest
    {
        private Dataset _ds;
        private Buoy _b;
        private DateTime _startTime;
        private DateTime _endTime;

        [SetUp]
        public void Setup()
        {
            _startTime = new DateTime(46, 4, 12, 6, 48, 20);
            _endTime = new DateTime(84, 2, 8, 2, 4, 20);
            _b = new Buoy(1, "dsf", "asdf", new Contact("asdf", "asdf", "adsf@sdfg.com", "uerh", "sadf"), new Contact("asdf", "asdf", "adsf@sdfg.com", "uerh", "sadf"), null, new GPSCoords(32, 5));
            _ds = new Dataset(_b, _startTime, _endTime);
        }
        [Test]
        public void BuoyGetSetTest()
        {
            Assert.AreEqual(_b, _ds.Buoy);
            _b = new Buoy(2, "dsf", "asdf", new Contact("asdf", "asdf", "adsf@sdfg.com", "uerh", "sadf"), new Contact("asdf", "asdf", "adsf@sdfg.com", "uerh", "sadf"), null, new GPSCoords(32, 5));
            _ds.Buoy = _b;
            Assert.AreEqual(_b, _ds.Buoy);
        }

        [Test]
        public void StartTimeStampGetTest()
        {
            Assert.AreEqual(_startTime, _ds.StartTimeStamp);
        }

        [Test]
        public void EndTimeStampGetTest()
        {
            Assert.AreEqual(_endTime, _ds.EndTimeStamp);
        }

        [Test]
        public void SensorListTest()
        {
            var s1 = new Sensor("temp", "deg");
            var s2 = new Sensor("DO", "%");
            var sensors = new List<Sensor> { s1, s2 };
            _ds.AddSensor(s1);
            _ds.AddSensor(s2);
            Assert.AreEqual(sensors, _ds.Sensors);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NullBuoyConstructorTest()
        {
            new Dataset(null, _startTime, _endTime);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNullSensorTest()
        {
            _ds.AddSensor(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void EndTimeBeforeStartTimeTest()
        {
            new Dataset(_b, _endTime, _startTime);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void EndTimeSameAsStartTest()
        {
            new Dataset(_b, _startTime, _startTime);
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void NullBuoyPropertyTest()
        {
            _ds.Buoy = null;
        }

        [Test]
        public void FindMissingValuesTest()
        {
            var missingDates = new List<DataValue>
                                   {
                                       new DataValue(new DateTime(2011, 8, 20, 0, 15, 0), 0),
                                       new DataValue(new DateTime(2011, 8, 20, 0, 30, 0), 0),
                                       new DataValue(new DateTime(2011, 8, 20, 0, 45, 0), 0),
                                       new DataValue(new DateTime(2011, 8, 20, 1, 0, 0), 0),
                                       new DataValue(new DateTime(2011, 8, 20, 1, 15, 0), 0),
                                       new DataValue(new DateTime(2011, 8, 20, 1, 30, 0), 0),
                                       new DataValue(new DateTime(2011, 8, 20, 1, 45, 0), 0)
                                   };
            var ds = new Dataset(_b, new DateTime(2011, 8, 20, 0, 0, 0), new DateTime(2011, 8, 20, 2, 0, 0));
            var sensor = new Sensor("sensor", "ml");
            var sensorState = new SensorState(new DateTime(2011, 8, 23, 0, 0, 0));
            sensorState.Values = new List<DataValue>
                                     {
                                         new DataValue(new DateTime(2011, 8, 20, 0, 0, 0), 100),
                                         new DataValue(new DateTime(2011, 8, 20, 2, 0, 0), 50)
                                     };
            sensor.AddState(sensorState);
            ds.AddSensor(sensor);
            Assert.AreEqual(missingDates,ds.GetMissingTimes(15));
        }
    }
}
