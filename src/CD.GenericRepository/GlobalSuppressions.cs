using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1068:CancellationToken parameters must come last",
    Justification = "The affected obsolete member preserves the parameter order published in version 2.x.")]
