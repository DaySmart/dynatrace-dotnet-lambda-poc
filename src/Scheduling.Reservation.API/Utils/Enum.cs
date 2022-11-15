namespace Scheduling.Reservation.Utils;

public class Enum<T> where T : Enum
{
    public static IEnumerable<T> GetAllValuesAsIEnumerable()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}

public class EnumDto
{
    public int Id { get { return Convert.ToInt32(_enum); } }
    public string Name { get { return _enum.ToString(); } }
    private Enum _enum;
    public EnumDto(Enum inputEnum)
    {
        _enum = inputEnum;
    }
}