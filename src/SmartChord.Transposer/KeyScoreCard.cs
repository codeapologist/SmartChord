using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SmartChord.Parser;
using SmartChord.Parser.Models;

namespace SmartChord.Transpose
{
    public class KeyScoreCard
    {
        private readonly Note? _targetKey;
        private readonly Note[] _scale = new Note[7];

        public KeyScoreCard(Note targetKey)
        {
            _targetKey = targetKey;
            Init();
        }

        private void Init()
        {
            if (_targetKey != null)
            {
                Note currentNote = _targetKey.Value;

                _scale[0] = currentNote;

                for (int i = 1; i < 7; i++)
                {
                    if (i == 3)
                    {
                        currentNote = currentNote.AddHalfStep();
                    }
                    else
                    {
                        currentNote = currentNote.AddWholeStep();
                    }

                    _scale[i] = currentNote;
                }
            }
        }

        public int Score(List<SongStat> stats)
        {
            var score = 0;
            var scaleCounts = new int[7];

            foreach (var stat in stats)
            {
                var index = Array.IndexOf(_scale, stat.RootNote);

                if (index >= 0)
                {
                    if (stat.Tone == Tone.Minor && IsMinorPosition(index))
                    {
                        scaleCounts[index]++;
                    }
                    else if(stat.Tone == Tone.Major && IsMajorPosition(index))
                    {
                        scaleCounts[index]++;
                    }
                }
            }

            score += scaleCounts.Count(x => x > 0);

            if (scaleCounts[ScalePosition._1st] > 0)
            {
                score += 2;
            }

            if (scaleCounts[ScalePosition._4th] > 0)
            {
                score += 2;
            }

            if(scaleCounts[ScalePosition._5th] > 0)
            {
                score += 2;
            }

            if (scaleCounts[ScalePosition._6th] > 0)
            {
                score += 1;
            }


            return score;
        }

        private bool IsMinorPosition(int position)
        {

            return position == ScalePosition._2nd || position == ScalePosition._3rd || position == ScalePosition._6th;
        }

        private bool IsMajorPosition(int position)
        {

            return position == ScalePosition._1st || position == ScalePosition._4th || position == ScalePosition._5th;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static class ScalePosition
        {
            public static int _1st => 1 - 1;
            public static int _2nd => 2 - 1;
            public static int _3rd => 3 - 1;
            public static int _4th => 4 - 1;
            public static int _5th => 5 - 1;
            public static int _6th => 6 - 1;
            public static int _7th => 7 - 1;
        }
    }
}