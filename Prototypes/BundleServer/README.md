# Prototype: *BundleServer*
## Brief description
This prototype loads a bundle from a server, checks which files it contains and finally displays it.

## How to use
* Clone folder to your local workspace
* Start Unity and open the Project through navigating to the BundleServer-Folder
* When the Project is fully loaded, select Assets -> Scenes -> Main (this step is essential, as it is set up in a way that will ensure the AssetManager is executed)
* Navigate into the Root-Folder of BundleServer and execute the Python-Script 'AssetServer.py' (e.g. from a command line)
* Make sure no other processes are currently using TCP-Port 80 to listen on your machine, as it would fail otherwise
* Switch back to Unity and press the Play-Button
* ...
* Be amazed!

