using FluentAssertions;
using Moq;
using NUnit.Framework;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Integration;

[TestFixture]
public class BmsRandomResolverUnitTests : BaseUnitTestFixture
{
    [Test]
    public void Test_SetRandom()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "SETRANDOM", Value = "3" },
            new BmsCommand { Name = "IF", Value = "3" },
            new BmsCommand { Name = "PASS" },
            new BmsCommand { Name = "ENDIF" },
        };

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS" }
            }
        );
    }

    [Test]
    public void Test_Random_If()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "RANDOM", Value = "3" },
            new BmsCommand { Name = "IF", Value = "3" },
            new BmsCommand { Name = "PASS" },
            new BmsCommand { Name = "ENDIF" },
        };

        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(x => x.GetInt(It.IsAny<int>())).Returns(3 - 1);
        Mocker.Inject(randomizer);

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS" }
            }
        );
    }

    [Test]
    public void Test_Random_ElseIf()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "RANDOM", Value = "3" },
            new BmsCommand { Name = "IF", Value = "1" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "ELSEIF", Value = "3" },
            new BmsCommand { Name = "PASS" },
            new BmsCommand { Name = "ENDIF" },
        };

        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(x => x.GetInt(It.IsAny<int>())).Returns(3 - 1);
        Mocker.Inject(randomizer);

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS" }
            }
        );
    }

    [Test]
    public void Test_Random_Else()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "RANDOM", Value = "3" },
            new BmsCommand { Name = "IF", Value = "1" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "ELSE" },
            new BmsCommand { Name = "PASS" },
            new BmsCommand { Name = "ENDIF" },
        };

        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(x => x.GetInt(It.IsAny<int>())).Returns(3 - 1);
        Mocker.Inject(randomizer);

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS" }
            }
        );
    }

    [Test]
    public void Test_Switch_Case()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "SWITCH", Value = "3" },
            new BmsCommand { Name = "CASE", Value = "1" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "SKIP" },
            new BmsCommand { Name = "CASE", Value = "3" },
            new BmsCommand { Name = "PASS" },
            new BmsCommand { Name = "SKIP" },
            new BmsCommand { Name = "DEF" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "ENDSW" },
        };

        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(x => x.GetInt(It.IsAny<int>())).Returns(3 - 1);
        Mocker.Inject(randomizer);

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS" }
            }
        );
    }

    [Test]
    public void Test_Switch_Def()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "SWITCH", Value = "3" },
            new BmsCommand { Name = "CASE", Value = "1" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "SKIP" },
            new BmsCommand { Name = "DEF" },
            new BmsCommand { Name = "PASS" },
            new BmsCommand { Name = "ENDSW" },
        };

        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(x => x.GetInt(It.IsAny<int>())).Returns(3 - 1);
        Mocker.Inject(randomizer);

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS" }
            }
        );
    }

    [Test]
    public void Test_Switch_Case_Fallthrough()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "SWITCH", Value = "3" },
            new BmsCommand { Name = "CASE", Value = "1" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "SKIP" },
            new BmsCommand { Name = "CASE", Value = "3" },
            new BmsCommand { Name = "PASS 1" },
            new BmsCommand { Name = "DEF" },
            new BmsCommand { Name = "PASS 2" },
            new BmsCommand { Name = "ENDSW" },
        };

        var randomizer = new Mock<IRandomizer>();
        randomizer.Setup(x => x.GetInt(It.IsAny<int>())).Returns(3 - 1);
        Mocker.Inject(randomizer);

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS 1" },
                new BmsCommand { Name = "PASS 2" }
            }
        );
    }

    [Test]
    public void Test_Nested_Random()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "SETRANDOM", Value = "3" },
            new BmsCommand { Name = "IF", Value = "3" },
            new BmsCommand { Name = "PASS 1" },
            new BmsCommand { Name = "SETRANDOM", Value = "2" },
            new BmsCommand { Name = "IF", Value = "2" },
            new BmsCommand { Name = "PASS 2" },
            new BmsCommand { Name = "ELSE" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "ENDIF" },
            new BmsCommand { Name = "PASS 3" },
            new BmsCommand { Name = "ENDIF" },
        };

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS 1" },
                new BmsCommand { Name = "PASS 2" },
                new BmsCommand { Name = "PASS 3" }
            }
        );
    }

    [Test]
    public void Test_Nested_Inherited_Random()
    {
        // Arrange.
        var commands = new[]
        {
            new BmsCommand { Name = "SETRANDOM", Value = "3" },
            new BmsCommand { Name = "IF", Value = "3" },
            new BmsCommand { Name = "PASS 1" },
            new BmsCommand { Name = "IF", Value = "2" },
            new BmsCommand { Name = "FAIL" },
            new BmsCommand { Name = "ELSE" },
            new BmsCommand { Name = "PASS 2" },
            new BmsCommand { Name = "ENDIF" },
            new BmsCommand { Name = "PASS 3" },
            new BmsCommand { Name = "ENDIF" },
        };

        // Act.
        var subject = Mocker.Create<BmsRandomResolver>();
        var output = subject.Resolve(commands);

        // Assert.
        output.Should().BeEquivalentTo(
            new[]
            {
                new BmsCommand { Name = "PASS 1" },
                new BmsCommand { Name = "PASS 2" },
                new BmsCommand { Name = "PASS 3" }
            }
        );
    }
}