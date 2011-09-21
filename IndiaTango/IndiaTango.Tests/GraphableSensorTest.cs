﻿using System;
using System.Linq;
using System.Windows.Media;
using IndiaTango.Models;
using NUnit.Framework;

namespace IndiaTango.Tests
{
    [TestFixture]
    class GraphableSensorTest
    {
        private GraphableSensor sensor;

        [SetUp]
        public void SetUp()
        {
            var rawSensor = new Sensor("Temperature", "Temperature at 30m", 40, 20, "C", 20, "Temperature Sensors Ltd.", "1102123", null);

            rawSensor.AddState(new SensorState(DateTime.Now));

            var baseDate = DateTime.Now;

            rawSensor.CurrentState.Values.Add(baseDate, 15);
            rawSensor.CurrentState.Values.Add(baseDate.AddMinutes(15), 20);
            rawSensor.CurrentState.Values.Add(baseDate.AddMinutes(30), 25);

            sensor = new GraphableSensor(rawSensor);
        }
        
        [Test]
        public void ColourTesting()
        {
            Assert.IsNotNull(sensor.Colour);

            sensor.Colour = Colors.Black;

            Assert.AreEqual(sensor.Colour, Colors.Black);
        }

        [Test]
        public void GetSensor()
        {
            Assert.IsNotNull(sensor.Sensor);
        }

        [Test]
        public void DataPointsTest()
        {
            var dataPoints = sensor.DataPoints.ToArray();
            Assert.AreEqual(dataPoints[0].Y, 15);
            Assert.AreEqual(dataPoints[1].Y, 20);
            Assert.AreEqual(dataPoints[2].Y, 25);
        }
    }
}
