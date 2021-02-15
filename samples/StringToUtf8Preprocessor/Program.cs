using StringToUtf8Preprocessor;
using System;

Console.WriteLine(Generator.Generate(null, "Literal", new[]
{
    ("M1", "abc", Accessibility.Public),
    ("M2", "\u0001\u0002\u0003", Accessibility.Protected),
    ("M3", "aΩБあ中😀", Accessibility.ProtectedAndInternal),
}));

Console.WriteLine(Generator.Generate("Utf8PreprocessorSample", "Literal", new[]
{
    ("M1", "à́̂̃̄̅̆̇̈̉̊̋̌̍̎̏", Accessibility.ProtectedOrInternal),
    ("M2", "🤹‍♀️", Accessibility.Internal),
    ("M3", "", Accessibility.Private),
}));
