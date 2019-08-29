using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneShortCut : Editor
{
    private static string scenePath = Application.dataPath + "/Scenes/";

    private static string GetScenePath(SceneName scene)
    {
        return scenePath + scene.ToString() + ".unity";
    }

    [MenuItem("SceneShortCuts/MusicSelect")]
    static public void OpenMusicSelectScene()
    {
        //セーブ確認
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // シーン遷移
            EditorSceneManager.OpenScene(GetScenePath(SceneName.MusicSelect));
        }
    }

    [MenuItem("SceneShortCuts/Main")]
    static public void OpenMainScene()
    {
        //セーブ確認
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // シーン遷移
            EditorSceneManager.OpenScene(GetScenePath(SceneName.Main));
        }
    }

    [MenuItem("SceneShortCuts/Editor")]
    static public void OpenEditorScene()
    {
        //セーブ確認
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // シーン遷移
            EditorSceneManager.OpenScene(GetScenePath(SceneName.NotesEditor));
        }
    }
}