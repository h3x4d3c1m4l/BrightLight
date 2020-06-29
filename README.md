# BrightLight
BrightLight is an extendible Spotlight like launcher for Windows.

## [NOT IN DEVELOPMENT ANYMORE]

Although the launcher runs, searches properly and has a nice basic plugin system, as the new [Microsoft PowerToys](https://github.com/microsoft/PowerToys) project contains a launcher with a lot more features, I don't think it is useful anymore to work on this project.

## Running the project
1. Open the solution file in Visual Studio 2019 or JetBrains Rider

2. Run the `BrightLight.DesktopApp.WPF` project

3. Press Ctrl+Shift+Spacebar in order to show the launcher

   (If this does not work, perhaps the default hotkey is already claimed, there is no settings for this at the moment, but for now the tray icon allows to open the launcher)

## Current features
* Evaluate math expressions
* Searches Wikipedia
* Searches $PATH and start menu
* Basic plugin system

## Roadmap
* Extend plugin system
* Builds on a regular base
* Different delays per search provider
  * This will make searching the start menu a lot faster
