using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class StoragePlace : Entity<Guid>
{
    public string Name { get; }
    public Volume TotalVolume { get; }
    public Guid? OrderId { get; private set; }

    /// <summary>
    /// Ctr
    /// </summary>
    private StoragePlace()
    {
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="name"></param>
    /// <param name="totalVolume"></param>
    private StoragePlace(string name, Volume totalVolume)
    {
        Name = name;
        TotalVolume = totalVolume;
    }

    /// <summary>
    /// Создает экземпляр StoragePlace
    /// </summary>
    /// <param name="name"></param>
    /// <param name="totalVolume"></param>
    /// <returns></returns>
    public static Result<StoragePlace, Error> Create(string name, Volume totalVolume)
    {
        if (string.IsNullOrEmpty(name)) return GeneralErrors.ValueIsRequired(nameof(name));
        if (string.IsNullOrEmpty(name.Trim())) return GeneralErrors.ValueIsInvalid(nameof(name));

        return new StoragePlace(name, totalVolume);
    }

    /// <summary>
    /// Проверяет можно ли разместить заказ в StoragePlace
    /// </summary>
    /// <param name="orderVolume"></param>
    /// <returns></returns>
    public Result<bool, Error> CanStoreOrder(Volume orderVolume)
    {
        if (OrderId is not null) return Errors.StoragePlaceAlreadyHasOrder(OrderId.Value);
        if (orderVolume > TotalVolume) return Errors.StoragePlaceIsTooSmall(orderVolume, TotalVolume);

        return true;
    }

    /// <summary>
    /// Размещает заказ внутри StoragePlace
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="orderVolume"></param>
    /// <returns></returns>
    public UnitResult<Error> StoreOrder(Guid orderId, Volume orderVolume)
    {
        var canStoreOrder = CanStoreOrder(orderVolume);
        if (canStoreOrder.IsFailure) return canStoreOrder.Error;

        OrderId = orderId;

        return new UnitResult<Error>();
    }

    /// <summary>
    /// Проверяет, есть ли заказ в StoragePlace
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => OrderId == null;

    /// <summary>
    /// Извлекает заказ из StoragePlace
    /// </summary>
    public void RemoveCurrentOrder()
    {
        OrderId = null;
    }

    private static class Errors
    {
        public static Error StoragePlaceAlreadyHasOrder(Guid existingOrderId) =>
            new("storage.place.already.has.order",
                $"Нельзя разместить новый заказ в место хранения: уже размещен другой заказ {existingOrderId.ToString()}");

        public static Error StoragePlaceIsTooSmall(Volume orderVolume, Volume storagePlaceVolume) =>
            new("storage.place.too.small",
                $"Нельзя разместить новый заказ в место хранения: объем заказа ({orderVolume}) превышает вместимость места хранения ({storagePlaceVolume})");
    }
}