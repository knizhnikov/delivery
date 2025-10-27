using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.SharedKernel;

public class Volume : ValueObject
{
    public static readonly string Unit = "л";
    public int Value { get; }

    /// <summary>
    /// Ctr
    /// </summary>
    private Volume()
    {
        
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="value"></param>
    private Volume(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Создает экземпляр объекта Volume
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<Volume, Error> Create(int value)
    {
        if (value <= 0) return GeneralErrors.ValueIsInvalid(nameof(value));
        
        return new Volume(value);
    }
    
    public override string ToString() => $"{Value.ToString()}{Unit}";

    public static bool operator >=(Volume left, Volume right) => left.Value >= right.Value;

    public static bool operator <=(Volume left, Volume right) => left.Value <= right.Value;
    
    public static bool operator <(Volume left, Volume right) => left.Value < right.Value;
    
    public static bool operator >(Volume left, Volume right) => left.Value > right.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}