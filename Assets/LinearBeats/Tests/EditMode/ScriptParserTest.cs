using System.IO;
using LinearBeats.Script;
using NUnit.Framework;

public class ScriptParserTest
{
    [Test]
    public void Should_Not_Set_Empty_Script()
    {
        #region Given
        string emptyScript;
        #endregion

        #region When
        emptyScript = @" ";
        #endregion

        #region Then
        Assert.Catch(() => new ScriptParser(emptyScript));
        #endregion
    }

    [Test]
    public void Can_Parse_Valid_Script()
    {
        #region Given
        var validScript = @"
VersionCode: 12
VersionName: 1.1.2
Metadata:
  GameMode: standard
  Category: original
  Level: 3
  LevelJudge: 1
  LevelLife: 1
  Difficulty: 1
  Title: Hello, World! 튜토리얼
  PulsePreviewStart: 0
  PulsePreviewEnd: 10000
  BpmInit: 150.0
  PulsesPerQuarterNote: 240
Timings:
    - Pulse: 3840
      PulseStopDuration: 0
      Bpm: 120.0";
        #endregion

        #region When
        var scriptParser = new ScriptParser(validScript);
        #endregion

        #region Then
        Assert.DoesNotThrow(() => scriptParser.Parse());
        #endregion
    }

    [Test]
    public void Throw_Exception_With_Invalid_Script()
    {
        #region Given
        var invalidScript = @"
VersionCode: 0
VersionName: 1.1.2
Metadata:
  GameMode: standard
  Category: original
  Level: 3
  LevelJudge: 1
  LevelLife: 1
  Difficulty: 1
  Title: Hello, World! 튜토리얼
  PulsePreviewStart: 0
  PulsePreviewEnd: 10000
  BpmInit: 150.0
  PulsesPerQuarterNote: 240
Timings:
    - Pulse: 3840
      PulseStopDuration: 0
      Bpm: 120.0";
        #endregion

        #region When
        var scriptParser = new ScriptParser(invalidScript);
        #endregion

        #region Then
        Assert.Catch<InvalidDataException>(() => scriptParser.Parse());
        #endregion
    }
}
