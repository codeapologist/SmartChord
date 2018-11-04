using System.Threading.Tasks;

namespace SmartChord.Extractions
{
    public interface IExtractor
    {
        Task<string> GetChordSheetText(string filePath);
    }
}