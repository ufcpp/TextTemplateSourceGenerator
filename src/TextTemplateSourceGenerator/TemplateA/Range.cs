namespace TextTemplateSourceGenerator.TemplateA;

public readonly struct Range : IEquatable<Range>
{
    public int Start { get; }
    public int End { get; }
    public Range(int start, int end) => (Start, End) = (start, end);
    public override bool Equals(object? value) => value is Range r && r.Start == Start && r.End == End;
    public bool Equals(Range other) => other.Start == Start && other.End == End;
    public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();
    public int Length => End - Start;
}
