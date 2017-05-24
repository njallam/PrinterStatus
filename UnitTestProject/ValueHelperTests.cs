using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Printer_Status.Helpers.Tests
{
    [TestClass]
    public class ValueHelperTests
    {
        [TestMethod]
        public void TryIPAddressTest()
        {
            IPAddress tempIP;

            //Valid IP addresses
            Assert.IsTrue(ValueHelper.TryIPAddress("172.20.1.6", out tempIP), "Valid IP Address should be determined valid");

            //Boundary IP addresses
            Assert.IsTrue(ValueHelper.TryIPAddress("0.0.0.0", out tempIP), "Boundary IP Address should be valid.");
            Assert.IsTrue(ValueHelper.TryIPAddress("255.255.255.255", out tempIP), "Boundary IP Address should be determined valid.");

            Assert.IsFalse(ValueHelper.TryIPAddress("-1.0.0.0", out tempIP), "Boundary IP Address should be determined invalid.");
            Assert.IsFalse(ValueHelper.TryIPAddress("0.0.256.20", out tempIP), "Boundary IP Address should be determined invalid.");

            //Extreme IP addresses
            Assert.IsFalse(ValueHelper.TryIPAddress("hamster", out tempIP), "Extreme IP Address should be determined invalid.");

            //Otherwise invalid IP addresses
            Assert.IsFalse(ValueHelper.TryIPAddress("2961104948", out tempIP), "IP Address in wrong format should be determined invalid.");
            Assert.IsFalse(ValueHelper.TryIPAddress("10110000.01111110.11100000.00110100", out tempIP), "IP Address in wrong format should be determined invalid.");
        }

        [TestMethod]
        public void LevelToPercentTest()
        {
            //Level special values
            Assert.AreEqual("other", ValueHelper.LevelToPercent(50, -1));
            Assert.AreEqual("unknown", ValueHelper.LevelToPercent(65, -2));
            Assert.AreEqual("OK", ValueHelper.LevelToPercent(23, -3));

            Assert.AreEqual("unknown", ValueHelper.LevelToPercent(50, -10));

            //Max capacity special values
            Assert.AreEqual("50", ValueHelper.LevelToPercent(0, 50));
            Assert.AreEqual("71", ValueHelper.LevelToPercent(-12, 71));

            //Percentage calculations
            Assert.AreEqual("50%", ValueHelper.LevelToPercent(100, 50));
            Assert.AreEqual("1%", ValueHelper.LevelToPercent(100, 1));
            Assert.AreEqual("0%", ValueHelper.LevelToPercent(100, 0));

            Assert.AreEqual("10%", ValueHelper.LevelToPercent(99, 10));
        }

        [TestMethod]
        public void IsLowTest()
        {
            Assert.IsFalse(ValueHelper.IsLow(-1, 115), "Should return false as indeterminable");
            Assert.IsFalse(ValueHelper.IsLow(55, -11), "Should return false as indeterminable");
            Assert.IsFalse(ValueHelper.IsLow(-1, -7), "Should return false as indeterminable");

            Assert.IsFalse(ValueHelper.IsLow(1,1), "Should return false as full");
            Assert.IsTrue(ValueHelper.IsLow(1, 0), "Should return true as empty");

            Assert.IsTrue(ValueHelper.IsLow(10,1), "Should return true as equal to LowValuePercent");
            Assert.IsTrue(ValueHelper.IsLow(11, 1), "Should return true as less than LowValuePercent");
            Assert.IsFalse(ValueHelper.IsLow(9, 1), "Should return false as greater than LowValuePercent");
        }
    }
}