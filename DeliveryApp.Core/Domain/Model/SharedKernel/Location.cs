using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.SharedKernel;

public class Location : ValueObject
{
    /// <summary>
    /// Горизонталь
    /// </summary>
    public int X { get; }
    
    /// <summary>
    /// Вертикаль
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Location()
    {
        
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Создать экземпляр Location
    /// </summary>
    /// <param name="x">Вертикаль</param>
    /// <param name="y">Горизонталь</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns></returns>
    public static Result<Location, Error> Create(int x, int y)
    {
        if (x is < 1 or > 10) return GeneralErrors.ValueIsInvalid(nameof(x));
        if (y is < 1 or > 10) return GeneralErrors.ValueIsInvalid(nameof(y));

        return new Location(x, y);
    }
    
    /// <summary>
    /// Создать рандомный экземпляр Location
    /// </summary>
    /// <returns></returns>
    public static Location CreateRandom()
    {
        var random = new Random();
        var x = random.Next(1, 10);
        var y = random.Next(1, 10);
        
        return Create(x, y).GetValueOrDefault();
    }

    /// <summary>
    /// Рассчитывает расстояние до целевой точки
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Result<int, Error> DistanceTo(Location target)
    {
        if (target is null) return GeneralErrors.ValueIsRequired(nameof(target));
        
        var distanceX = Math.Abs(X - target.X);
        var distanceY = Math.Abs(Y - target.Y);
        
        return distanceX + distanceY;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}