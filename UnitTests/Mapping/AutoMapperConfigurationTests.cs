using System.Reflection;
using AutoMapper;
using Common;

namespace UnitTests.Mapping;

public class AutoMapperConfigurationTests
{
    private IMapper _mapper;
    private MapperConfiguration _subject;

    [Fact]
    public async Task Assert_AutoMapperConfiguration_IsValid()
    {

        // Arrange
        var mappingAssemblies = new List<Assembly>
        {
            typeof(MappingAssemblyMarker).Assembly
        };

        _subject = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(mappingAssemblies);
        });
        _mapper = _subject.CreateMapper();

        // Act
        // Assert
        _subject.AssertConfigurationIsValid();
    }

}