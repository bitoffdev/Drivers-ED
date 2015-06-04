using UnityEngine;

public class SceneLoader : MonoBehaviour {
	public string SceneName;

	void Load(){
		Application.LoadLevel (SceneName); 
	}
}
