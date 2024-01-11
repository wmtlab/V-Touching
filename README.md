# V-Touching Project

## 1. Related Hardware
   - VR HMD (HTC VIVE Pro 2 in this project)
   - VR Tracker (HTC VIVE Tracker 3.0 in this project)
   - Tactile glove (AvatarVR in this project)
   - Keyboard (optional if you don't want to use VR devices)

## 2. Related Software
   - Unity 2018 (2018.4.36f1 recommended)

## 3. Quick Start
1. Clone all the projects in this repository to your local machine, including TouchMaterialClient as the client and TouchMaterialServer as the server. Although the projects are compatible in LAN environment, we recommend you to run the projects on the same machine. If you want to run the projects on different machines, you need to modify some network settings in the projects.
2. Open the server and client projects in Unity. In the server project, open `Assets/_Project/Scenes/Main.unity` as active scene. In the client project, open `Assets/_Project/Scenes/Main.unity` as active scene.
3. If you want to run in VR environment, before running, make sure the HMD, tracker and tactile glove are ready to use. Generally, the tracker and the tactile glove are wore on the same hand. Set `App>VR Switcher>Use Vr` on the `client App object` to true. Then, you can control the position of the hand with the tracker. The position reset key is `Q`.
4. If you want to run in non-VR environment, set `App>VR Switcher>Use Vr` on the `client App object` to false. Then, you can control the position of the hand with `WASD` or `arrow` keys. The position reset key is `Q`.
5. Click play button in the server project and client project. There may be some errors in the console, but it doesn't matter. If everything goes well, you can see the video feedback in the client project. What the video feedback displays is same as the scene in the server project.
6. Move the virtual hand to touch the virtual objects in the client project. You can feel the tactile feedback rendered by the tactile glove. In the setting of this project, tactile points are the tips of index finger, middle finger and ring finger.
7. MPEG phase I codec has been merged in this project. However, because the codec is not completed, we had to use the JSON file version, causing the encoded tactile data much larger than the raw tactile data. Due to the large size of the encoded tactile data and the update rate of Unity, the client may break down when running for a long time. If you want to disable/enable the codec, set `App>Tactile Controller>Use Compression` on the `client App object` to false/true. After the codec is ready, we will update the codec and the client project.

## 4. Detailed Configuration and Customization
See [Client Configuration and Customization](./TouchMaterialClient/README.md) and [Server Configuration and Customization](./TouchMaterialServer/README.md) for more details.

## 5. License
This project is licensed under the GPL 3.0 License - see the LICENSE file for details.

Referenced projects:

1. `UniTask` MIT https://github.com/Cysharp/UniTask
2. `LitJSON` unlicensed https://github.com/LitJSON/litjson
3. `steamvr_unity_plugin` BSD-3-Clause license https://github.com/ValveSoftware/steamvr_unity_plugin
4. `AvatarVR SDK` NeuroDigital Technologies https://www.neurodigital.es
