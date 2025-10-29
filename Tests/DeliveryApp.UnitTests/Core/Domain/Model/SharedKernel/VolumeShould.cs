using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Core.Domain.Model.SharedKernel;

public class VolumeShould
{
    [Fact]
    public void BeCorrectWhenValueIsValidOnCreate()
    {
        // Arrange
        var value = 5;
        
        // Act
        var volume = Volume.Create(value);
        
        // Assert
        volume.IsSuccess.Should().BeTrue();
        volume.Value.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void ReturnErrorWhenValueIsInvalidOnCreate(int value)
    {
        // Arrange
        
        // Act
        var volume = Volume.Create(value);
        
        // Assert
        volume.IsSuccess.Should().BeFalse();
        volume.Error.Should().NotBeNull();
    }

    [Fact]
    public void BeEqualWhenValuesAreEqual()
    {
        // Arrange
        var volume1 = Volume.Create(5).Value;
        var volume2 = Volume.Create(5).Value;

        // Act
        var areEqual = volume1 == volume2;
        
        // Assert
        areEqual.Should().BeTrue();
    }
    
    [Fact]
    public void NotBeEqualWhenValuesAreNotEqual()
    {
        // Arrange
        var volume1 = Volume.Create(5).Value;
        var volume2 = Volume.Create(10).Value;

        // Act
        var areEqual = volume1 == volume2;
        
        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void ReturnCorrectStringRepresentation()
    {
        // Arrange
        var volume = Volume.Create(5).Value;
        
        // Act
        var stringRepresentation = volume.ToString();
        
        // Assert
        stringRepresentation.Should().Be("5л");
    }

    [Theory]
    [InlineData(5, 3, true)]
    [InlineData(3, 5, false)]
    [InlineData(5, 5, true)]
    public void CompareCorrectlyWithGreaterThanOrEqualOperator(int leftValue, int rightValue, bool expectedResult)
    {
        // Arrange
        var leftVolume = Volume.Create(leftValue).Value;
        var rightVolume = Volume.Create(rightValue).Value;

        // Act
        var result = leftVolume >= rightVolume;
        
        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(3, 5, true)]
    [InlineData(5, 3, false)]
    [InlineData(5, 5, true)]
    public void CompareCorrectlyWithLessThanOrEqualOperator(int leftValue, int rightValue, bool expectedResult)
    {
        // Arrange
        var leftVolume = Volume.Create(leftValue).Value;
        var rightVolume = Volume.Create(rightValue).Value;

        // Act
        var result = leftVolume <= rightVolume;
        
        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(3, 5, true)]
    [InlineData(5, 3, false)]
    [InlineData(5, 5, false)]
    public void CompareCorrectlyWithLessThanOperator(int leftValue, int rightValue, bool expectedResult)
    {
        // Arrange
        var leftVolume = Volume.Create(leftValue).Value;
        var rightVolume = Volume.Create(rightValue).Value;

        // Act
        var result = leftVolume < rightVolume;
        
        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(5, 3, true)]
    [InlineData(3, 5, false)]
    [InlineData(5, 5, false)]
    public void CompareCorrectlyWithGreaterThanOperator(int leftValue, int rightValue, bool expectedResult)
    {
        // Arrange
        var leftVolume = Volume.Create(leftValue).Value;
        var rightVolume = Volume.Create(rightValue).Value;

        // Act
        var result = leftVolume > rightVolume;
        
        // Assert
        result.Should().Be(expectedResult);
    }
}