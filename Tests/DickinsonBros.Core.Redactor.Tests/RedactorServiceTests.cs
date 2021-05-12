using DickinsonBros.Core.Redactor.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DickinsonBros.Core.Redactor.Tests
{
    public class TestObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [TestClass]
    public class RedactorServiceTests
    {
        [TestMethod]
        public void Constructor_NullRedactOptions_EmptyRedactOptionsSet()
        {
            //Setup
            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = null,
                RegexValuesToRedact = null,
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            //Act
            var uut = new RedactorService(options);

            //Assert
            Assert.IsNotNull(uut._propertiesToRedact);
            Assert.IsNotNull(uut._propertiesToRedact);
        }

        [TestMethod]
        public void RedactObject_InputPassedAsInObjectButIsAString_ReturnsRedactedMessage()
        {
            //Setup
            var input = (object)@"Bearer 1000";
            var expectedRedacted = RedactorService.REDACTED_REPLACEMENT_VALUE;

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            Assert.AreEqual(expectedRedacted, observed);
        }

        [TestMethod]
        public void RedactObject_InputPassedAsInObject_ReturnsRedactedMessage()
        {
            //Setup
            var usernameExpected = "username";
            var passwordExpected = RedactorService.REDACTED_REPLACEMENT_VALUE;

            var input = new TestObject
            {
                Username = usernameExpected,
                Password = "password"
            };

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            var objObserved = System.Text.Json.JsonSerializer.Deserialize<TestObject>(observed);

            Assert.AreEqual(usernameExpected, objObserved.Username);
            Assert.AreEqual(passwordExpected, objObserved.Password);
        }

        [TestMethod]
        public void RedactString_InputIsJsonWithRedactingPrams_ReturnsRedactedMessage()
        {
            //Setup
            var usernameExpected = "username";
            var passwordExpected = RedactorService.REDACTED_REPLACEMENT_VALUE;

            var input = @"{
                          ""Username"": ""username"",
                          ""Password"": ""password""
                        }";

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            var objObserved = System.Text.Json.JsonSerializer.Deserialize<TestObject>(observed);

            Assert.AreEqual(usernameExpected, objObserved.Username);
            Assert.AreEqual(passwordExpected, objObserved.Password);
        }

        [TestMethod]
        public void RedactString_InputIsJson_ReturnsRedactedMessage()
        {
            //Setup
            var usernameExpected = "username";

            var input = @"{
                          ""Username"": ""username""
                        }";

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            var objObserved = System.Text.Json.JsonSerializer.Deserialize<TestObject>(observed);

            Assert.AreEqual(usernameExpected, objObserved.Username);
            Assert.IsNull(objObserved.Password);
        }

        [TestMethod]
        public void RedactString_InputIsNotJsonAndHasARedactedValue_ReturnsRedactedMessage()
        {
            //Setup
            var input = @"Bearer 1000";
            var expectedRedacted = RedactorService.REDACTED_REPLACEMENT_VALUE;

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            Assert.AreEqual(expectedRedacted, observed);
        }

        [TestMethod]
        public void RedactString_InputIsNotJsonAndDoesNotHaveARedactedValue_ReturnsRedactedMessage()
        {
            //Setup
            var inputExpected = @"DemoValue";

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { },
                RegexValuesToRedact = new string[] { },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(inputExpected);

            //Assert
            Assert.AreEqual(inputExpected, observed);
        }

        [TestMethod]
        public void RedactJToken_JTokenIsNull_ReturnsNull()
        {
            //Setup
            var input = (JToken)null;

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { },
                RegexValuesToRedact = new string[] { },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            Assert.IsNull(observed);
        }

        [TestMethod]
        public void RedactJToken_JTokenInputNodeIsRedactedValue_ReturnsRedactedJson()
        {
            //Setup
            var Jsoninput =
                @"{
                      ""ValueToRedact"": ""Bearer""
                  }";

            var input = JToken.Parse(Jsoninput);
            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            Assert.AreEqual(1, observed.Count());
            Assert.AreEqual(RedactorService.REDACTED_REPLACEMENT_VALUE, observed["ValueToRedact"].ToString());
        }

        [TestMethod]
        public void RedactJToken_JTokenContainsEmbededJson_ReturnsRedactedJson()
        {
            //Setup
            var Jsoninput =
                @"{
                     ""Json"": ""{}""
                  }";

            var input = JToken.Parse(Jsoninput);
            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            Assert.AreEqual(1, observed.Count());
            Assert.AreEqual("{}", observed["Json"].ToString());
        }

        [TestMethod]
        public void RedactJToken_JTokenNodeHasRedactableValue_ReturnsRedactedJson()
        {
            //Setup
            var Jsoninput =
                @"{
                      ""TestObject"": {
                                          ""Password"": ""password""
                                      },
                  }";

            var input = JToken.Parse(Jsoninput);
            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            Assert.AreEqual(1, observed.Count());
            Assert.AreEqual(RedactorService.REDACTED_REPLACEMENT_VALUE, observed["TestObject"]["Password"].ToString());
        }

        [TestMethod]
        public void RedactJToken_JTokenMixedInput_ReturnsRedactedJson()
        {
            //Setup
            var Jsoninput =
                @"{
                      ""TestObject"": {
                                          ""Username"": ""username"",
                                          ""Password"": ""password""
                                      },
                        ""Json"": ""{}"",
                        ""ValueToRedact"": ""Bearer""
                  }";

            var input = JToken.Parse(Jsoninput);
            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.Redact(input);

            //Assert
            Assert.AreEqual(3, observed.Count());
            Assert.AreEqual("{}", observed["Json"].ToString());
            Assert.AreEqual(RedactorService.REDACTED_REPLACEMENT_VALUE, observed["ValueToRedact"].ToString());
            Assert.AreEqual(2, observed["TestObject"].Count());
            Assert.AreEqual("username", observed["TestObject"]["Username"].ToString());
            Assert.AreEqual(RedactorService.REDACTED_REPLACEMENT_VALUE, observed["TestObject"]["Password"].ToString());
        }

        [TestMethod]
        public void IsRedactedValueWithString_ValueToRedactFound_ReturnsTrue()
        {
            //Setup
            var input = @"Bearer 1000";

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.IsRedactedValue(input);

            //Assert
            Assert.IsTrue(observed);
        }

        [TestMethod]
        public void IsRedactedValueWithString_NoValueToRedactFound_ReturnsFalse()
        {
            //Setup
            var input = @"Bearer 1000";

            var redactorServiceOptions = new RedactorServiceOptions
            {
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            var observed = uut.IsRedactedValue(input);

            //Assert
            Assert.IsFalse(observed);
        }

        [TestMethod]
        public void IsRedactedValueWithJValue_ValueToRedactFound_ReturnsTrue()
        {
            //Setup
            var input = new JValue("Bearer");

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.IsRedactedValue(input);

            //Assert
            Assert.IsTrue(observed);
        }

        [TestMethod]
        public void IsRedactedValueWithJValue_NoValueToRedactFound_ReturnsFalse()
        {
            //Setup
            var input = new JValue("Bearer");

            var redactorServiceOptions = new RedactorServiceOptions
            {
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            var observed = uut.IsRedactedValue(input);

            //Assert
            Assert.IsFalse(observed);
        }

        [TestMethod]
        public void IsRedactedValueWithJProperty_ValueToRedactFound_ReturnsTrue()
        {
            //Setup
            var input = new JProperty("Password", "");

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[] { "Password" },
                RegexValuesToRedact = new string[] { "Bearer" },
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);

            var uut = new RedactorService(options);

            //Act
            var observed = uut.IsRedactedValue(input);

            //Assert
            Assert.IsTrue(observed);
        }

        [TestMethod]
        public void IsRedactedValueWithJProperty_NoValueToRedactFound_ReturnsFalse()
        {
            //Setup
            var input = new JProperty("Password", "");

            var redactorServiceOptions = new RedactorServiceOptions
            {
            };

            var options = Options.Create<RedactorServiceOptions>(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            var observed = uut.IsRedactedValue(input);

            //Assert
            Assert.IsFalse(observed);
        }

        [TestMethod]
        public void Parse_InputIsNull_ReturnsNull()
        {
            //Setup
            var input = (string)null;

            var redactorServiceOptions = new RedactorServiceOptions();
            var options = Options.Create(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            var observed = uut.Parse(input);

            //Assert
            Assert.IsNull(observed);
        }

        [TestMethod]
        public void Parse_FirstCharIsNotJson_ReturnsNull()
        {
            //Setup
            var input = "A";

            var redactorServiceOptions = new RedactorServiceOptions();
            var options = Options.Create(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            var observed = uut.Parse(input);

            //Assert
            Assert.IsNull(observed);
        }

        [TestMethod]
        public void Parse_InputIsJsonArray_ReturnsNull()
        {
            //Setup
            var input = "[0,1]";

            var redactorServiceOptions = new RedactorServiceOptions();
            var options = Options.Create(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            var observed = uut.Parse(input);

            //Assert
            Assert.AreEqual(2, observed.Count());
            Assert.AreEqual(0, observed.First());
            Assert.AreEqual(1, observed.Last());
        }

        [TestMethod]
        public void Parse_InputIsJsonObject_ReturnsNull()
        {
            //Setup
            var input = @"{
                          ""Username"": ""username"",
                          ""Password"": ""password""
                        }";

            var redactorServiceOptions = new RedactorServiceOptions();
            var options = Options.Create(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            var observed = uut.Parse(input);

            //Assert
            Assert.AreEqual(2, observed.Count());
            Assert.AreEqual("username", observed["Username"].ToString());
            Assert.AreEqual("password", observed["Password"].ToString());
        }

        [TestMethod]
        public void Parse_InvaildJson_ReturnsNull()
        {
            //Setup
            var input = "{ In%^(:'&vaild }";

            var redactorServiceOptions = new RedactorServiceOptions();
            var options = Options.Create(redactorServiceOptions);
            var uut = new RedactorService(options);

            //Act
            try
            {
                var observed = uut.Parse(input);
                Assert.Fail("Exception Expected");
            }
            catch
            {

            }
        }
    }
}