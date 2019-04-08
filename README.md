### Customization

All changes are applied through the `config.json` file which is included with the mod download. The values that come with the mod provide a "Normal" MultiMod experience. If you want to start from scratch, there is a file with pre-configured vanilla settings available on Nexus

### config.json descriptions
the given values reflect the "normal" version of the mod. if you start with the "vanilla" configuration, you should reference the Description of the values and adjust as you please

Name | Value | Description
:--- | :-----: | :---
ScannerSpeedNormalInterval | 14.0 | the default interval in seconds for locating a new blip
ScannerSpeedMinimumInterval | 1.0 | sets the fastest rate that your scanner can run at, after other adjustments
ScannerSpeedIntervalPerModule | 3.0 | each speed module added removes 3.0 seconds from the interval
ScannerBlipRange | 1000.0 | how far your holomap will reach. also increases cutoff distance on the HUD icon
ScannerMinRange | 600.0 | 
ScannerUpgradeAddedRange | 100.0 | the added range of the scanner per range module
ScannerCameraRange | 1000.0 | the distance where the camera begins to get noise/fuzz. the full sugnal is lost at 2.05x this number
