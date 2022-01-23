using System;

namespace SmartChord.Extractions
{
    public class ExtractorFactory
    {
        public IExtractor GetExtraactor(string url)
        {
            if(url.Contains("ultimate-guitar.com", StringComparison.OrdinalIgnoreCase))
            {
                return new UltimateGuitarExtractor();
            }
            else if (url.Contains("worshipchords.com", StringComparison.OrdinalIgnoreCase))
            {
                return new WorshipChordsExtractor();
            }
            return null;
        }
    }
}