# AURA

**This project is discontinued due to time reasons and hardware availability.**

AURA is a Mixed Reality Project implemented with Unity and running on the Microsoft HoloLens. It aims to enable the user to enrich the real world with 3D geometry.

## Product Video

[<img src="https://img.youtube.com/vi/4JplB5ZMiek/maxresdefault.jpg" width="75%">](https://youtu.be/4JplB5ZMiek)

## Reports 

[Winter 2017/2018](./report_ws1718.pdf)

[Summer 2018](./report_ss18.pdf)


## Set up development environment
### Clone the repository

```bash
git clone --recurse-submodules -j8 https://gitlab.com/project-aura/aura.git
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

## License

LGPL 2.1