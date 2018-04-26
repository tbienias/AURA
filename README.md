# Holo Lens

## Retrieve the project with MRTK
Clone the repository of your project. The use of the –recursive flag is necessary to retrieve also all submodules.

```
git clone --recursive git@gitlab.com:HDA_MPSE_HoloLens/HoloLens.git
```

If this is the first time you retrieve the project on your machine, make a folder link in Assets to the HoloToolkit folder to make it available again to the project. Run this command from within the project folder:

```
cd HoloLens\UnityProject
mklink /j .\Assets\HoloToolkit .\MixedRealityToolkit-Unity\Assets\HoloToolkit
```

Open the project in Unity and open the main scenario. Set the project settings using the MRTK menu [Configure – Apply Mixed Reality Project Settings] and you’re done.