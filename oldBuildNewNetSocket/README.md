# Visualization Tool for a Drone Pilot in Microsoft HoloLens 2
## Bachelor thesis 2021/2022
## Author: Martin Kyjac
## University: Brno, University of Technology, Faculty of Information Technology

Runs with Unity 2019.4.12f1

To run in unity, open scene with name SampleScene.

# Build and deploy
Firstly generated build is needed. This build is generated in unity. Open SampleScene if not opened already. From the top menu select File->Build settings. Then select Universal Windows Platform on the left side of the window. After that, this values have to be set:
- **Target Device**: HoloLens
- **Architecture**: ARM64
- **Build Type**: D3D Project
- **Target SDK Version**: Latest installed
- **Minimum platform version**: 10.0.18362.0
- **Visual Studio Version**: Latest installed
- **Build and Run on**: USB Device
- **Build configuration**: Release
- **Select checkbox Copy PDB files**

Then click on Build. This will generate IL2CPP project which will be deployed to MS HoloLens 2. Open drone_outdoor_alpha.sln located in generated folder and add NuGet package SQLite.Universal(https://www.nuget.org/packages/SQLite.Universal/) to project drone_outdoor_alpha (Universal Windows Platform). Then connect MS HoloLens 2 with PC via cable, select Release option for ARM64 architecture and run. This will deploy the application into MS HoloLens 2.

# Test application
It is possible to test application by sending sample drone data generated from script in this repo https://github.com/kycendo/sample_drone_data.
