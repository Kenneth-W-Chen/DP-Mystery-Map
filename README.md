# About

This is a Unity 2D game. The purpose of this game is to help a user learn about the layout of the University of North Texas building, [Discovery Park](https://engineering.unt.edu/about/discovery-park).

A detailed map of the building can be found [here](https://engineering.unt.edu/sites/default/files/Discovery_Park_with_BMEN_addition.pdf).

Discovery Park is located at 3940 N. Elm St., Denton, TX 76207.

# Environmental set-up

*This is a guide on how to set up the Unity project to contribute. If you want to play the game, please head to the [releases page](https://github.com/Kenneth-W-Chen/DP-Mystery-Map/releases).*

## Requirements

* [Unity Hub](https://unity.com/download)
* Unity Editor 2022.3.9f1
* A Unity license (personal is good)
* About 1-2 GB of disk space available
* A system that can run Unity Editor 2022.3.9f1


### Unity Editor Downloads

If 2022.3.9f1 is available in the *Installs* tab in Unity Hub, then download from there. Otherwise, use one of these download links:

[Unity Editor for Windows](https://download.unity3d.com/download_unity/ea401c316338/Windows64EditorInstaller/UnitySetup64-2022.3.9f1.exe)

[Unity Editor for MacOS \(Intel\)](https://download.unity3d.com/download_unity/ea401c316338/MacEditorInstaller/Unity.pkg)

[Unity Editor for MacOS \(Apple silicon\)](https://download.unity3d.com/download_unity/ea401c316338/MacEditorInstallerArm64/Unity.pkg)

[Unity Editor for Linux](https://download.unity3d.com/download_unity/ea401c316338/UnitySetup-2022.3.9f1)

## Set-up

Download the GitHub repository. If you do not have a Git client installed, use this [link](https://github.com/Kenneth-W-Chen/DP-Mystery-Map/archive/refs/heads/main.zip) and unzip the file somewhere. If you use [Git Bash](https://git-scm.com/downloads), clone the repository with that using:

```bash
git clone git@github.com:Kenneth-W-Chen/DP-Mystery-Map.git
```

Open Unity Hub. In the *Projects* tab, press *Open*. Locate the git repository location. The repository should have a folder structure like this:

```
DP-Mystery-Map
├── DP-Mystery-Map
│   ├── Assets
│   ├── Packages
│   ├── ProjectSettings
│   └── .gitignore
└── .git
```
Open the second DP-Mystery-Map folder (i.e., the parent folder of Assets, Packages, and ProjectSettings).

Unity will initialize the project for you.

# Folder structure

* Scripts and code are located in `DP-Mystery-Map/DP-Mystery-Map/Assets/Scripts/`
* Scenes are located in `DP-Mystery-Map/DP-Mystery-Map/Assets/Scenes/`
* Sprites and images are located in `DP-Mystery-Map/DP-Mystery-Map/Assets/Sprites/`
* Prefabs are located in `DP-Mystery-Map/DP-Mystery-Map/Assets/Respurces/Prefabs/`
