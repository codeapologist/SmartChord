using SmartChord.Parser.Annotations;

namespace SmartChord.Parser.Models
{
    public enum Note
    {
        Unknown = -1,
        [Alias(Name = "A")] A,
        [Alias(Name = "A#")] [Alias(Name = "Bb")] ASharp,
        [Alias(Name = "B")] B,
        [Alias(Name = "C")] C,
        [Alias(Name = "C#")] [Alias(Name = "Db")] CSharp,
        [Alias(Name = "D")] D,
        [Alias(Name = "D#")] [Alias(Name = "Eb")] DSharp,
        [Alias(Name = "E")] E,
        [Alias(Name = "F")] F,
        [Alias(Name = "F#")] [Alias(Name = "Gb")] FSharp,
        [Alias(Name = "G")] G,
        [Alias(Name = "G#")] [Alias(Name = "Ab")] GSharp
    }
}