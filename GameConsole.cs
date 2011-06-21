using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CoralEngine
{
    public class GameConsole
    {
        private Dictionary<string, string> cvars = new Dictionary<string, string>();
        private Dictionary<string, Func<string[], string>> cfuncs = new Dictionary<string, Func<string[], string>>();
        private Dictionary<string, string> aliases = new Dictionary<string, string>();

        public GameConsole()
        {
            AddCfunc("echo", new Func<string[], string>(EchoCfunc));
            AddCfunc("alias", new Func<string[], string>(AddAliasCfunc));
        }

        private string EchoCfunc(string[] args)
        {
            string ret = "";

            foreach (string arg in args)
                ret += arg + " ";

            return ret.Trim() + "\n";
        }

        private string AddAliasCfunc(string[] args)
        {
            if (args.Length != 2)
            {
                string ret = "";
                Dictionary<string, string>.Enumerator e = aliases.GetEnumerator();
                for (int i = 0; i < aliases.Count; i++)
                {
                    e.MoveNext();
                    ret += e.Current.Key + "=\"" + e.Current.Value + "\"\n";
                }
                return ret;
            }
            else
            {
                aliases.Add(args[0], args[1]);
                return args[0] + "=\"" + args[1] + "\"\n";
            }
        }

        public void AddAlias(string name, string command)
        {

        }

        public void AddCvar(string name, string def)
        {
            if (!cvars.ContainsKey(name))
                cvars.Add(name, def);
        }

        public string GetCvar(string name)
        {
            if (cvars.ContainsKey(name))
                return cvars[name];
            return "";
        }

        public void SetCvar(string name, string value)
        {
            if (cvars.ContainsKey(name))
                cvars[name] = value;
        }

        public void AddCfunc(string name, Func<string[], string> func)
        {
            if (!cvars.ContainsKey(name))
                cfuncs.Add(name, func);
        }

        public string Input(string input)
        {
            input = input.Trim();

            if (input.Length > 0)
            {
                if (input[input.Length - 1] == ';')
                    input = input.Substring(0, input.Length - 1);
            }

            Regex stringRegex = new Regex("(.*) \"(.+)\"(.*)");
            string b64r = stringRegex.Replace(input, "$2");
            input = stringRegex.Replace(input, "$1 %B64" + Base64_Encode(b64r) + "%$3");

            string[] parts = input.Split(';');

            string ret = "";
            foreach (string part in parts)
                ret += HandleCommand(part);

            return ret;
        }

        private string HandleCommand(string input)
        {
            input = input.Trim();

            if (input != "")
            {
                string[] parse = input.Split(new char[] { ' ' }, 2);

                if (parse.Length > 0 && parse[0] != "")
                {
                    if (aliases.ContainsKey(parse[0]))
                        return Input(aliases[parse[0]] + (parse.Length == 2 ? " " + parse[1] : ""));
                    else if (cfuncs.ContainsKey(parse[0]))
                    {
                        string[] args = new string[0];
                        if (parse.Length == 2)
                        {
                            args = parse[1].Split(' ');
                            for (int i = 0; i < args.Length; i++)
                                args[i] = HandleArgument(args[i]);
                        }

                        return cfuncs[parse[0]].Invoke(args);
                    }
                    else if (cvars.ContainsKey(parse[0]))
                    {
                        if (parse.Length == 2)
                            cvars[parse[0]] = parse[1];

                        return cvars[parse[0]] + "\n";
                    }
                }
            }

            return "Unknown cvar/cfunc\n";
        }

        private string HandleArgument(string arg)
        {
            Regex b64 = new Regex("^%B64(.+)%$");
            if (b64.IsMatch(arg))
                return Base64_Decode(b64.Replace(arg, "$1"));
            else
                return arg;
        }

        private string Base64_Encode(string input)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input));
        }

        private string Base64_Decode(string input)
        {
            Decoder utf8Decode = new UTF8Encoding().GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(input);
            char[] decoded_char = new char[utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length)];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            return new string(decoded_char);
        }
    }
}