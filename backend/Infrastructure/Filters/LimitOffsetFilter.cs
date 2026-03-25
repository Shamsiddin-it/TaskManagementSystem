public class LimitOffsetFilter
{
    private const int MaxLimit = 100;

    public int Limit { get; set; } = 20;
    public int Offset { get; set; } = 0;

    public void Normalize()
    {
        if (Limit < 1) Limit = 1;
        if (Limit > MaxLimit) Limit = MaxLimit;
        if (Offset < 0) Offset = 0;
    }
}
