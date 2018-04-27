using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebGM
{
    public class WebGMCmds
    {
        [CUDLR.Command("log", "设置log级别, log verbose/debug/warning/error/fatal")]
        public static void LogLevel(string[] args)
        {
            if (args.Length > 0)
            {
                var level = args[0];
                switch (level)
                {
                    case "verbose":
                        {
                            LogSystem.LogLevel = ELogLevel.Verbose;
                            CUDLR.Console.Log("Done. Set logLevel to:" + level);
                            break;
                        }
                    case "debug":
                        {
                            LogSystem.LogLevel = ELogLevel.Debug;
                            CUDLR.Console.Log("Done. Set logLevel to:" + level);
                            break;
                        }
                    case "warning":
                        {
                            LogSystem.LogLevel = ELogLevel.Warning;
                            CUDLR.Console.Log("Done. Set logLevel to:" + level);
                            break;
                        }
                    case "error":
                        {
                            LogSystem.LogLevel = ELogLevel.Error;
                            CUDLR.Console.Log("Done. Set logLevel to:" + level);
                            break;
                        }
                    case "fatal":
                        {
                            LogSystem.LogLevel = ELogLevel.Fatal;
                            CUDLR.Console.Log("Done. Set logLevel to:" + level);
                            break;
                        }
                    default:
                        {
                            CUDLR.Console.Log("invalid params");
                            break;
                        }

                }
            }
            else
            {
                CUDLR.Console.Log("Current LogLevel is:" + LogSystem.LogLevel.ToString());
            }

        }

        [CUDLR.Command("webgm", "设置WebGM标志，下次启动游戏是否自动开启webgm。ex:webgm on/off")]
        public static void SetWebGM(string[] args)
        {
            if (args.Length > 0)
            {
                var param = args[0];

                switch (param)
                {
                    case "on":
                        {
                            PlayerPrefsX.SetBool(WebGMWakeupListener.kWebGMAutoStart, true);
                            CUDLR.Console.Log("Done. WebGM service will auto start when game start");
                            break;
                        }
                    case "off":
                        {
                            PlayerPrefsX.SetBool(WebGMWakeupListener.kWebGMAutoStart, false);
                            CUDLR.Console.Log("Done. WebGM service will not auto start when game start");
                            break;
                        }
                    default:
                        {
                            CUDLR.Console.Log("invalid params");
                            break;
                        }
                }
            }
            else
            {
                CUDLR.Console.Log("invalid params");
            }

        }

        [CUDLR.Command("pathlist", "列出一些常用路径")]
        public static void ListCommonPaths(string[] args)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("paths:");
            sb.AppendLine("Application.persistentDataPath:" + Application.persistentDataPath);
            sb.AppendLine("Application.temporaryCachePath:" + Application.temporaryCachePath);
            sb.AppendLine("Application.streamingAssetsPath:" + Application.streamingAssetsPath);
            sb.AppendLine("...");
            sb.AppendLine("you can add more in 'WebGMCmds.cs > ListCommonPaths method.");

            CUDLR.Console.Log(sb.ToString());
        }
    }
}