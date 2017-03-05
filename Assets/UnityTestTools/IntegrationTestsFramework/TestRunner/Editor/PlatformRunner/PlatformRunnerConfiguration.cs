using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PlatformRunnerConfiguration
{
    public string[] scenes = new string[0];
    public BuildTarget buildTarget;
    public bool runInEditor;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
	public string projectName = EditorApplication.currentScene;
#else
	public string projectName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
#endif

    public string resultsDir = null;
    public bool sendResultsOverNetwork;
    public List<string> ipList;
    public int port;

    public PlatformRunnerConfiguration(BuildTarget buildTarget)
    {
        this.buildTarget = buildTarget;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		projectName = EditorApplication.currentScene;
#else
		projectName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
#endif
    }

    public PlatformRunnerConfiguration()
        : this(BuildTarget.StandaloneWindows)
    {
    }

    public string GetTempPath()
    {
        if (string.IsNullOrEmpty(projectName))
            projectName = Path.GetTempFileName();

        var path = Path.Combine("Temp", projectName);
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return path + ".exe";
            case BuildTarget.StandaloneOSXIntel:
                return path + ".app";
            case BuildTarget.Android:
                return path + ".apk";
            default:
                if (buildTarget.ToString() == "BlackBerry" || buildTarget.ToString() == "BB10")
                    return path + ".bar";
                return path;
        }
    }

    public string[] GetConnectionIPs()
    {
        return ipList.Select(ip => ip + ":" + port).ToArray();
    }

    public static int TryToGetFreePort()
    {
        var port = -1;
        try
        {
            var l = new TcpListener(IPAddress.Any, 0);
            l.Start();
            port = ((IPEndPoint)l.Server.LocalEndPoint).Port;
            l.Stop();
        }
        catch (SocketException e)
        {
            Debug.LogException(e);
        }
        return port;
    }
}
