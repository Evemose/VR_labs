using System.Diagnostics.CodeAnalysis;
using Unity.Cinemachine.TargetTracking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prefabs;

namespace Movement.Editor
{
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
    public class Lab2SetupWindow : EditorWindow
    {
        private const string ShaderName = "Universal Render Pipeline/Lit";

        private const string CameraDataTypeName =
            "UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime";

        [MenuItem("Tools/Lab2/Setup Scene Helper")]
        public static void ShowWindow()
        {
            GetWindow<Lab2SetupWindow>("Lab2 Scene Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Lab2 Scene Setup Helper", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("1. Create Basic Scene Setup", GUILayout.Height(30)))
            {
                CreateBasicSceneSetup();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("2. Create Sample Collectibles", GUILayout.Height(30)))
            {
                CreateSampleCollectibles();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("3. Create Sample Obstacles", GUILayout.Height(30)))
            {
                CreateSampleObstacles();
            }
        }

        private static void CreateBasicSceneSetup()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.tag = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(10, 1, 10);

            var shader = Shader.Find(ShaderName);
            var groundMat = new Material(shader)
            {
                color = new Color(0.4f, 0.35f, 0.25f)
            };
            ground.GetComponent<Renderer>().material = groundMat;

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 1, 0);
            player.AddComponent<Movement>();

            var playerMat = new Material(shader)
            {
                color = new Color(0.3f, 0.5f, 0.8f)
            };
            player.GetComponent<Renderer>().material = playerMat;

            var mainCamera = Camera.main;
            if (mainCamera is null)
            {
                var cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";

                var cameraDataType = System.Type.GetType(CameraDataTypeName);
                if (cameraDataType != null)
                {
                    cameraObj.AddComponent(cameraDataType);
                }
            }
            else
            {
                var cameraDataType = System.Type.GetType(CameraDataTypeName);
                if (cameraDataType != null && mainCamera.GetComponent(cameraDataType) is null)
                {
                    mainCamera.gameObject.AddComponent(cameraDataType);
                }
            }

            if (mainCamera.GetComponent<Unity.Cinemachine.CinemachineBrain>() is null)
            {
                mainCamera.gameObject.AddComponent<Unity.Cinemachine.CinemachineBrain>();
            }

            var vcamObj = new GameObject("CM vcam1");
            var vcam = vcamObj.AddComponent<Unity.Cinemachine.CinemachineCamera>();
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;

            var follow = vcam.gameObject.AddComponent<Unity.Cinemachine.CinemachineFollow>();
            follow.FollowOffset = new Vector3(0, 1, -10);
            follow.TrackerSettings.BindingMode = BindingMode.LockToTargetWithWorldUp;
            follow.TrackerSettings.RotationDamping = new Vector3(1, 1, 1);
            follow.TrackerSettings.PositionDamping = new Vector3(1, 1, 1);

            vcam.gameObject.AddComponent<Unity.Cinemachine.CinemachineHardLookAt>();

            vcamObj.transform.position = new Vector3(0, 5, -10);

            Debug.Log("[Lab2 Setup] Cinemachine camera created!");

            if (FindFirstObjectByType<Light>() is null)
            {
                var light = new GameObject("Directional Light");
                var lightComp = light.AddComponent<Light>();
                lightComp.type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(50, -30, 0);
            }

            Selection.activeGameObject = player;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("[Lab2 Setup] Basic scene setup completed!");
        }

        private static void CreateSampleCollectibles()
        {
            var collectibleMat = new Material(Shader.Find(ShaderName))
            {
                color = new Color(0.9f, 0.7f, 0.2f)
            };

            for (var i = 0; i < 5; i++)
            {
                var collectible = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                collectible.name = $"Collectible_{i + 1}";
                collectible.tag = "Collectible";
                collectible.transform.position = new Vector3(Random.Range(-8f, 8f), 1, Random.Range(-8f, 8f));
                collectible.transform.localScale = Vector3.one * 0.5f;
                collectible.GetComponent<Renderer>().material = collectibleMat;

                var collider = collectible.GetComponent<SphereCollider>();
                collider.isTrigger = true;

                collectible.AddComponent<Collectible>();
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("[Lab1 Setup] Created 5 collectibles!");
        }

        private static void CreateSampleObstacles()
        {
            Material obstacleMat = new Material(Shader.Find(ShaderName))
            {
                color = new Color(0.7f, 0.3f, 0.25f)
            };

            for (var i = 0; i < 3; i++)
            {
                var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.name = $"Obstacle_{i + 1}";
                obstacle.tag = "Obstacle";
                obstacle.transform.position = new Vector3(Random.Range(-8f, 8f), 1, Random.Range(-8f, 8f));
                obstacle.transform.localScale = new Vector3(1, 2, 1);
                obstacle.GetComponent<Renderer>().material = obstacleMat;
                obstacle.AddComponent<Obstacle>();
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("[Lab1 Setup] Created 3 obstacles!");
        }
    }
}