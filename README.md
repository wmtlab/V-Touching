# V-Touching Project

## 1. Related Hardware
VR HMD (HTC VIVE Pro 2 in this project)

VR Tracker (HTC VIVE Tracker 3.0 in this project)

Tactile glove (AvatarVR in this project)

Keyboard (optional if you don't want to use VR devices)

## 2. Related Software
\* Unity 2018 (2018.4.36f1 recommended), for the VR project

\* Python (3.8 recommended), for bridging the VR project and the encoding/decoding project

Matlab R2021a, for the tactile encoding/decoding used in this project. If you want to use other encoding/decoding methods, you can ignore this requirement.

## 3. Quick Start
1. Clone all the projects in this repository to your local machine, including TouchMaterialClient as the client, TouchMaterialServer as the server, and TactileCompression as the encoder/decoder. Although the projects are compatible in LAN environment, we recommend you to run the projects on the same machine. If you want to run the projects on different machines, you need to modify some network settings in the projects.
2. Open the server and client projects in Unity.
3. If you want to use the codec, open the encoder/decoder project in any code editor which supports Python. Matlab is not required to be opened. Run `main.py` in the encoder/decoder project and wait until the console shows `"socket running"`.
4. In the server project, open `Assets/_Project/Scenes/Main.unity` as active scene. In the client project, open `Assets/_Project/Scenes/Main.unity` as active scene.
5. If you want to run in VR environment, before running, make sure the HMD, tracker and tactile glove are ready to use. Generally, the tracker and the tactile glove are wore on the same hand. Set `App>VR Switcher>Use Vr` on the `client App object` to true. Then, you can control the position of the hand with the tracker. The position reset key is `Q`.
6. If you want to run in non-VR environment, set `App>VR Switcher>Use Vr` on the `client App object` to false. Then, you can control the position of the hand with `WASD` or `arrow` keys. The position reset key is `Q`.
7. Click play button in the server project and client project. There may be some errors in the console, but it doesn't matter. If everything goes well, you can see the video feedback in the client project. What the video feedback displays is same as the scene in the server project.
8. Move your hand with the tactile glove to touch the virtual objects in the client project. You can feel the tactile feedback in the tactile glove. In the setting of this project, tactile points are the tips of index finger, middle finger and ring finger.
9.  Since the codec is still reworking, `App>Tactile Controller>Use Compression` on the `client App object` is set to false by default. If you want to use the codec, set it to true. After the codec is ready, we will update the codec and the client project.

## 4. Detailed Configuration and Customization
Still in progress...

## 5. License
This project is licensed under the GPL 3.0 License - see the LICENSE file for details.

Referenced projects:

1. `UniTask` MIT https://github.com/Cysharp/UniTask
2. `LitJSON` unlicensed https://github.com/LitJSON/litjson
3. `steamvr_unity_plugin` BSD-3-Clause license https://github.com/ValveSoftware/steamvr_unity_plugin
4. `AvatarVR SDK` NeuroDigital Technologies https://www.neurodigital.es
