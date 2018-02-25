![Smart Chord](images/icon.png)

# Smart Chord Transposer [![NuGet](https://img.shields.io/nuget/v/SmartChordTransposer.svg)](https://nuget.org/packages/SmartChordTransposer) [![Build status](https://ci.appveyor.com/api/projects/status/pk68p3nayy34b7js?svg=true)](https://ci.appveyor.com/project/codeapologist/smartchordtransposer)

A .NET library to parse, transpose, and analyze chord sheets

## Usage

```csharp
string chordSheet = 

@"Tears in Heaven – Eric Clapton

A   E/G#  F#m   D  E7sus E7  A

[Chorus]

A            E       F#m
Would you know my name,
D    A            E
If I saw you in heaven"
```
### Provide the transposer the text of the chord sheet and destination key. 

```csharp
var transposer = new Transposer();

var output = transposer.ChangeKey(chordSheet, "C"); //The original key is A.

```

### The output:
```
Tears in Heaven – Eric Clapton

C   G/B  Am   F  G7sus G7  C


[Chorus]

C            G       Am
Would you know my name,
F    C            G
If I saw you in heaven

```

 ### Optionally, you can provide the original key of the song. The transposer will attempt to determine the key if omitted.
