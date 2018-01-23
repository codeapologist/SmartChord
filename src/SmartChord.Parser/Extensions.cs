using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartChord.Parser.Annotations;
using SmartChord.Parser.Models;

namespace SmartChord.Parser
{
    public static class Extensions
    {

        private static readonly IDictionary<string, Note> NoteByString = new Dictionary<string, Note>();

        static Extensions()
        {
            var values = GetValues<Note>();
            foreach (var value in values)
            {
                var attributes = value.GetType()
                    .GetMember(value.ToString())
                    .First()
                    .GetCustomAttributes<AliasAttribute>();

                foreach (var attribute in attributes)
                {
                    NoteByString.Add(attribute.Name, value);
                }
            }
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            var aliasAttribute = enumValue.GetType().GetMember(enumValue.ToString())
                .First()
                .GetCustomAttributes<AliasAttribute>()
                .First();

            if (aliasAttribute == null)
            {
                throw new InvalidOperationException($"AliasAttribute does not exist on member {enumValue}");
            }

            return aliasAttribute.Name;
        }

        public static Note ToNote(this string input)
        {
            if (NoteByString.TryGetValue(input, out var output))
            {
                return output;
            }

            throw new InvalidOperationException($"{input} is not a valid note.");

        }

        public static Note AddHalfStep(this Note note)
        {
            var result = ++note;

            if (result > (Note)11)
            {
                result = 0;
            }

            return result;
        }

        public static Note AddWholeStep(this Note note)
        {
            return note.AddHalfStep()
                .AddHalfStep();
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

}