# V-Touching Project

## 1. Required Hardware
\* VR HMD (HTC VIVE Pro 2 in this project)

\* VR Tracker (HTC VIVE Tracker 3.0 in this project)

\* Tactile glove (AvatarVR in this project)

## 2. Required Software
\* Unity 2018 (2018.4.36f1 recommended), for the VR project

\* Python (3.8 recommended), for bridging the VR project and the encoding/decoding project

Matlab R2021a, for the tactile encoding/decoding used in this project. If you want to use other encoding/decoding methods, you can ignore this requirement.

## 3. Quick Start
1. Clone all the projects in this repository to your local machine, including TouchMaterialClient as the client, TouchMaterialServer as the server, and TactileCompression as the encoder/decoder.
2. Open the server and client projects in Unity, and open the encoder/decoder project in any code editor which supports Python. Matlab is not required to be opened.
3. In the server project, open Assets/_Project/Scenes/Main.unity as current scene. In the client project, open Assets/_Project/Scenes/Main.unity as current scene.
4. Before running, make sure the HMD, tracker and tactile glove are ready to use. Generally, the tracker and the tactile glove are wore on the same hand.
5. Run main.py in the encoder/decoder project and wait until the console shows "socket running".
6. Click play button in the server project and client project. There may be some errors in the console, but it doesn't matter. If everything goes well, you can see the video feedback in the client project. What the video feedback displays is same as the scene in the server project.
7. Move your hand with the tactile glove to touch the virtual objects in the client project. You can feel the tactile feedback in the tactile glove. In the setting of this project, tactile points are the tips of index finger, middle finger and ring finger.
8. If you want to control the position of the hand without VR, you can set **Use Vr** (will be introduced later) to false. Then, you can control the position of the hand with W, A, S, and D keys. The position reset key is Q.

## 4. Project Structure
### 4.1 Client Main Scene
Notice: Some unimportant objects are ignored in the following description.

    App: {
        [CameraRig],
        Canvas: {
            RawImage
        }
    },
    NDInitializer,
    LeftHand: {
        LeftHandMesh,
        LeftHandBone
    }

1. **[CameraRig]** is the VR play space provided by SteamVR.
2. **Canvas** contains an RawImage object, which is used to display the video feedback.
3. **NDInitializer** is the controller object of AvatarVR.
4. **LeftHand** is the virtual hand of AvatarVR. **LeftHandMesh** is inactive and **LeftHandBone** is active. The poses to upload to the server are from the objects in **LeftHandBone**.

When the **App** object is selected, you can see the settings of the client in the inspector, whose script is App.cs. The settings are as follows:

1. **Net Json**: The json file to store the network settings such as IPs, ports, etc. In this project, the file is Assets/_Project/Properties/NetworkSetting.json.
2. **Pose Controller/Sync Objects**: The objects to be uploaded to the server, such as the objects in **LeftHandBone**.
3. **Steam Vr Avatar Init**: Set the virtual hand as the child of the tracker if **Use Vr** is true. If **Use Vr** is false, the hierarchy would not be changed.
4. **Video Controller**: The controller of the video feedback. The video feedback is from the server. **Raw Image** is the RawImage object in **Canvas**. **Interval Frames** is the interval physics frames between two video frames.
5. **Tactile Controller**: **Actuator Controller** is an adapter for the tactile glove to render the tactile feedback. You can edit Assets/_Project/Scripts/TactileActuator/TactileActuatorController.cs to adapt to other tactile gloves. Generally, there should be an array of actuators in **Actuator Controller**, which is corresponding one-to-one to **Tactile Controller/Detectors** in the server's **App** object. **Frame Count** is the number of frames in a tactile feedback message, while **Use Compression** is whether to use the encoder/decoder.

### 4.2 Server Main Scene
... Introduction still at work...

### 4.3 Encoder/Decoder Project
... Introduction still at work...