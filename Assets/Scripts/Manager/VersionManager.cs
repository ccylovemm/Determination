using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//  Created by CaoChunYang 

public class VersionManager : MonoBehaviour
{
	static public string VersionFile = "Version.txt";
    static public string Latest_VersionFile = "Latest_Version.txt";

	static public string LocalResUrl{
		get{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR  
			return "file://" + Application.dataPath + "/StreamingAssets/";
			#elif UNITY_IPHONE
			return "file://" + Application.dataPath + "/Raw/";
			#elif UNITY_ANDROID
			return "jar:file://" + Application.dataPath + "!/assets/";
			#endif
		}
	}

	static public string LatestResUrl{
		get{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR  
			return Application.persistentDataPath + "/";   
			#elif UNITY_IPHONE
			return Application.persistentDataPath + "/";   
			#elif UNITY_ANDROID
			return Application.persistentDataPath + "/"; 
			#endif
		}
	}

	static public string ServerResUrl = "";     

	[HideInInspector]
	public EventDelegate.Callback CheckVersionFinish;

	static private Dictionary<string , string> LocalResVersion = new Dictionary<string, string>();   //本地所有最新资源 版本  用于远程比较
	static private Dictionary<string , string> LatestResVersion = new Dictionary<string, string>();  //本地所有安装后更新资源 版本 用于查找本地最新资源(未包括本次登录更新)
	static private Dictionary<string , string> ServerResVersion = new Dictionary<string, string>();  //远程最新资源 版本 
	static private Dictionary<string , string> UpdateResVersion = new Dictionary<string, string>();  //本次登录更新资源 版本 用于查找本地最新资源

	private List<string> NeedDownFiles = new List<string>();  

	static public string GetConfigPath(string filePath)
	{
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR 
		return Application.persistentDataPath + "/Config/" + filePath;
		#else
		if(UpdateResVersion.ContainsKey(filePath) || LatestResVersion.ContainsKey(filePath) || LocalResVersion.ContainsKey(filePath))
		{
			return LatestResUrl + "Config/" + filePath;
		}
		return ServerResUrl + "Config/" + filePath;
        #endif
    }

	static public string GetResPath(string filePath)
	{
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR 
		return "file://" + Application.streamingAssetsPath + "/AssetBundle/" + filePath;
		#else
		if(UpdateResVersion.ContainsKey(filePath) || LatestResVersion.ContainsKey(filePath))
		{
			return "file://" + LatestResUrl + "AssetBundle/" + filePath;
		}
		if(LocalResVersion.ContainsKey(filePath))
		{
			return LocalResUrl + "AssetBundle/" + filePath; 
		}
		return ServerResUrl + "AssetBundle/" + filePath;
        #endif
    }
	
	public void CheckVersion(EventDelegate.Callback OnFinish)    
	{     
		CheckVersionFinish = OnFinish;
        LoadVersion();
	}

	void LoadVersion()
	{
        StartCoroutine(DownLoad("file://" + LocalResUrl + VersionFile, delegate(WWW localVersion)
        {
            ParseVersionFile(localVersion.text, LocalResVersion);   
            if (File.Exists(LatestResUrl + Latest_VersionFile))
            {
                StartCoroutine(DownLoad("file://" + LatestResUrl + Latest_VersionFile, delegate(WWW latestVersion)
                {
                    ParseVersionFile(latestVersion.text, LatestResVersion);
                    if (int.Parse(LocalResVersion["VersionNumber"]) > int.Parse(LatestResVersion["VersionNumber"]))
                    {
                        Directory.Delete(LatestResUrl);
                        LatestResVersion.Clear();
                        LoadVersion_();
                    }
                    else
                    {
                        StartCoroutine(DownLoad("file://" + LatestResUrl + VersionFile, delegate(WWW localVersion_)
                        {
                            ParseVersionFile(localVersion_.text, LocalResVersion);
                            LoadVersion_();
                        }));
                    }
                }));
            }
            else
            {
                if(Directory.Exists(LatestResUrl))
                {
                     Directory.Delete(LatestResUrl);
                }
                LoadVersion_();
            }
        }));
	}

	void LoadVersion_()
	{
		StartCoroutine(this.DownLoad(ServerResUrl + VersionFile , delegate(WWW serverVersion)      
        {
			ParseVersionFile(serverVersion.text, ServerResVersion);  
			if(int.Parse(LocalResVersion["VersionNumber"]) > int.Parse(ServerResVersion["VersionNumber"]))
			{
                UpdateLocalVersionFile();
				CheckVersionFinish ();
			}
			else
			{
				CompareVersion();   
				DownLoadRes();    
			}
		}));      
	}
	  
	void DownLoadRes()    
	{    
		if (NeedDownFiles.Count == 0)    
		{    
			UpdateLocalVersionFile();    
			return;    
		}    

		string file = NeedDownFiles[0];    
		NeedDownFiles.RemoveAt(0);    

		StartCoroutine(this.DownLoad(ServerResUrl + file, delegate(WWW w)    
      	{      
			ReplaceLocalRes(LatestResUrl + file, w.bytes);   
			DownLoadRes();    
		}));    
	}    

	void UpdateLocalVersionFile()    
	{ 
		StringBuilder versions = new StringBuilder();    
		foreach (var item in ServerResVersion)    
		{    
			versions.Append(item.Key).Append("|").Append(item.Value).Append("\n");    
		}    
		SaveVersionFile(LatestResUrl + VersionFile , versions);      

		StringBuilder versions_ = new StringBuilder();    
		foreach (var item in LatestResVersion)    
		{    
			if(!UpdateResVersion.ContainsKey(item.Key))
			{
				versions_.Append(item.Key).Append("|").Append(item.Value).Append("\n");    
			}
		}   
		foreach (var item in UpdateResVersion)    
		{    
			versions_.Append(item.Key).Append("|").Append(item.Value).Append("\n");    
		}   
		SaveVersionFile(LatestResUrl + VersionFile , versions_);  
		CheckVersionFinish ();
	}  

	void ReplaceLocalRes(string path, byte[] data)    
	{
		Debug.Log(path);
		path = path.Replace("file://" , "");
		string directoryName = path.Substring(0 , path.LastIndexOf("/"));
		if(!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if(File.Exists(path))
		{
			File.Delete(path);
		}  
		FileStream stream = File.Create(path);
		stream.Write(data, 0, data.Length);    
		stream.Flush();    
		stream.Close();    
	}     

	void SaveVersionFile(string path , StringBuilder versions)
	{
		Debug.Log(path);
		path = path.Replace("file://" , "");
		string directoryName = path.Substring(0 , path.LastIndexOf("/"));
		if(!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if(File.Exists(path))
		{
			File.Delete(path);
		}  
		FileStream stream = new FileStream(path, FileMode.Create);      
		byte[] data = Encoding.ASCII.GetBytes(versions.ToString());    
		stream.Write(data, 0, data.Length);    
		stream.Flush();    
		stream.Close(); 
	}
	
	void CompareVersion()    
	{    
		foreach (var version in ServerResVersion)    
		{    
			string fileName = version.Key;    
			string serverMd5 = version.Value;   
			if(fileName == "VersionNumber" || fileName == "FileCount" || fileName == "Version.txt")continue;
			if (!LocalResVersion.ContainsKey(fileName))    
			{    
				NeedDownFiles.Add(fileName);    
				UpdateResVersion.Add(fileName , serverMd5);
			}    
			else    
			{    
				string localMd5;    
				LocalResVersion.TryGetValue(fileName, out localMd5);    
				if (!serverMd5.Equals(localMd5))    
				{    
					NeedDownFiles.Add(fileName);   
					UpdateResVersion.Add(fileName , serverMd5);
				}    
			}    
		}     
	}    
	
	void ParseVersionFile(string content, Dictionary<string , string> dict)    
	{    
		dict.Clear();
		if (content == null || content.Length == 0)    
		{    
			return;    
		}    
		string[] items = content.Split('\n');    
		foreach (string item in items)    
		{    
			string[] info = item.Split('|');    
			if (info != null && info.Length == 2)    
			{    
				if(info[0].IndexOf("VersionNumber") != -1)
				{
					dict.Add("VersionNumber", info[1]);    
				}
				else
				{
					if(dict.ContainsKey(info[0]))
					{
						Debug.Log("有相同的Key " + info[0]);
					}
					else
					{
						dict.Add(info[0], info[1]);   
					}
				}
			}    
		}    
	}    
	
	IEnumerator DownLoad(string url, HandleFinishDownload finishFun)    
	{    
		Debug.Log("DownLoad " + url);
		WWW www = new WWW(url);    
		yield return www;    
		if (finishFun != null)    
		{    
			finishFun(www);    
		}    
		www.Dispose();    
	}    
	
	public delegate void HandleFinishDownload(WWW www);

    static private VersionManager inst_;
    static public VersionManager Inst
    {
        get
        {
            if (inst_ == null)
            {
                inst_ = new VersionManager();
            }
            return inst_;
        }
    }
} 