# Gorilla Tag Map Project

A unity project to create your own maps for the [MonkeMapLoader](https://github.com/Vadix88/VmodMonkeMapLoader/) mod for Gorilla Tag. 

This guide is pretty long and in-depth, so if you have any questions please don't hesitate to join the [Gorilla Tag Modding Discord](http://discord.gg/b2MhDBAzTv)

## Setup
This project is made with Unity version 2019.3.15f. Higher or lower Unity versions may not work properly, so make sure to download it from the [Unity Archive](https://unity3d.com/get-unity/download/archive) if you don't have it already. It's recommended to use Unity Hub to make managing versions easier.

**MAKE SURE TO ADD ANDROID BUILD SUPPORT TO YOUR UNITY 2019.3.15F!** This is needed to make sure your bundles properly support quest. Instructions can be found here: https://docs.unity3d.com/Manual/android-sdksetup.html

## Prefabs
This project contains several prefabs meant to add functionality and make map-making easier. These prefabs have a physical appearance to make things easier for map makers. This physical appearance is removed on build, though, so don't worry about it ending up in the final map - it's all taken care of for you.

Here's a short list of what's included in `Assets/MapPrefabs`:
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
    - If enabled, using this teleporter will also infect the player
- Teleport Delay
    - Currently unused and not implemented.
    - Will ensure that the player is in the teleporter for at least x seconds.

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
## Exporting