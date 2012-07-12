using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AAafeen.Models.Commands
{
    public static partial class CommandsExcute
    {
        public static string Excute(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return "コマンドが空のため何もせずに終了しました。";

            var lines = command.Split('\n')
                .Select((line, index) => new { Line = line, LineNum = index + 1 })
                .Where(_ => !string.IsNullOrWhiteSpace(_.Line));

            int completeLineCount = 0;

            string result = "";
            bool methodCalled = false;

            var @params = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                completeLineCount++;
                
                var setProperty = Regex.Match(line.Line, @"^\s*(?<propName>\S+)\s*=\s*(?<propData>\S+)\s*$");
                if (setProperty.Success)
                {
                    @params.Add(setProperty.Groups["propName"].ToString(), setProperty.Groups["propData"].ToString());
                    continue;
                }

                var callMethod = Regex.Match(line.Line, @"^\s*(?<methodName>.+)\s*\(\s*\)\s*$");
                if (callMethod.Success)
                {
                    var method = typeof(Methods).GetMethod(callMethod.Groups["methodName"].ToString().ToLowerInvariant());
                    if (method == null)
                        throw new Exception(callMethod.Groups["methodName"].ToString() + "という命令はありません。");
                    object args = method.GetParameters()[0].ParameterType.GetConstructor(new Type[] { }).Invoke(null);
                    foreach (var param in @params)
                        args.GetType().InvokeMember(param.Key.ToLowerInvariant(), BindingFlags.SetProperty, null, args, new object[] { param.Value });
                    result = method.Invoke(null, new object[] { args }).ToString();
                    methodCalled = true;
                    break;
                }

                throw new Exception(line.LineNum + "行目を処理できませんでした。");
            }

            var ignoreLines = lines.Skip(completeLineCount).Select(_ => _.LineNum);

            var returnDataBuilder = new StringBuilder();

            if (ignoreLines.Any())
            {
                foreach (int lineNum in ignoreLines)
                    returnDataBuilder.AppendFormat("Info：{0}行目を無視しました。\n", lineNum);
                returnDataBuilder.AppendLine();
            }

            if (!methodCalled)
            {
                result = "メソッドが呼び出されていないため何もせずに終了しました。";
            }

            returnDataBuilder.Append(result);

            return returnDataBuilder.ToString();
        }
    }
}
