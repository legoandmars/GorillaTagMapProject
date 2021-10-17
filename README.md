# Gorilla Tag Map Project

A unity project to create your own maps for the [MonkeMapLoader](https://github.com/Vadix88/MonkeMapLoader/) mod for Gorilla Tag.

This guide is pretty long and in-depth, so make sure to read it thoroughly. If you have any questions please don't hesitate to join the [Gorilla Tag Modding Discord](https://discord.gg/b2MhDBAzTv) to ask.

**Contents**

  * [Setup](#setup)
  * [Creating a map](#creating-a-map)
  * [Spawn Points](#spawn-points)
  * [Matching Gorilla Tag's Style](#matching-gorilla-tags-style)
  * [Lighting](#lighting)
    + [Lighting Setup](#lighting-setup)
    + [Lighting Meshes](#lighting-meshes)
    + [Lights](#lights)
    + [Other Tips](#other-tips)
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
  * [Custom Data](#custom-data)
    * [For Developers](#for-developers)

## Setup
This project is made with Unity version 2019.3.15f. Higher or lower Unity versions may not work properly, so make sure to download it from the [Unity Archive](https://unity3d.com/get-unity/download/archive) if you don't have it already. It's recommended to use Unity Hub to make managing versions easier.

**MAKE SURE TO ADD ANDROID BUILD SUPPORT TO YOUR UNITY 2019.3.15F!** This is needed to make sure your bundles properly support quest. Instructions can be found here: https://docs.unity3d.com/Manual/android-sdksetup.html

You can download the latest version of this project on the [Releases tab](https://github.com/legoandmars/GorillaTagMapProject/releases).

## Creating a map
For the most part, creating a map itself is the same as creating anything in Unity. However, there are a few specific things that you'll need to do to ensure that the Map Loader can properly load it.

To load up the Unity project, go ahead and open up Unity or Unity Hub (Hub is recommended) and then click Open/Add and navigate to the downloaded + unzipped project. Navigate to the folder that contains the `Assets`, `Packages`, and `ProjectSettings` folders, then click `Select Folder`.

If you don't see anything in your Unity Project, open the `Example Scene` found in the assets folder. You'll see an example map or two.

Feel free to take a look at how these example maps work, but for now, we're going to disable them by clicking each of them in the hierarchy then pressing the checkbox next to the GameObject's name on the right.


Create an empty GameObject that will hold everything in your map. Make sure the position is 0, and the scale is 1.

Next, click Add Component and add a Map Descriptor. This will hold some information on your map.

![Map Descriptor in Unity](https://user-images.githubusercontent.com/33105645/137638265-6e00fe42-7a30-4c6e-936f-a1aa241438f7.png)

Here's what each setting does:
- Map name
    - Set this to what you want your map to be named
    - Try to make it unique, and avoid naming it something generic like just "Map"
- Author name
    - The map's author. Set this to your username.
- Description
    - A description that will show up when the map is being selected
    - Set this to whatever you want
- Gravity speed
    - The speed of gravity on your map
    - It's recommended to leave this at -9.8 unless you want to make maps with lower/higher gravity (eg, space maps)
- Slow Jump Limit
    - The fastest speed that a player can reach when they are a survivor.
    - It's recommended to leave this at 6.5 unless you want survivors or casual maps to have a different move speed.
- Fast Jump Limit
    - The fastest speed that a player can reach when they are infected.
    - It's recommended to leave this at 8.5 unless you want infected players to have a different move speed.
- Slow Jump Multiplier
    - The jump multiplier that will be applied when a player is a survivor. This multiplier is used on the arm speed when a jump is made.
    - It's recommended to leave this at 1.1 unless you want survivors or casual maps to have a different move speed.
- Fast Jump Multiplier
    - The jump multiplier that will be applied when a player is infected. This multiplier is used on the arm speed when a jump is made.
    - It's recommended to leave this at 1.3 unless you want infected players to have a different move speed.
- Reset Properties
    - Resets the player settings (gravity speed, slow and fast jump limits and multipliers) to their default values.
    - It's recommended to click this if you think you may have accidentally altered any of the player settings
- Spawn Points
    - A list of spawn points for the map. 
    - You shouldn't set this manually unless you know what you're doing
    - More Info in the [Spawn Points section](#spawn-points)
- Custom Skybox
    - A cubemap that will be used as the skybox on your map
    - If this empty, it'll automatically give your map the default game's skybox
- Export Lighting
    - Whether or not to generate lightmaps for your map
    - Please read the [Lighting Section](#lighting) for more information.
- Required PC Mods Id
    - A list of mod ids that the user must have installed to load the map on PC.
- Required Quest Mods Id
    - A list of mod ids that the user must have installed to load the map on Quest.
- Game Mode
    - The game mode that the map will be
    - Default is normal infection, and casual has nobody infected, with tagging disabled. With casual mode selected, all player speeds are set to the slow speeds, and fast speeds are hidden.
- Thumbnail Preview
    - A preview of what the map thumbnail will look like
    - Click on `Refresh Preview` to manually update the preview

## Spawn Points
If you want people to be able to teleport to your map you'll need to add some Spawn Points.

Under MapPrefabs, there's a `SpawnPoint` prefab. Drag one into your scene and make sure it's in your Map's GameObject.

You can position the Spawn Point anywhere you want. It's recommended that you put it a little bit above the ground/away from walls to prevent people from spawning inside of things.

_**Some important things to keep in mind:**_

The player is about as large as the Spawn Point cube - 1 Unity unit wide/tall/long

The max amount of players is 10, so it's a good idea to put multiple spawn points. You should have at least 4 in different locations - more if you set your map to respawn players on map end.

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

## Lighting
An important part of making a map look good is the lighting. Since Gorilla Tag bakes lighting, the process to get it working is a bit involved, but it's absolutely worth it.

**When you SHOULD use lighting:**
- Most maps
- Complex maps with many objects
- Maps that need shadows
- Maps with reflections

**When you SHOULDN'T use lighting:**
- Maps that consist of mostly Unlit shaders
    - For example, N64 maps, minecraft maps, etc
- Maps where shadows aren't important
- Maps that really need to save filesize

If your map falls under the "SHOULDN'T USE LIGHTING" category, you can set your map's `Export Lighting` value to false and ignore the rest of this section.

Otherwise, follow these steps to getting lighting looking nice on your map:

### Lighting Setup
Make sure to set your map's `Export Lighting` value to true.

Click on your map's GameObject, and set the `Static` value next to the name in the properties window to true.

When ask if you'd like to enable the static flags for all the child objects, click `Yes, change children`

Every object on your map should be static EXCEPT for:
- Objects that have an animation
- Objects that will somehow change or get enabled/disabled, such as a trigger
- Objects that should not have shadows

### Lighting Meshes
When you're initially importing a mesh, go to the properties and make sure that the `Generate Lightmap UVs` box is checked. 

Go through all of your imported meshes now and make sure it's enabled for **all of them!** (unless you know what you're doing and have applied Lightmap UVs in an external program)

Next, go to each object with a `Mesh Renderer` in the scene, and ensure that `Contribute Global Illumination` is enabled. If you want to disable an object Receiving/Casting shadows, mess with the `Cast Shadows` and `Receive Shadows` properties - otherwise, leave them as the default values.

### Lights
Your map includes a `Directional Light` by default. Don't remove this unless you know what you're doing, as it (pretty accurately) recreates the base game lighting.

You can add any other sort of `Light` to your map that you want, but ensure that the type is set to `Baked`.

### Other Tips
Map compile time when baking lighting for the first time may be high. There's not much of a workaround here, so just wait for it to finish. Subsequent exports will be significantly faster.

If your map is too big or laggy after adding lighting, you can change these values in Window/Rendering/Lighting Settings:
- Lightmap Resolution (Default 32) - Change to 16 or 8
- Lightmap Size (Default 512) - Change to 256 or 128

By default, your map preview in-editor won't have shadows or proper lighting. If you want a preview of how it looks, go to `Window/Rendering/Lighting Settings` and click `Generate Lighting` in the bottom right. If you want to get rid of the baked preview data, click the little arrow next to `Generate Lighting` and click `Clear Baked Data.`

If your map is looking too light or you want to play around with how the lighting works, try adjusting the intensity of the included `Directional Light.`

If some materials look washed out ingame, try changing these settings on those materials:
- Set Metallic to 0
- Set Smoothness to 0
- Turn off Specular Highlights
- Turn off Reflections


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
- Did you read over the [lighting section](#lighting) and follow all the steps?
- If you're using Teleporters, double check that the Teleporter Points are set properly.
- Double check all of your Triggers to ensure that options are set properly.

If you wanted to use a custom skybox, import it into your Unity project as an image, set the `Texture Shape` to Cube and set your map's `Custom Skybox` to it.

There's one more final step you'll need to do before exporting, and that's creating a `ThumbnailCamera`. Right click on your map's GameObject, then click `Camera` to add a camera. Rename this camera `ThumbnailCamera`.

Get your Scene view into a good position for a thumbnail (something that shows off your map and its main features), then click on the `ThumbnailCamera` and at the top of your screen click `GameObject -> Align With View` (You can position the camera manually too, but this way is easier)

Now that your thumbnail is created, it's time to export! Go to `Window/Map Exporter` and click Export [your map name]. Alternatively, you can click on your map's GameObject and click `Export Map`.

Select the folder to export to. 

- If you play on PC, this will probably your Gorilla Tag's custom map folder at `Gorilla Tag/BepInEx/plugins/MonkeMapLoader/CustomMaps/`

- If you're on quest, just save the map to a random folder temporarily (like your desktop) and then drag it into QuestPatcher to install it.

Click save, and **you're all done!** Go test out your map ingame.

If your map doesn't look quite right in game, read back over the [Lighting section](#lighting)

Once your map is done, join the [Gorilla Tag Modding Discord](https://discord.gg/b2MhDBAzTv) and share it in the beta channels so people can test it out! 

After you've had some other people try it out, feel free to upload it to [Monke Map Hub](https://monkemaphub.com) so more people can download it.

## Custom Data

Some mods build upon the map loader to add extra functionality. Supporting these mods in your map requires you to add their descriptors to your map. If the descriptor is not already in the map project, you may need to import a script from the mod developer into the Scripts folder. Any descriptors you add must be attached to the same gameobject as the Map Descriptor.

### For Developers

In order to add your own descriptor to a map, implement the IBuildEvents interface in your class:

```cs
using VmodMonkeMapLoader.Behaviours

public class MyModDescriptor : MonoBehaviour, IBuildEvents
{
    public void OnBuild(MapDescriptor mapDescriptor) {
        // Set the gravity speed
        mapDescriptor.GravitySpeed = 0;

        // Add to the custom data
        mapDescriptor.CustomData.Add("myKey", "myValue");
        mapDescriptor.CustomData.Add("myList", [1,2,3]);

        // Require a mod
        mapDescriptor.RequiredPCModsID.Add("com.me.gorillatag.mypcmod");
        mapDescriptor.RequiredQuestModsID.Add("com.me.gorillatag.myquestmod");
	}
}
```

Add your own data by adding to `mapDescriptor.CustomData`, which is a Dictionary<string, object>. To require your mod to be installed to load the map, add to `mapDescriptor.RequiredPCModsId` and `mapDescriptor.RequiredQuestModsId` lists. Any property of the descriptor can be modified by your script, though other build events may override these values.

If you make a custom descriptor, feel free to make a pull request to this repository with the descriptor and updated documentation.

Documentation for accessing this data in your mod can be found on the [PC](https://github.com/Vadix88/MonkeMapLoader#for-developers) and [Quest](https://github.com/RedBrumbler/MonkeMapLoader) github pages.
