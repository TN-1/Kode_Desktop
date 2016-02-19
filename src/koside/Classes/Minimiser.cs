using System;
using System.Text.RegularExpressions;


namespace koside
{
    class Minimiser
    {
        static public string Minimise(string cache)
        {
            //Open file into string s
            string s = cache;

            //Remove comments
            s = Regex.Replace(s, @"//(.*?)\r?\n", me => {
                if (me.Value.StartsWith("//"))
                    return me.Value.StartsWith("//") ? Environment.NewLine : "";
                return me.Value;
            }, RegexOptions.Singleline);

            //Remove double spaces
            s = s.Replace("  ", " ");

            //Remove lines containing only whitespace
            s = Regex.Replace(s, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

            return s;
        }
    }
}
