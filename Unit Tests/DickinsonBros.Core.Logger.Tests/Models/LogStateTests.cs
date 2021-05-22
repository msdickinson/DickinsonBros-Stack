using DickinsonBros.Core.Logger.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace DickinsonBros.Core.Logger.Tests.Models
{
    [TestClass]
    public class LogStateTests
    {
        [TestMethod]
        public void Constructor_Runs_KeyValuePairHasValue()
        {
            //Setup
            var propertiesRedacted = new List<KeyValuePair<string, object>>();

            var uut = new LogState(propertiesRedacted);

            //Act
            uut.GetEnumerator();

            //Assert
            Assert.AreEqual(propertiesRedacted, uut._keyValuePairs);
        }

        [TestMethod]
        public void GetEnumerator_Runs_ReturnsEnumerator()
        {
            //Setup
            var keyExpected = "C";
            var valueExpected = "D";

            var propertiesRedacted = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("A", "B"),
                new KeyValuePair<string, object>(keyExpected, valueExpected)
            };

            var uut = new LogState(propertiesRedacted);

            //Act
            var observedEnumerator = uut.GetEnumerator();
            observedEnumerator.MoveNext();
            observedEnumerator.MoveNext();
            var observedItem = observedEnumerator.Current;

            //Assert
            Assert.AreEqual(propertiesRedacted, uut._keyValuePairs);

            //Assert
            Assert.IsNotNull(observedEnumerator);
            Assert.AreEqual(keyExpected, observedItem.Key);
            Assert.AreEqual(valueExpected, observedItem.Value);
        }

        [TestMethod]
        public void GetEnumerator_PastRange_ReturnsLastItem()
        {
            //Setup
            var keyExpected = "C";
            var valueExpected = "D";

            var propertiesRedacted = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("A", "B"),
                new KeyValuePair<string, object>(keyExpected, valueExpected)
            };

            var uut = new LogState(propertiesRedacted);

            //Act
            var observedEnumerator = uut.GetEnumerator();
            observedEnumerator.MoveNext();
            observedEnumerator.MoveNext();
            observedEnumerator.MoveNext();

            var observedItem = observedEnumerator.Current;

            //Assert
            Assert.AreEqual(keyExpected, observedItem.Key);
            Assert.AreEqual(valueExpected, observedItem.Value);

        }


        [TestMethod]
        public void GetEnumerator_Runs_ReturnsArray()
        {
            //Setup
            var keyExpected = "C";
            var valueExpected = "D";

            var propertiesRedacted = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("A", "B"),
                new KeyValuePair<string, object>(keyExpected, valueExpected)
            };

            var uut = new LogState(propertiesRedacted);

            //Act
            var observedEnumerator = ((IEnumerable)uut).GetEnumerator();

            //Assert
            Assert.IsNotNull(observedEnumerator); ;
        }


        [TestMethod]
        public void KeyValuePair_Runs_ReturnsEnumerator()
        {
            //Setup
            var keyExpected = "A";
            var valueExpected = "B";

            var propertiesRedacted = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(keyExpected, valueExpected)
            };

            var uut = new LogState(propertiesRedacted);

            //Act
            var observed = uut[0];

            //Assert
            Assert.IsNotNull(observed);
            Assert.AreEqual(keyExpected, observed.Key);
            Assert.AreEqual(valueExpected, observed.Value);
        }



        [TestMethod]
        public void Count_KeyValuePairsIsNull_ReturnsZero()
        {
            //Setup
            var uut = new LogState(null);

            //Act
            var observed = uut.Count;

            //Assert
            Assert.AreEqual(0, observed);
        }


        [TestMethod]
        public void Count_KeyValuePairsHasItems_ReturnsCount()
        {
            //Setup
            var propertiesRedacted = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>()
            };

            var uut = new LogState(propertiesRedacted);

            //Act
            var observed = uut.Count;

            //Assert
            Assert.AreEqual(1, observed);
        }
    }
}
