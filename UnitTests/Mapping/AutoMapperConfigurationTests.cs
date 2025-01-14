using System.Reflection;
using AutoMapper;
using Common;

namespace UnitTests.Mapping;

public class AutoMapperConfigurationTests
{
    private MapperConfiguration _subject = null!;

    public AutoMapperConfigurationTests()
    {
        var mappingAssemblies = new List<Assembly>
        {
            typeof(MappingAssemblyMarker).Assembly
        };

        _subject = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(mappingAssemblies);
        });
        var mapper = _subject.CreateMapper();
    }

    [Fact]
    public void Assert_AutoMapperConfiguration_IsValid()
    {
        // Assert
        _subject.AssertConfigurationIsValid();
    }
}