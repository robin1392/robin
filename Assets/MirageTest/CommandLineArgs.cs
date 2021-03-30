namespace MirageTest
{
    public static class CommandLineArgs
    {
        public static int? GetInt(string key)
        {
            int? value = null;
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i];
                if (!arg.StartsWith("-"))
                {
                    continue;
                }

                var argKey = arg.TrimStart('-');
                if (argKey == key)
                {
                    var valueIndex = i + 1;
                    if (valueIndex >= arg.Length)
                    {
                        break;
                    }

                    var valueStr = args[valueIndex];
                    if (!int.TryParse(valueStr, out var valueParsed))
                    {
                        break;
                    }

                    value = valueParsed;
                }
            }

            return value;
        }
        
        public static string GetString(string key)
        {
            string value = null;
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i];
                if (!arg.StartsWith("-"))
                {
                    continue;
                }

                var argKey = arg.TrimStart('-');
                if (argKey == key)
                {
                    var valueIndex = i + 1;
                    if (valueIndex >= arg.Length)
                    {
                        break;
                    }
                    
                    value = args[valueIndex];
                }
            }

            return value;
        }
        
        public static bool HasArg(string key)
        {
            string value = null;
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i];
                if (!arg.StartsWith("-"))
                {
                    continue;
                }

                var argKey = arg.TrimStart('-');
                if (argKey == key)
                {
                    return true;
                }
            }

            return false;
        }
    }
}