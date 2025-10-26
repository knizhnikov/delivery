using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.SharedKernel;

public class LocationShould
{
    [Fact]
    public void BeCorrectWhenAllParamsAreCorrectOnCreate()
    {
        // Arrange
        
        // Act
        var location = Location.Create(1, 10);
        
        // Assert
        location.IsSuccess.Should().BeTrue();
        location.Value.X.Should().Be(1);
        location.Value.Y.Should().Be(10);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(11, 1)]
    [InlineData(1, 11)]
    [InlineData(11, 11)]
    public void ReturnErrorWhenParametersAreInvalidOnCreate(int x, int y)
    {
        // Arrange
        
        // Act
        var location = Location.Create(x, y);
        
        // Assert
        location.IsSuccess.Should().BeFalse();
        location.Error.Should().NotBeNull();
    }

    [Fact]
    public void BeEqualWhenAllPropertiesAreEqual()
    {
        // Arrange
        var location1 = Location.Create(1, 10).Value;
        var location2 = Location.Create(1, 10).Value;

        // Act
        var areEqual = location1 == location2;
        
        // Assert
        areEqual.Should().BeTrue();
    }
    
    [Fact]
    public void NotBeEqualWhenPropertiesAreNotEqual()
    {
        // Arrange
        var location1 = Location.Create(1, 10).Value;
        var location2 = Location.Create(10, 1).Value;

        // Act
        var areEqual = location1 == location2;
        
        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void BeAbleToBeGeneratedRandomly()
    {
        // Arrange
        
        // Act
        var location = Location.CreateRandom();
        
        // Assert
        location.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(1, 1, 2, 2, 2)]
    [InlineData(2, 2, 1, 1, 2)]
    [InlineData(2, 6, 4, 9, 5)]
    public void BeAbleToGetDistanceToAnotherLocation(int x1, int y1, int x2, int y2, int expectedDistance)
    {
        // Arrange
        var location1 = Location.Create(x1, y1).Value;
        var location2 = Location.Create(x2, y2).Value;

        // Act
        var distance = location1.DistanceTo(location2).Value;
        
        // Assert
        distance.Should().Be(expectedDistance);
    }

    [Fact]
    public void ShouldNotGetDistanceToAnInvalidLocation()
    {
        // Arrange
        var location = Location.CreateRandom();
        
        // Act
        var distance = location.DistanceTo(null);
        
        // Assert
        distance.IsSuccess.Should().BeFalse();
    }
}