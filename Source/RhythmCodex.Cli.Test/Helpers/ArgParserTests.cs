using System;
using System.Linq;
using ClientCommon;
using NUnit.Framework;
using Shouldly;

namespace RhythmCodex.Cli.Helpers;

[TestFixture]
public class ArgParserTests : BaseUnitTestFixture<ArgParser, IArgParser>
{
    [Test]
    public void Parse_ParsesEmptyArgs()
    {
        // Arrange.
        var data = Array.Empty<string>();

        // Act.
        var output = Subject.Parse(data);

        // Assert.
        output.Options.ShouldBeEmpty();
    }

    [Test]
    public void Parse_ParsesSingleKeySingleValue()
    {
        // Arrange.
        var argKind = OneOf(CharacterSets.AsciiLetters);
        var argValue = Create<string>();
        var data = new[] {$"-{argKind}", argValue};

        // Act.
        var output = Subject.Parse(data);

        // Assert.
        output.Options.Count.ShouldBe(1);
        output.Options[$"{argKind}"].ShouldBe([argValue]);
    }

    [Test]
    public void Parse_ParsesSingleKeyMultiValue()
    {
        // Arrange.
        var argKind = OneOf(CharacterSets.AsciiLetters);
        var argValues = CreateMany<string>(2);
        var data = new[] {$"-{argKind}", argValues[0], $"-{argKind}", argValues[1]};

        // Act.
        var output = Subject.Parse(data);

        // Assert.
        output.Options.Count.ShouldBe(1);
        output.Options[$"{argKind}"].ShouldBeEquivalentTo(argValues);
    }

    [Test]
    public void Parse_ParsesMultiKeyMultiValue()
    {
        // Arrange.
        var argKinds = ManyOf(CharacterSets.AsciiLetters, 2, true);
        var argValues = CreateMany<string>(2);
        var data = new[] {$"-{argKinds[0]}", argValues[0], $"-{argKinds[1]}", argValues[1]};

        // Act.
        var output = Subject.Parse(data);

        // Assert.
        output.Options.Count.ShouldBe(2);
        output.Options[$"{argKinds[0]}"].ShouldBe([argValues[0]]);
        output.Options[$"{argKinds[1]}"].ShouldBe([argValues[1]]);
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
        output.InputFiles.Count.ShouldBe(1);
        output.InputFiles.ShouldBe([argValue]);
    }

    [Test]
    public void Parse_ParsesDefaultKeyMultiValue()
    {
        // Arrange.
        var argValues = CreateMany<string>();

        // Act.
        var output = Subject.Parse(argValues);

        // Assert.
        output.InputFiles.Count.ShouldBe(argValues.Length);
        output.InputFiles.ShouldBe(argValues);
    }

    [Test]
    public void Parse_ParsesMixtureOfDefaultAndSpecificKeys()
    {
        // Arrange.
        var argKinds = Create<string>().Take(2).ToArray();
        var argValues = CreateMany<string>(2);
        var defaultValues = CreateMany<string>(2);
        var data = new[]
            {$"-{argKinds[0]}", argValues[0], defaultValues[0], $"-{argKinds[1]}", argValues[1], defaultValues[1]};
            
        // Act.
        var output = Subject.Parse(data);
            
        // Assert.
        output.Options.Count.ShouldBe(2);
        output.InputFiles.ShouldBeEquivalentTo(defaultValues);
        output.Options[$"{argKinds[0]}"].ShouldBe([argValues[0]]);
        output.Options[$"{argKinds[1]}"].ShouldBe([argValues[1]]);
    }
        
    [Test]
    public void Parse_ParsesEscapedArgs()
    {
        // Arrange.
        var defaultValues = CreateMany<string>(3).Select(s => $"-{s}").ToArray();
        var data = new[] {"--"}.Concat(defaultValues);
            
        // Act.
        var output = Subject.Parse(data);
            
        // Assert.
        output.InputFiles.ShouldBeEquivalentTo(defaultValues);
    }
}