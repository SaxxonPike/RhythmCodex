using System.IO;
using System.Linq;
using ClientCommon;
using NUnit.Framework;
using Shouldly;

namespace RhythmCodex.Cli.Helpers;

[TestFixture]
public class FileSystemTests : BaseUnitTestFixture<FileSystem, IFileSystem>
{
    [Test]
    public void CurrentPath_ReturnsCurrentDirectory()
    {
        // Act.
        var output = Subject.CurrentPath;
            
        // Assert.
        output.ShouldBe(TestContext.CurrentContext.WorkDirectory);
    }

    [Test]
    public void CombinePath_CombinesPaths()
    {
        // Arrange.
        var paths = CreateMany<string>();

        // Act.
        var output = Subject.CombinePath(paths);
            
        // Assert.
        output.ShouldBe(Path.Combine(paths));
    }

    [Test]
    public void CreateDirectory_CreatesDirectory_WhenDirectoryDoesNotExist()
    {
        // Arrange.
        var name = Create<string>();
            
        try
        {
            // Act.
            Subject.CreateDirectory(name);
                
            // Assert.
            Directory.Exists(name).ShouldBeTrue();
        }
        finally
        {
            Directory.Delete(name);
        }
    }
        
    [Test]
    public void CreateDirectory_DoesNotThrow_WhenDirectoryAlreadyExists()
    {
        // Arrange.
        var name = Create<string>();
        Directory.CreateDirectory(name);
            
        try
        {
            // Act.
            Subject.CreateDirectory(name);
                
            // Assert.
            Directory.Exists(name).ShouldBeTrue();
        }
        finally
        {
            Directory.Delete(name);
        }
    }
        
    [Test]
    public void CreateDirectory_CreatesMultipleLevelDirectories()
    {
        // Arrange.
        var paths = CreateMany<string>();
        Directory.CreateDirectory(Path.Combine(paths));
            
        try
        {
            // Act.
            Subject.CreateDirectory(Path.Combine(paths));
                
            // Assert.
            Directory.Exists(Path.Combine(paths)).ShouldBeTrue();
        }
        finally
        {
            Directory.Delete(paths.First(), true);
        }
    }

    [Test]
    public void GetDirectory_GetsDirectoryPortionOfPath()
    {
        // Arrange.
        var directory = Create<string>();
        var file = Create<string>();
        var path = Path.Combine(directory, file);
                
        // Act.
        var output = Subject.GetDirectory(path);
            
        // Assert.
        output.ShouldBe(directory);
    }

    [Test]
    public void GetDirectoryNames_GetsDirectoryNamesFromPath()
    {
        // Arrange.
        var basePath = Create<string>();
        var directories = CreateMany<string>();
        foreach (var directory in directories)
            Directory.CreateDirectory(Path.Combine(basePath, directory));
            
        try
        {
            // Act.
            var output = Subject.GetDirectoryNames(basePath);
                
            // Assert.
            output.ShouldBe(directories, ignoreOrder: true);
        }
        finally
        {
            Directory.Delete(basePath, true);
        }
    }

    [Test]
    public void GetFileName_GetsFileNameFromPath()
    {
        // Arrange.
        var directory = Create<string>();
        var file = Create<string>();
        var path = Path.Combine(directory, file);
                
        // Act.
        var output = Subject.GetFileName(path);
            
        // Assert.
        output.ShouldBe(file);
    }

    [Test]
    public void GetFileNames_ReturnsSingleFileForPath()
    {
        // Arrange.
        var basePath = Create<string>();
        Directory.CreateDirectory(basePath);
            
        var files = CreateMany<string>();
        foreach (var file in files)
            File.WriteAllBytes(Path.Combine(basePath, file), []);
            
        try
        {
            // Act.
            var output = Subject.GetFileNames(basePath, files.First());
                
            // Assert.
            output.ShouldBe([Path.Combine(basePath, files.First())]);
        }
        finally
        {
            Directory.Delete(basePath, true);
        }
    }
        
    [Test]
    [TestCase("*")]
    [TestCase("")]
    public void GetFileNames_ReturnsAllFilesForWildcardOrEmpty(string pattern)
    {
        // Arrange.
        var basePath = Create<string>();
        Directory.CreateDirectory(basePath);
            
        var files = CreateMany<string>();
        foreach (var file in files)
            File.WriteAllBytes(Path.Combine(basePath, file), []);
            
        try
        {
            // Act.
            var output = Subject.GetFileNames(basePath, pattern);
                
            // Assert.
            output.ShouldBe(files.Select(f => Path.Combine(basePath, f)), ignoreOrder: true);
        }
        finally
        {
            Directory.Delete(basePath, true);
        }
    }
}