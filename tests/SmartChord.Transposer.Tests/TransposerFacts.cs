using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Xunit;

   
namespace SmartChord.Transposer.Tests
{
    public class TransposerFacts
    {
        private string GetResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException("Cannot find resource.")))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        [Fact]
        public void Changing_without_original_key_specified_works()
        {
            var transposer = new Transposer();

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            string song = GetResource($"{assemblyName}.Resources.Input.tears-in-heaven-key-a.txt");

            var result = transposer.ChangeKey(song, "C");

            string songInKeyOfC = GetResource($"{assemblyName}.Resources.Output.tears-in-heaven-key-c.txt");

            result.Should().Be(songInKeyOfC);
        }

        [Fact]
        public void Changing_with_original_key_specified_works()
        {
            var transposer = new Transposer();
                
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            string song = GetResource($"{assemblyName}.Resources.Input.tears-in-heaven-key-a.txt");

            var result = transposer.ChangeKey(song, "C", "A");

            string songInKeyOfC = GetResource($"{assemblyName}.Resources.Output.tears-in-heaven-key-c.txt");

            result.Should().Be(songInKeyOfC);
        }


        [Fact]
        public void Song_with_ambiguous_elements_works()
        {
            var transposer = new Transposer();

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            string song = GetResource($"{assemblyName}.Resources.Input.fake-song-key-a.txt");

            var result = transposer.ChangeKey(song, "G");

            string songInKeyOfG = GetResource($"{assemblyName}.Resources.Output.fake-song-key-g.txt");

            result.Should().Be(songInKeyOfG);

        }
    }
}
