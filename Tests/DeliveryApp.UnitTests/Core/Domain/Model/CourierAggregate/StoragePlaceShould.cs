using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DeliveryApp.UnitTests.Core.Domain.Model.CourierAggregate;

public class StoragePlaceShould
{
    private readonly ITestOutputHelper _testOutputHelper;

    public StoragePlaceShould(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private static StoragePlace CreateStoragePlace(string name = "Место хранения 1", int totalVolumeValue = 10)
    {
        var totalVolume = Volume.Create(totalVolumeValue).Value;
        return StoragePlace.Create(name, totalVolume).Value;
    }

    private static Volume CreateOrderVolume(int value = 5)
    {
        return Volume.Create(value).Value;
    }

    [Fact]
    public void BeCorrectWhenAllParamsAreValidOnCreate()
    {
        // Arrange
        var name = "Место хранения 1";
        var totalVolume = Volume.Create(10).Value;
        
        // Act
        var storagePlace = StoragePlace.Create(name, totalVolume);
        
        // Assert
        storagePlace.IsSuccess.Should().BeTrue();
        storagePlace.Value.Name.Should().Be(name);
        storagePlace.Value.TotalVolume.Should().Be(totalVolume);
        storagePlace.Value.OrderId.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void ReturnErrorWhenNameIsInvalidOnCreate(string name)
    {
        // Arrange
        var totalVolume = Volume.Create(10).Value;
        
        // Act
        var storagePlace = StoragePlace.Create(name, totalVolume);
        
        // Assert
        storagePlace.IsSuccess.Should().BeFalse();
        storagePlace.Error.Should().NotBeNull();
        _testOutputHelper.WriteLine(storagePlace.Error.Message);
    }
    
    [Fact]
    public void NotBeEqualWhenAllPropertiesButIdAreEqual()
    {
        // Arrange
        var storagePlace1 = CreateStoragePlace();
        var storagePlace2 = CreateStoragePlace();

        // Act
        var areEqual = storagePlace1 == storagePlace2;
        
        // Assert
        areEqual.Should().BeFalse();
    }
    
    [Fact]
    public void NotBeEqualWhenPropertiesAreNotEqual()
    {
        // Arrange
        var storagePlace1 = CreateStoragePlace("Место хранения 1");
        var storagePlace2 = CreateStoragePlace("Место хранения 2");

        // Act
        var areEqual = storagePlace1 == storagePlace2;
        
        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void AllowStoringOrderWhenStoragePlaceIsEmptyAndVolumeIsSufficient()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        var orderVolume = CreateOrderVolume();
        
        // Act
        var canStore = storagePlace.CanStoreOrder(orderVolume);
        
        // Assert
        canStore.IsSuccess.Should().BeTrue();
        canStore.Value.Should().BeTrue();
    }

    [Fact]
    public void NotAllowStoringOrderWhenStoragePlaceAlreadyHasOrder()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        var orderVolume = CreateOrderVolume();
        var existingOrderId = Guid.NewGuid();
        
        // Act
        storagePlace.StoreOrder(existingOrderId, orderVolume);
        var canStore = storagePlace.CanStoreOrder(orderVolume);
        
        // Assert
        canStore.IsSuccess.Should().BeFalse();
        canStore.Error.Code.Should().Be("storage.place.already.has.order");
        _testOutputHelper.WriteLine(canStore.Error.Message);
    }

    [Fact]
    public void NotAllowStoringOrderWhenOrderVolumeExceedsStorageCapacity()
    {
        // Arrange
        var storagePlace = CreateStoragePlace(totalVolumeValue: 5);
        var orderVolume = CreateOrderVolume(10);
        
        // Act
        var canStore = storagePlace.CanStoreOrder(orderVolume);
        
        // Assert
        canStore.IsSuccess.Should().BeFalse();
        canStore.Error.Code.Should().Be("storage.place.too.small");
        _testOutputHelper.WriteLine(canStore.Error.Message);
    }

    [Fact]
    public void SuccessfullyStoreOrderWhenConditionsAreMet()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        var orderVolume = CreateOrderVolume();
        var orderId = Guid.NewGuid();
        
        // Act
        var storeResult = storagePlace.StoreOrder(orderId, orderVolume);
        
        // Assert
        storeResult.IsSuccess.Should().BeTrue();
        storagePlace.OrderId.Should().Be(orderId);
    }

    [Fact]
    public void NotStoreOrderWhenStoragePlaceAlreadyHasOrder()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        var orderVolume = CreateOrderVolume();
        var existingOrderId = Guid.NewGuid();
        var newOrderId = Guid.NewGuid();
        
        // Act
        storagePlace.StoreOrder(existingOrderId, orderVolume);
        var storeResult = storagePlace.StoreOrder(newOrderId, orderVolume);
        
        // Assert
        storeResult.IsSuccess.Should().BeFalse();
        storagePlace.OrderId.Should().Be(existingOrderId);
        storeResult.Error.Code.Should().Be("storage.place.already.has.order");
        _testOutputHelper.WriteLine(storeResult.Error.Message);
    }

    [Fact]
    public void NotStoreOrderWhenOrderVolumeExceedsStorageCapacity()
    {
        // Arrange
        var storagePlace = CreateStoragePlace(totalVolumeValue: 5);
        var orderVolume = CreateOrderVolume(10);
        var orderId = Guid.NewGuid();
        
        // Act
        var storeResult = storagePlace.StoreOrder(orderId, orderVolume);
        
        // Assert
        storeResult.IsSuccess.Should().BeFalse();
        storagePlace.OrderId.Should().BeNull();
        storeResult.Error.Code.Should().Be("storage.place.too.small");
        _testOutputHelper.WriteLine(storeResult.Error.Message);
    }

    [Fact]
    public void BeEmptyWhenCreated()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        
        // Act
        var isEmpty = storagePlace.IsEmpty();
        
        // Assert
        isEmpty.Should().BeTrue();
    }

    [Fact]
    public void NotBeEmptyWhenOrderIsStored()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        var orderVolume = CreateOrderVolume();
        var orderId = Guid.NewGuid();
        
        // Act
        storagePlace.StoreOrder(orderId, orderVolume);
        var isEmpty = storagePlace.IsEmpty();
        
        // Assert
        isEmpty.Should().BeFalse();
    }

    [Fact]
    public void BeEmptyAfterRemovingOrder()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        var orderVolume = CreateOrderVolume();
        var orderId = Guid.NewGuid();
        
        // Act
        storagePlace.StoreOrder(orderId, orderVolume);
        storagePlace.RemoveCurrentOrder();
        var isEmpty = storagePlace.IsEmpty();
        
        // Assert
        isEmpty.Should().BeTrue();
        storagePlace.OrderId.Should().BeNull();
    }

    [Fact]
    public void AllowStoringNewOrderAfterRemovingCurrentOrder()
    {
        // Arrange
        var storagePlace = CreateStoragePlace();
        var orderVolume = CreateOrderVolume();
        var firstOrderId = Guid.NewGuid();
        var secondOrderId = Guid.NewGuid();
        
        // Act
        storagePlace.StoreOrder(firstOrderId, orderVolume);
        storagePlace.RemoveCurrentOrder();
        var storeResult = storagePlace.StoreOrder(secondOrderId, orderVolume);
        
        // Assert
        storeResult.IsSuccess.Should().BeTrue();
        storagePlace.OrderId.Should().Be(secondOrderId);
        storagePlace.IsEmpty().Should().BeFalse();
    }
}