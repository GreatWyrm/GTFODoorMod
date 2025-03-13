# GTFODoorMod

A GTFO mod for a personal rundown I am working on. Currently, adds 6 new WardenObjectiveEvents, described in the table below.

## Usage

This is meant for Rundown developers, as it just adds more events into the game and *should* leave the base game untouched.
It does make one alteration to the base game to have Pablo (the immortal) stagger on any hit.

To use the new events, add a new WardenObjectiveEvent with the types and parameters described below:

| Event                               | Id (Type) | Parameters Used                                                                                         |
|-------------------------------------|-----------|---------------------------------------------------------------------------------------------------------|
| LockAllDoorsInZone                  | 50        | LocalIndex - The target zone to lock all doors in                                                       |
| UnlockAllDoorsInZone                | 51        | LocalIndex - The target zone to unlock all doors in                                                     |
| OpenAllWeakDoorsInZone              | 52        | LocalIndex - The target zone to open all doors in                                                       |
| CloseAllWeakDoorsInZone             | 53        | LocalIndex - The target zone to close all doors in                                                      |
| ReplaceZoneDoorAlarm (Experimental) | 54        | LocalIndex - The target zone (retrieves the security door), ChainPuzzle - The new chained puzzle to use |
| ForgetLastKnowMainDimensionPos      | 55        | None                                                                                                    |

The Type field should be filled in with the number in the Id column.

## Compiling

This project was compiled with JetBrain's Rider on Linux, but can be compiled with `dotnet build` in the project root directory.

You will need to create a `deps` folder and put in specific .dll files from the BepInEx interop folder, created when you launch a modded instance with [R2Modman](https://thunderstore.io/package/ebkr/r2modman/) with BepInEx installed. You can copy the specific ones listed in GTFODoorMod.csproj file, or just copy over the entire directory just to be safe.

## Contributing

If you spot an issue, feel free to make a PR, and I'll try to get back to you in a few days!
