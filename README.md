# Gorilla Tag Map Project

A unity project to create your own maps for the [MonkeMapLoader](https://github.com/Vadix88/VmodMonkeMapLoader/) mod for Gorilla Tag.

This guide is pretty long and in-depth, so make sure to read it thoroughly. If you have any questions please don't hesitate to join the [Gorilla Tag Modding Discord](https://discord.gg/b2MhDBAzTv) to ask.

**Contents**

  * [Setup](#setup)
  * [Creating a map](#creating-a-map)
  * [Spawn Points](#spawn-points)
  * [Matching Gorilla Tag's Style](#matching-gorilla-tags-style)
  * [Triggers](#triggers)
    + [TagZone](#tagzone)
    + [TeleporterZone](#teleporterzone)
    + [Teleporter](#teleporter)
    + [TeleporterTreehouse](#teleportertreehouse)
  * [Other Scripts](#other-scripts)
    + [Surface Climb Settings](#surface-climb-settings)
    + [Round End Actions](#round-end-actions)
    + [UnclimbableSurface](#unclimbablesurface)
  * [Exporting](#exporting)

## Setup
This project is made with Unity version 2019.3.15f. Higher or lower Unity versions may not work properly, so make sure to download it from the [Unity Archive](https://unity3d.com/get-unity/download/archive) if you don't have it already. It's recommended to use Unity Hub to make managing versions easier.

**MAKE SURE TO ADD ANDROID BUILD SUPPORT TO YOUR UNITY 2019.3.15F!** This is needed to make sure your bundles properly support quest. Instructions can be found here: https://docs.unity3d.com/Manual/android-sdksetup.html

You can download the latest version of this project on the [Releases tab](https://github.com/legoandmars/GorillaTagMapProject/releases).

## Creating a map
For the most part, creating a map itself is the same as creating anything in Unity. However, there are a few specific things that you'll need to do to ensure that the Map Loader can properly load it.

To load up the Unity project, go ahead and open up Unity or Unity Hub (Hub is recommended) and then click Open/Add and navigate to the downloaded + unzipped project. Navigate to the folder that contains the `Assets`, `Packages`, `ProjectSettings` folders, then click `Select Folder`.

If you don't see anything in your Unity Project, open the `Example Scene` found in the assets folder. You'll see an example map or two.

Feel free to take a look at how these example maps work, but for now, we're going to disable them by clicking each of them in the hierarchy then pressing the checkbox next to the GameObject's name on the right.


Create an empty GameObject that will hold everything in your map. Make sure the position is 0, and the scale is 1.

Next, click Add Component and add a Map Descriptor. This will hold some information on your map.

![image](https://user-images.githubusercontent.com/34404266/110260460-356fb280-7f61-11eb-9a2b-6e5e622c1563.png)

Here's what each setting does:
- Map name
    - Set this to what you want your map to be named
    - Try to make it unique, and avoid naming it something generic like just "Map"
- Author name
    - The map's author. Set this to your username.
- Description
    - A description that will show up when the map is being selected
    - Set this to whatever you want
- Spawn Points
    - A list of spawn points for the map. 
    - You shouldn't set this manually unless you know what you're doing
    - More Info in the [Spawn Points section](#spawn-points)
- Custom Skybox
    - A cubemap that will be used as the skybox on your map
    - If this empty, it'll automatically give your map the default game's skybox
- Gravity speed
    - The speed of gravity on your map
    - It's recommended to leave this at -9.8 unless you want to make maps with lower/higher gravity (eg, space maps)

## Spawn Points
If you want people to be able to teleport to your map you'll need to add some Spawn Points.

Under MapPrefabs, there's a `SpawnPoint` prefab. Drag one into your scene and make sure it's in your Map's GameObject.

You can position the Spawn Point anywhere you want. It's recommended that you put it a little bit above the ground/away from walls to prevent people from spawning inside of things.

_**Some important things to keep in mind:**_

The player is about as tall as the Spawn Point cube.

The max amount of players is 10, so it's a good idea to put multiple spawn points. You should have at least 4 in different locations - more if you set your map to respawn players at the ends of rounds.

## Matching Gorilla Tag's Style
Although not every custom map has to look exactly like the game, making your map look similar to the base game's visuals will help improve player experience, so here's a couple of tips:

To make your textures have the same low-poly PS2 style as Gorilla Tag, change the following settings:
- Filter Mode - Point (no filter) [This will ensure your textures aren't blurry]
- Max Size - 64/128/256 [This will depend on your texture's size and what you're using it on. Just do what looks like the base game]
- Compression - None [This will make sure that your images don't get garbled by compression]

![An image showing the best settings for Gorilla Tag textures](https://user-images.githubusercontent.com/34404266/110262890-e8440e80-7f69-11eb-870c-4445084b680e.png)

Additionally, if you want to make a model low poly you can add a Decimate modifier to it in Blender. Lower the threshold until the model looks low poly enough for you.

![Decimate Modifier in blender being applied](https://i.imgur.com/ijne3mw.gif)

Getting the art style exactly right can be hard, so make sure to join the [Gorilla Tag Modding Discord](https://discord.gg/b2MhDBAzTv) if you need help or want some textures other people have made.

## Triggers
This project contains several triggers meant to add functionality and make map-making easier. These triggers have a physical appearance to make things easier for map makers. This physical appearance is removed on build, though, so don't worry about it ending up in the final map - it's all taken care of for you.

There are 2 properties found on all Triggers:
- Touch Type 
    - What the player has to touch the trigger with to make it trigger
    - Valid values: Heads/Arms/Any
- Delay
    - How long the player has to be touching the trigger for it to trigger
    - Zero is useful for instant effects
    - Shorter values can be used for things that should have a bit of a delay
    - Longer values are useful for some teleporters to prevent accidental teleportation

Here's a short list of the types of Trigger Prefabs included in `Assets/MapPrefabs`:

### TagZone
When players go inside of a TagZone, they'll get tagged/infected.

To add a TagZone to your scene, simply drag and drop it into the Hierarchy from your `MapPrefabs` folder. You can have as many as you want and position/rescale them however you'd like.

### TeleporterZone
When players go inside of a TeleporterZone, they'll get teleported to a specific point/points.

**Script Options:**
- Teleport Points
    - A list of GameObjects that this teleporter can lead to. Make sure it's at least 1! 
    - If it's more than 1, it'll randomly pick a point from the list.
- Tag On Teleport
    - If enabled, using this teleporter will also infect the player.
    - Useful for out of bounds teleports or things like a lava pit that should infect and "respawn" you.

To add a TeleporterZone to your scene, simply drag and drop it into the Hierarchy from your `MapPrefabs` folder. You can have as many as you want and position/rescale them however you'd like.

You can use any object as a Teleporter Point, but it's highly recommended that you use some `MapPrefabs/TeleporterPoint`s to keep things organized. Simply:
- Drag a `TeleporterPoint` into the scene
- Position the `TeleporterPoint` where you'd like (It's recommended to put it a bit above the ground)
- Click back onto your `TeleporterZone`, then find your `TeleporterPoint` in the hierarchy and drag it onto the `Teleporter Points` field.

**MAKE SURE TO SET AT LEAST ONE TELEPORT POINT!** If you don't set a teleport point, the teleporter won't go anywhere!

### Teleporter
The same as a `TeleporterZone`, but with a fancy ingame teleporter attached. 

Make sure to follow the instructions for a `TeleporterZone!`

### TeleporterTreehouse
Similar to a `Teleporter` with fancy ingame visuals, but it teleports back to the treehouse. 

It's recommended that you put at least one `TeleporterTreehouse` in your map, preferably near the spawn point/points.

Make sure **NOT** to set teleport points for a `TeleporterTreehouse`, as doing so will make it not lead to the treehouse anymore.

## Other Scripts
Although most of the scripts/prefabs are Triggers, not all of them are. Here's some other scripts included with the project.

### Surface Climb Settings
If you want to modify how climbing works on an object, you can add a `Surface Climb Settings` script to it. 

**Script Options:**
- Unclimbable
    - If enabled, players will be unable to climb this surface.
- Slip percentage
    - A number that decides how "slippery" an object is.
    - Default value is 0.03. Higher values are more slippery, and lower values are less slippery.
    - Don't set this too high or below 0, as it can cause physics weirdness. 

### Round End Actions
An optional script that you can place on your Map's GameObject to control how the map behaves on round end.

**Script Options:**
- Respawn On Round End
    - If enabled, players will respawn whenever the game of tag is over.
- Objects To Enable
    - Put GameObjects here and they'll be force re-enabled when the round is over.
    - Useful if you have Object Triggers that disable objects, but you want to reset them on round end.
- Objects To Disable
    - Put GameObjects here and they'll be force disabled when the round is over.
    - Useful if you have Object Triggers that enable objects, but you want to reset them on round end.

### UnclimbableSurface
A surface that players can't climb. Useful for parkour courses and boundaries.

To add an `UnclimbableSurface` to your scene, simply drag and drop it into the Hierarchy from your `MapPrefabs` folder. You can have as many as you want and position/rescale them however you'd like.

Note that if you don't want to use the default material that's included with an `UnclimbableSurface`, you can swap it out. You can make any surface unclimbable by adding a `SurfaceClimbSettings` script to it and checking Unclimbable, so feel free to add it to existing objects to change their surface settings.

## Exporting

Once your map is all done, it's time to export! First, let's run through our checklist:

- Did you add Colliders to Objects that the player needs to collide with?
    - Make sure your colliders work relatively similar to have the mesh looks
    - Avoid using too many Mesh Colliders if possible (if you know what you're doing, you can always create lower poly versions of your meshes to use for the colliders)
- Did you completely fill out your descriptor?
    - Name, Author, Description, etc should all be filled out
- Did you add at least one Spawn Point to your map?
    - More is better - at least 4 in different spots are recommended
    - Brush back over the tips in the [Spawn Points section](#spawn-points)
- Did you add at least one `TeleporterTreehouse` to your map?
    - Not technically required, but without one players won't be able to get back to the treehouse 
    - Make sure your `TeleporterTreehouse`s are in an a semi-obvious spot so players can easily leave if they need to.
- If you're using Teleporters, double check that the Teleporter Points are set properly.
- Double check all of your Triggers to ensure that options are set properly.

If you wanted to use a custom skybox, import it into your Unity project as an image, set the `Texture Shape` to Cube, and set your map's `Custom Skybox` to it.

There's one more final step you'll need to do before exporting, and that's creating a `ThumbnailCamera`. Right click on your map's GameObject, then click `Camera` to add a camera. Rename this camera `ThumbnailCamera`.

Get your Scene view into a good position for a thumbnail (something that shows off your map and its main features), then click on the `ThumbnailCamera` and at the top of your screen click `GameObject/Align With View` (You can position the camera manually too, but this way is easier)

Now that your thumbnail is created, it's time to export! Go to `Window/Map Exporter` and click Export [your map name]. Alternatively, you can click on your map's GameObject and click `Export Map`.

Select the folder to export to (probably your Gorilla Tag's custom map folder at `Gorilla Tag/BepInEx/plugins/VmodMonkeMapLoader/CustomMaps/`) and click Save.

**You're all done!** Go test out your map ingame.

If your materials look washed out ingame, try changing these settings on them:
- Set Metallic to 0
- Set Smoothness to 0
- Turn off Specular Highlights
- Turn off Reflections

Once your map is done, join the [Gorilla Tag Modding Discord](https://discord.gg/b2MhDBAzTv) and share it so people can play!
