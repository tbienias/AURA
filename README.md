# AURA

## Set up development environment
### Clone the repository

```bash
git clone --recurse-submodules -j8 https://gitlab.com/HDA_MPSE_HoloLens/HoloLens.git
```

Adding the `--recurse-submodules` flag checks out the dependency submodules as well.

**Optional**: If you skip this step you must manually check out the dependency submodules when you need them. You can do this with:

```bash
git submodule update --init
```

### Create a symbolic link to the MixedRealityToolkit

```bash
cd AURA && mklink /D Assets\HoloToolkit ..\MixedRealityToolkit-Unity\Assets\HoloToolkit
```

### Apply the MixedRealityToolkit project settings
Don't forget to Check the "Use Toolkit-specific InputManager axes" box, so that the XboxController work.

![MRTK project settings](/uploads/1ffada9b98338e01f43c23b7f9bd60ce/Unity_2018-05-17_20-14-38.png)