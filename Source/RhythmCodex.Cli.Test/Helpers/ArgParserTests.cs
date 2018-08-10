﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace RhythmCodex.Cli.Helpers
{
    [TestFixture]
    public class ArgParserTests : BaseUnitTestFixture<ArgParser, IArgParser>
    {
        [Test]
        public void Parse_ParsesEmptyArgs()
        {
            // Arrange.
            var data = new string[] { };

            // Act.
            var output = Subject.Parse(data);

            // Assert.
            output.Should().BeEmpty();
        }

        [Test]
        public void Parse_ParsesSingleKeySingleValue()
        {
            // Arrange.
            var argKind = Create<char>();
            var argValue = Create<string>();
            var data = new[] {$"-{argKind}", argValue};

            // Act.
            var output = Subject.Parse(data);

            // Assert.
            output.Should().HaveCount(1);
            output[$"{argKind}"].Should().BeEquivalentTo(new[] {argValue});
        }

        [Test]
        public void Parse_ParsesSingleKeyMultiValue()
        {
            // Arrange.
            var argKind = Create<char>();
            var argValues = CreateMany<string>(2);
            var data = new[] {$"-{argKind}", argValues[0], $"-{argKind}", argValues[1]};

            // Act.
            var output = Subject.Parse(data);

            // Assert.
            output.Should().HaveCount(1);
            output[$"{argKind}"].Should().BeEquivalentTo(argValues);
        }

        [Test]
        public void Parse_ParsesMultiKeyMultiValue()
        {
            // Arrange.
            var argKinds = CreateMany<char>(2);
            var argValues = CreateMany<string>(2);
            var data = new[] {$"-{argKinds[0]}", argValues[0], $"-{argKinds[1]}", argValues[1]};

            // Act.
            var output = Subject.Parse(data);

            // Assert.
            output.Should().HaveCount(2);
            output[$"{argKinds[0]}"].Should().BeEquivalentTo(new[] {argValues[0]});
            output[$"{argKinds[1]}"].Should().BeEquivalentTo(new[] {argValues[1]});
        }

        [Test]
        public void Parse_ParsesDefaultKeySingleValue()
        {
            // Arrange.
            var argValue = Create<string>();
            var data = new[] {argValue};

            // Act.
            var output = Subject.Parse(data);

            // Assert.
            output.Should().HaveCount(1);
            output[string.Empty].Should().BeEquivalentTo(new[] {argValue});
        }

        [Test]
        public void Parse_ParsesDefaultKeyMultiValue()
        {
            // Arrange.
            var argValues = CreateMany<string>();

            // Act.
            var output = Subject.Parse(argValues);

            // Assert.
            output.Should().HaveCount(1);
            output[string.Empty].Should().BeEquivalentTo(argValues);
        }

        [Test]
        public void Parse_ParsesMixtureOfDefaultAndSpecificKeys()
        {
            // Arrange.
            var argKinds = CreateMany<char>(2);
            var argValues = CreateMany<string>(2);
            var defaultValues = CreateMany<string>(2);
            var data = new[]
                {$"-{argKinds[0]}", argValues[0], defaultValues[0], $"-{argKinds[1]}", argValues[1], defaultValues[1]};
            
            // Act.
            var output = Subject.Parse(data);
            
            // Assert.
            output.Should().HaveCount(3);
            output[string.Empty].Should().BeEquivalentTo(defaultValues);
            output[$"{argKinds[0]}"].Should().BeEquivalentTo(argValues[0]);
            output[$"{argKinds[1]}"].Should().BeEquivalentTo(argValues[1]);
        }
        
        [Test]
        public void Parse_ParsesEscapedArgs()
        {
            // Arrange.
            var defaultValues = CreateMany<string>().Select(s => $"-{s}").ToArray();
            var data = new[] {"--"}.Concat(defaultValues);
            
            // Act.
            var output = Subject.Parse(data);
            
            // Assert.
            output.Should().HaveCount(1);
            output[string.Empty].Should().BeEquivalentTo(defaultValues);
        }
    }
}