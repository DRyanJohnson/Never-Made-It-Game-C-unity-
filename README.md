Never-Made-It-Game
The Programming involved with "Never made it"
Never Made It - Unity C# Scripts

This repository contains the C# source code for "Never Made It", a solo built visual novel developed in Unity for the Game Development at UWT Fall 2026 Game Jam.

Playable Web Build: https://tackytac.itch.io/never-made-it

Script Breakdown
StorySceneController.cs: Handles the core visual novel loop, including typewriter text progression, branching dialogue choices, scene transitions, and UI button states.

GameState.cs: Tracks player choices and persistent flags across scenes to determine narrative paths and endings.

MainMenuController.cs: Manages the title screen UI, scene loading into the starting platform scene, and menu audio feedback.

Backgroundmusic.cs: Implements a singleton audio manager using "DontDestroyOnLoad" to keep background music playing smoothly across scene transitions without duplicating audio sources.

CameraShake.cs: Triggers procedural camera shake effects when attacked during narrative moments.

ScrollingSignText.cs: Controls animated scrolling text for environmental signage.
