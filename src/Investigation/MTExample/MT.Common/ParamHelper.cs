using System;
using System.Collections.Generic;

namespace Common
{
    public class ParamHelpers
    {
        private string[] Args { get; set; }
        private int NumArgs { get; set; }

        public ParamHelpers(string[] args)
        {
            Args = args;
            NumArgs = args.Length;
        }

        public void CheckArgs(string paramName, int readIndex, int expectedParamCount)
        {
            if (readIndex + expectedParamCount - 1 >= NumArgs) throw new ArgumentException($"Expected {expectedParamCount} arguments for {paramName}");

            for (var paramOffset = 0; paramOffset < expectedParamCount; paramOffset++)
            {
                if (Args[readIndex + paramOffset].StartsWith("-")) throw new ArgumentException($"Expected {expectedParamCount} arguments for {paramName}");
            }
        }

        public (int NewIndex, bool Value) SetSwitch(int readIndex)
        {
            if (readIndex >= NumArgs || Args[readIndex].StartsWith("-"))
            {
                return (readIndex, true);
            }

            var candidate = Args[readIndex];
            if ((new List<string> { "true", "y", "1" }).Contains(candidate.ToLowerInvariant()))
            {
                return (readIndex + 1, true);
            }
            if ((new List<string> { "false", "n", "0" }).Contains(candidate.ToLowerInvariant()))
            {
                return (readIndex + 1, false);
            }
            throw new ArgumentException($"Switch parameters only support 'true', 'y', '1', 'false', 'n', '0'. Could not parse '{candidate}'");
        }

        public (int NewIndex, string ChildParam) ReadString(string paramName, int readIndex)
        {
            return (readIndex + 1, Args[readIndex]);
        }
    }

}
