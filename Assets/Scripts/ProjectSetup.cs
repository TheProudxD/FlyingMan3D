using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static System.Environment;
using static System.IO.Path;

public static class ProjectSetup
{
    [MenuItem("Tools/Setup/Import Essential Assets")]
    public static void ImportEssentials()
    {
        Assets.ImportAsset("Fast Script Reload.unitypackage", "Chris Handzlik/Editor ExtensionsUtilities");
        Assets.ImportAsset("Better Hierarchy.unitypackage", "Toaster Head/Editor ExtensionsUtilities");
        Assets.ImportAsset("Joystick Pack.unitypackage", "Fenerax Studios/ScriptingInput - Output");
        Assets.ImportAsset("Autosaver.unitypackage", "Sixpolys/Editor ExtensionsUtilities");
        Assets.ImportAsset("Missing Script Checker.unitypackage", "LLS/Editor ExtensionsUtilities");
        Assets.ImportAsset("WebGL optimizer.unitypackage", "CrazyGames/Editor ExtensionsUtilities");
        //Assets.ImportAsset("NaughtyAttributes.unitypackage", "Denis Rizov/Editor ExtensionsUtilities");
        Assets.ImportAsset("PlayerPrefs Editor.unitypackage", "BG Tools/Editor ExtensionsUtilities");
        Assets.ImportAsset("Audio Preview Tool.unitypackage", "Warped Imagination/Editor ExtensionsAudio");
        //Assets.ImportAsset("FlexyAssetRefs.unitypackage", "FlexyTools/Editor ExtensionsUtilities");
    }

    [MenuItem("Tools/Setup/Install Essential Packages")]
    public static void InstallEssentialPackages()
    {
        Packages.InstallPackages(new[]
        {
            //"git+https://github.com/handzlikchris/FastScriptReload.git#upm",
            "git+https://github.com/KyleBanks/scene-ref-attribute.git",
            "git+https://github.com/Thundernerd/Unity3D-SerializableInterface.git",
            "git+https://github.com/leth4/Colorlink.git",
            "git+https://github.com/gustavopsantos/reflex.git?path=/Assets/Reflex/",
            "git+https://github.com/dbrizov/NaughtyAttributes.git#upm",
            "git+https://github.com/antonysze/unity-custom-play-button.git",
            "git+https://github.com/annulusgames/LitMotion.git?path=src/LitMotion/Assets/LitMotion",
            "com.unity.recorder",
            //"com.unity.textmeshpro",
            "com.unity.ide.rider",
            "com.unity.device-simulator.devices",
            // If necessary, import new Input System last as it requires a Unity Editor restart
            // "com.unity.inputsystem"
        });
    }

    [MenuItem("Tools/Setup/Install UniTask")]
    public static void InstallUniTask()
    {
        Packages.InstallPackages(new[]
        {
            "git+https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
        });
    }

    [MenuItem("Tools/Setup/Install Post Processing")]
    public static void InstallPostProcessing()
    {
        Packages.InstallPackages(new[]
        {
            "com.unity.postprocessing"
        });
    }

    [MenuItem("Tools/Setup/Install Yandex Plugin")]
    public static void InstallYandexPlugin() =>
        Assets.ImportAsset("Plugin Your Games 20.unitypackage", "Maximalist/ScriptingIntegration");

    [MenuItem("Tools/Setup/Create Folders")]
    public static void CreateFolders()
    {
        Folders.Create("_Project", "Animation", "Art/UI", "Art/Fonts", "Art/Animations/AnimationClips",
            "Art/Animations/Animators", "Art/Models", "Art/Sprites",
            "Art/Shaders",
            "Art/Materials", "Audio/AudioClips", "PhysicMaterials", "Prefabs", "Scripts/Tests", "Scripts/Tests/Editor",
            "Scripts/Tests/Runtime");

        Folders.Create("Resources");
        Folders.Create("Plugins");
        Folders.Create("Settings");

        AssetDatabase.Refresh();
        Folders.Move("_Project", "Scenes");
        Folders.Move("_Project", "Settings");
        Folders.Delete("TutorialInfo");
        AssetDatabase.Refresh();

        AssetDatabase.MoveAsset("Assets/InputSystem_Actions.inputactions",
            "Assets/_Project/Settings/InputSystem_Actions.inputactions");

        AssetDatabase.DeleteAsset("Assets/Readme.asset");
        AssetDatabase.Refresh();

        // Disable Domain Reload
        EditorSettings.enterPlayModeOptions =
            EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
    }

    private static class Assets
    {
        public static void ImportAsset(string asset, string folder)
        {
            string basePath;

            if (OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            {
                string homeDirectory = GetFolderPath(SpecialFolder.Personal);
                basePath = Combine(homeDirectory, "Library/Unity/Asset Store-5.x");
            }
            else
            {
                string defaultPath = Combine(GetFolderPath(SpecialFolder.ApplicationData), "Unity");
                basePath = Combine(EditorPrefs.GetString("AssetStoreCacheRootPath", defaultPath), "Asset Store-5.x");
            }

            asset = asset.EndsWith(".unitypackage") ? asset : asset + ".unitypackage";

            string fullPath = Combine(basePath, folder, asset);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The asset package was not found at the path: {fullPath}");
            }

            AssetDatabase.ImportPackage(fullPath, false);
        }
    }

    private static class Packages
    {
        private static AddRequest request;
        private static Queue<string> packagesToInstall = new Queue<string>();

        public static void InstallPackages(string[] packages)
        {
            foreach (var package in packages)
            {
                packagesToInstall.Enqueue(package);
            }

            if (packagesToInstall.Count > 0)
            {
                StartNextPackageInstallation();
            }
        }

        private static async void StartNextPackageInstallation()
        {
            string identifier = packagesToInstall.Dequeue();
            var search = Client.SearchAll();

            while (!search.IsCompleted)
                await Task.Delay(10);

            foreach (var packageInfo in search.Result)
            {
                Debug.Log(packageInfo.git);
            }

            if (search.Status != StatusCode.Success)
            {
                request = Client.Add(identifier);
                Debug.Log("Installing: " + identifier);

                while (!request.IsCompleted)
                    await Task.Delay(10);

                if (request.Status == StatusCode.Success) Debug.Log("Installed: " + request.Result.packageId);
                else if (request.Status >= StatusCode.Failure) Debug.LogError(request.Error.message);
            }

            if (packagesToInstall.Count > 0)
            {
                await Task.Delay(1000);
                StartNextPackageInstallation();
            }
        }
    }

    private static class Folders
    {
        public static void Create(string root, params string[] folders)
        {
            var fullpath = Combine(Application.dataPath, root);

            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }

            foreach (var folder in folders)
            {
                CreateSubFolders(fullpath, folder);
            }
        }

        private static void CreateSubFolders(string rootPath, string folderHierarchy)
        {
            var folders = folderHierarchy.Split('/');
            var currentPath = rootPath;

            foreach (var folder in folders)
            {
                currentPath = Combine(currentPath, folder);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        public static void Move(string newParent, string folderName)
        {
            var sourcePath = $"Assets/{folderName}";

            if (AssetDatabase.IsValidFolder(sourcePath))
            {
                var destinationPath = $"Assets/{newParent}/{folderName}";
                var error = AssetDatabase.MoveAsset(sourcePath, destinationPath);

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to move {folderName}: {error}");
                }
            }
        }

        public static void Delete(string folderName)
        {
            var pathToDelete = $"Assets/{folderName}";

            if (AssetDatabase.IsValidFolder(pathToDelete))
            {
                AssetDatabase.DeleteAsset(pathToDelete);
            }
        }
    }
}