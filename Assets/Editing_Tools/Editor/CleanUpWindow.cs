using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CleanUpWindow : EditorWindow
{
	bool groupEnabled = true;
	List<string> usedAssets = new List<string>();
	List<string> includedDependencies = new List<string>();
	private Vector2 scrollPos;
	private List<Object> unUsed;
	
	private Dictionary<string, List<Object>> unUsedArranged;
	private Dictionary<string, List<bool>> toggles;

	private bool needToBuild = false;

	// Add menu named "CleanUpWindow" to the Window menu  
	[MenuItem("Window/CleanUpWindow")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:  
		CleanUpWindow window = (CleanUpWindow)EditorWindow.GetWindow(typeof(CleanUpWindow), true, "Project Cleanup", true);
		window.Show();
	}

	void OnGUI()
	{

		if (needToBuild)
		{
			GUI.color = Color.red;
			GUILayout.Label("Are you sure you remembered to build project? Because you really need to...", EditorStyles.boldLabel);
		}

		GUILayout.BeginHorizontal();
			if (!needToBuild)
			{
				if (GUILayout.Button("Clear EditorLog"))
				{   
					clearEditorLog();
					needToBuild = true;
				}
			}

			GUI.color = Color.green;
			if (GUILayout.Button("Load EditorLog"))
				loadEditorLog();

			GUI.color = Color.white;
		GUILayout.EndHorizontal();

		GUI.changed = false;

		if (!needToBuild)
		{
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(300));

				if (groupEnabled)
				{
					GUILayout.Label("DEPENDENCIES");
					for (int i = 0; i < includedDependencies.Count; i++)
					{
						EditorGUILayout.LabelField(i.ToString().Trim(), includedDependencies[i], GUILayout.MaxWidth(299));
					}
				}

				if(GUILayout.Button("Selected Objects"))
					PrintSelected();

				GUILayout.Space(10);

				GUI.color = Color.red;

				if(GUILayout.Button("Delete Selected", GUILayout.MinHeight(32)))
					DeleteSelected();

				GUI.color = Color.white;

			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();

GUILayout.BeginHorizontal();
			GUILayout.Label("All: ", EditorStyles.boldLabel, GUILayout.MaxWidth(64));

			if(GUILayout.Button("on",  EditorStyles.radioButton, GUILayout.MaxWidth(32)))
				ToggleAll(true);

			if(GUILayout.Button("off",  EditorStyles.radioButton, GUILayout.MaxWidth(32)))
				ToggleAll(false);
GUILayout.EndHorizontal();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			if (groupEnabled)
			{
				if (unUsedArranged != null)
				{
					foreach (KeyValuePair<string, List<Object>> objList in unUsedArranged)
					{
						if (objList.Value.Count >= 1)
						{
							GUILayout.Space(10);
							GUILayout.BeginHorizontal();
								if(GUILayout.Button("on",  EditorStyles.radioButton, GUILayout.MaxWidth(32)))
									ToggleKey(true, objList.Key.ToString());
								if(GUILayout.Button("off", EditorStyles.radioButton, GUILayout.MaxWidth(32)))
									ToggleKey(false, objList.Key);					

								GUILayout.Label(objList.Key, EditorStyles.boldLabel, GUILayout.MaxWidth(200));

							GUILayout.EndHorizontal();
							for (int i = 0; i < objList.Value.Count; i++)
							{
								if(objList.Value[i] != null) {
									GUILayout.BeginHorizontal();
										EditorGUILayout.ObjectField(objList.Value[i], typeof(Object), false, GUILayout.MinWidth(340));
										toggles[objList.Key][i] = EditorGUILayout.Toggle(toggles[objList.Key][i]);
									GUILayout.EndHorizontal();
								}
							}
						}
					}
				}
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
		}

	}

	void PrintSelected()
	{
		foreach(KeyValuePair<string, List<Object>> kvp in unUsedArranged)
		{
			for(int i = 0; i < toggles[kvp.Key].Count; i++)
			{
				if(toggles[kvp.Key][i] == true)
					Debug.Log("" + unUsedArranged[kvp.Key][i]);
			}
		}
	}

	void DeleteSelected()
	{
		if(!EditorUtility.DisplayDialog("Delete Unused Assets",
            "Are you sure you want to delete the selected assets?  There is no going back.", "Delete", "Aaagh, stop!"))
			return;

		foreach(KeyValuePair<string, List<Object>> kvp in unUsedArranged)
		{
			for(int i = 0; i < toggles[kvp.Key].Count; i++)
			{
				if(toggles[kvp.Key][i] == true) {
					AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath(unUsedArranged[kvp.Key][i]));
				}
			}
		}

		AssetDatabase.Refresh();

		loadEditorLog();
	}

	void ToggleAll(bool val)
	{
		foreach(KeyValuePair<string, List<bool>> kvp in toggles)
		{
			for(int i = 0; i < toggles[kvp.Key].Count; i++)
				toggles[kvp.Key][i] = val;
		}
	}

	void ToggleKey(bool val, string key)
	{
		for(int i = 0; i < toggles[key].Count; i++)
			toggles[key][i] = val;		
	}

	private void clearEditorLog()
	{
		
		string LocalAppData = string.Empty;
		string UnityEditorLogfile = string.Empty;

		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			LocalAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
			UnityEditorLogfile = LocalAppData + "\\Unity\\Editor\\Editor.log";
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			LocalAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			UnityEditorLogfile = LocalAppData + "/Library/Logs/Unity/Editor.log";
		}

		try
		{
			// Have to use FileStream to get around sharing violations!
			//System.IO.File.WriteAllText(UnityEditorLogfile, string.Empty);
			FileStream FS = new FileStream(UnityEditorLogfile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
			//StreamReader SR = new StreamReader(FS);
			StreamWriter SW = new StreamWriter(FS);
			
			SW.Write(string.Empty);
			SW.Flush();
			SW.Close();
		}
		catch (System.Exception E)
		{
			Debug.LogError("Error: " + E);
		}
	}

	private void loadEditorLog()
	{
		UsedAssets.GetLists(ref usedAssets, ref includedDependencies);

		if (usedAssets.Count == 0)
		{
			needToBuild = true;
		}
		else
		{
			compareAssetList(UsedAssets.GetAllAssets());
			groupEnabled = true;
			needToBuild = false;
		}
	}

	private void compareAssetList(string[] assetList)
	{

		unUsed = new List<Object>();

		unUsedArranged = new Dictionary<string, List<Object>>();
		unUsedArranged.Add("plugins", new List<Object>());
		unUsedArranged.Add("editor", new List<Object>());
		unUsedArranged.Add("unsorted", new List<Object>());
		unUsedArranged.Add("material", new List<Object>());
		unUsedArranged.Add("texture", new List<Object>());
		unUsedArranged.Add("model", new List<Object>());
		unUsedArranged.Add("prefab", new List<Object>());

		toggles = new Dictionary<string, List<bool>>();
		foreach(KeyValuePair<string, List<Object>> kvp in unUsedArranged)
		{
			toggles.Add(kvp.Key.ToString(), new List<bool>());
		}

		bool saveme = false;
		for (int i = 0; i < assetList.Length; i++)
		{
			if (!usedAssets.Contains(assetList[i]))
			{
				Object objToFind = AssetDatabase.LoadAssetAtPath(assetList[i], typeof(Object));
				unUsed.Add(objToFind);
				string key = getArrangedPos(objToFind);
				unUsedArranged[key].Add(objToFind);
				toggles[key].Add(false);			
			}
		}
	}

	private string getArrangedPos(Object value)
	{
		string path = AssetDatabase.GetAssetPath(value).ToLower();

		if (path.Contains("/plugins/"))
		{
			return "plugins";
		}
		else if (path.Contains("/editor/"))
		{
			return "editor";
		}
		else if (path.Contains(".prefab"))
		{
			return "prefab";
		}
		else if (path.Contains(".mat"))
		{
			return "material";
		}
		else if (path.Contains(".png") || path.Contains(".jpg") || path.Contains(".jpeg") || path.Contains(".psd") || path.Contains(".gif"))
		{
			return "texture";
		}        
		else if (path.Contains(".fbx") || path.Contains(".ma") || path.Contains(".mb") || path.Contains(".dae"))
		{
			return "model";
		}        
		else {
			return "unsorted";
		}
	}
}