using System.Reflection;
using LinearBeats.Input;
using NUnit.Framework;
using UnityEngine;

public class ScriptParserTest
{
    [Test]
    public void Can_Parse_Valid_Script()
    {
        #region Given
        var validScript = @"VersionCode: 12
VersionName: 1.1.2
Metadata:
  GameMode: standard
  Category: original
  Level: 3
  LevelJudge: 1
  LevelLife: 1
  Difficulty: 1";
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
        var invalidScript = @"VersionCode: 12
VersionName: 1.1.2
Metadata:
  GameMode: standard
  Category: original
  Level: 3
  LevelJudge: 1
  LevelLife: 1
  Difficulty: 1";
        #endregion

        #region When
        var scriptParser = new ScriptParser(invalidScript);
        #endregion

        #region Then
        Assert.Throws<Exception>(() => scriptParser.Parse());
        #endregion
    }
}
