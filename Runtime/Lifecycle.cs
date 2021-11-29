using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rehawk.Lifecycle
{
    public static class Lifecycle
    {
        private const string MULTISCENE_GAME_OBJECT_NAME = "Lifecyle - Multi-scene";

        private static readonly PollerRegister multiSceneRegister = new PollerRegister();
        private static readonly Dictionary<Scene, SceneData> scenePoller = new Dictionary<Scene, SceneData>();
        
        private static GameObject multiScenePollerObj;
        private static Scene currentScene;
        
        static Lifecycle()
        {
            // in case unity has hot-reloaded
            if (Application.isEditor)
            {
                multiScenePollerObj = GameObject.Find(MULTISCENE_GAME_OBJECT_NAME);
            }

            currentScene = SceneManager.GetActiveScene();
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnActiveSceneChanged(Scene previous, Scene current)
        {
            currentScene = current;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                currentScene = scene;
            }
        }

        private static void OnSceneUnloaded(Scene unloadedScene)
        {
            ResetScene(unloadedScene);
        }
        
        public static void ResetGlobal()
        {
            multiSceneRegister.Clear();
            
            if (!multiScenePollerObj)
            {
                CreateGlobalPoller();
            }
        }

        public static void ResetScene(Scene scene)
        {
            if (!scenePoller.ContainsKey(scene))
            {
                return;
            }
            
            Object.Destroy(scenePoller[scene].gameObject);
            scenePoller.Remove(scene);
        }
        
        private static void CreateGlobalPoller()
        {
            if (multiScenePollerObj != null)
            {
                return;
            }
            
            multiScenePollerObj = new GameObject(MULTISCENE_GAME_OBJECT_NAME);
            var poller = multiScenePollerObj.AddComponent<LifecyclePoller>();
            poller.Setup(multiSceneRegister);
            Object.DontDestroyOnLoad(multiScenePollerObj);
        }

        private static void CreateScenePoller(Scene scene)
        {
            if (scenePoller.ContainsKey(scene))
            {
                return;
            }

            var sceneData = new SceneData();
                
            GameObject pollerObj = new GameObject("Lifecycle - Scene: " + scene.name);
            SceneManager.MoveGameObjectToScene(pollerObj, scene);
            
            var poller = pollerObj.AddComponent<LifecyclePoller>();
            poller.Setup(sceneData.register);
            
            sceneData.gameObject = pollerObj;
            
            scenePoller.Add(scene, sceneData);
        }
        
        public static void Add<T>(T instance)
        {
            CreateGlobalPoller();
            multiSceneRegister.Add(instance);
        }
        
        public static void AddForScene<T>(T instance)
        {
            AddForScene(currentScene, instance);
        }

        public static void AddForScene<T>(Scene scene, T instance)
        {
            CreateScenePoller(scene);
            scenePoller[scene].register.Add(instance);
        }
        
        private class SceneData
        {
            public readonly PollerRegister register = new PollerRegister();
            public GameObject gameObject;
        }
    }
}