using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1707:IdentifiersShouldNotContainUnderscores",
    Justification = "Подчёркивания допустимы в именах тестовых методов (Given_When_Then).")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "Документируется только публичное API; внутренние элементы намеренно без комментариев.")]

// #error version
