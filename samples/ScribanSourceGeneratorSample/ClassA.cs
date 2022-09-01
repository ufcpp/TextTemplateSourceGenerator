using System;

namespace ScribanSourceGeneratorSample
{
    using System.Text;

    [ScribanGeneretor.ClassMember("""
        {{ $x = ["a","abc","ABC","xyz"] -}}
        {{- for $i in 0..<$x.size ~}}
            public const string X{{ $i }} = "{{ $x[$i] }}";
        {{ end }}
        """)]
    [ScribanGeneretor.ClassMember("""
        {{- for $i in 0..5 ~}}
            public const int Y{{ $i }} = {{ $i }};
        {{ end }}
        """)]
    internal partial class ClassA
    {
    }

    [ScribanGeneretor.ClassMember("")]
    public partial class ClassB
    {
        [ScribanGeneretor.ClassMember("")]
        protected partial class Inner
        {
        }
    }

    [ScribanGeneretor.ClassMember("")]
    public partial struct StructA { }

    [ScribanGeneretor.ClassMember("")]
    public partial record R1 { }

    [ScribanGeneretor.ClassMember("")]
    public partial record class R2 { }

    [ScribanGeneretor.ClassMember("")]
    public partial record struct R3 { }

    [ScribanGeneretor.ClassMember("")]
    public partial record struct R<T1, T2>
        where T1 : struct
        where T2 : notnull
    { }

    // not supoprted: non-partial class
    [ScribanGeneretor.ClassMember("")] public class NonPartial1 { }
    [ScribanGeneretor.ClassMember("")] public record NonPartial2 { }
    [ScribanGeneretor.ClassMember("")] public record class NonPartial3 { }
    [ScribanGeneretor.ClassMember("")] public struct NonPartial4 { }
    [ScribanGeneretor.ClassMember("")] public record struct NonPartial5 { }
}
