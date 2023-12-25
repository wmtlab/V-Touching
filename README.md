# V-Touching Project

## 1. Related Hardware
VR HMD (HTC VIVE Pro 2 in this project)

VR Tracker (HTC VIVE Tracker 3.0 in this project)

Tactile glove (AvatarVR in this project)

Keyboard (optional if you don't want to use VR devices)

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

## 4. Data Source
In this project, we provide [some data](TouchMaterialServer/Assets/_Project/Materials/DUT) for direct testing of our application. Additionally, we have placed the data source and usage instructions in another repository: [Pixel2Taxel Database](https://github.com/wmtlab/Pixel2Taxel).

## 5. Project Structure
### 5.1 Client Main Scene
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
2. **Pose Controller/Sync Objects**: The objects to be uploaded to the server, such as the objects in **LeftHandBone**. Notice that the index of an object is always small than its child objects. For example, if the index of **LeftHandBone/hand_l** is 0, the index of **LeftHandBone/hand_l/index_01_l** must be bigger than 0.
3. **Steam Vr Avatar Init**: Set the virtual hand as the child of the tracker if **Use Vr** is true. If **Use Vr** is false, the hierarchy would not be changed.
4. **Video Controller**: The controller of the video feedback. The video feedback is from the server. **Raw Image** is the RawImage object in **Canvas**. **Interval Frames** is the interval physics frames between two video frames.
5. **Tactile Controller**: **Actuator Controller** is an adapter for the tactile glove to render the tactile feedback. You can edit Assets/_Project/Scripts/TactileActuator/TactileActuatorController.cs to adapt to other tactile gloves. Generally, there should be an array of actuators in **Actuator Controller**, which is corresponding one-to-one to **Tactile Controller/Detectors** in the server's **App** object. **Frame Count** is the number of frames in a tactile feedback message, while **Use Compression** is whether to use the decoder.

### 5.2 Server Main Scene
Notice: Some unimportant objects are ignored in the following description.

    App: {
        Main Camera,
        LeftHand: {
            LeftHandMesh,
            LeftHandBone
        },
        DUT: {
            book001,
            ...
        }
    }

1. **Main Camera** is the camera to capture the view of the user. Its pose is synchronized with the eye camera of **[Camera Rig]** in the client project.
2. **LeftHand** is the virtual hand. **LeftHandMesh** is active, in order to display the virtual hand. The poses of objects in **LeftHandBone** are synchronized with that in the client project, and drive the **LeftHandMesh** to move.
3. **DUT** is a folder containing the tactile objects. Tactile objects like **book001** and **gift_box001**, are the objects to be touched by the user. When a tactile object is touched, the tactile feedback would be generated.

Tactile objects are attached with **TactileObject** script. There are 2 properties in the inspector: **Reference Map** and **Tactile Multiplier**. **Reference Map** is the tactile texture of the tactile object, whose color is corresponding to the tactile feedback. **Tactile Multiplier** is the multiplier of the tactile feedback, ranging from 0.0 to 1.0. **Tactile Multiplier** indicates the strength of the tactile object.

Finger tips in the objects of **LeftHandBone**, for example **LeftHandBone/hand_l/index_01_l/index_02_l/index_03_l**, are the tactile points. The tactile points are corresponding one-to-one to the actuators in the client project. These tactile point objects are attached with **TactileDetector** script, which is used to detect the collision between the tactile points and the tactile objects. When a tactile point stays in a tactile object, tactile feedback would be generated by the touched tactile object and gathered continuously until the tactile point leaves the tactile object. 

When the **App** object is selected, you can see the settings of the server in the inspector, whose script is App.cs. The settings are as follows:

1. **Net Json**: The json file to store the network settings such as IPs, ports, etc. In this project, the file is Assets/_Project/Properties/NetworkSetting.json.
2. **Pose Controller**: Receive the poses from the client project and update the poses of the objects **Sync Objects**. **Sync Objects** is an array of the objects to be synchronized with the client project, the order should be the same as that in the client project. **Time Out Sec** is the time out of the synchronization. If the synchronization is not updated for a long time, the synchronization and the feedbacks would be stopped.
3. **Video Controller**: Capture the view and send the video frames to the client project. **Main Camera** is filled with **Main Camera** in the scene. **Interval Frames** is the interval physics frames between two video frames.
4. **Tactile Controller**: Obtain the tactile feedbacks, send them to the encoder (optional), and send the encoded tactile feedbacks to the client project. **Detectors** is an array of the tactile points. The order should be the same as the actuators in the client project. **Frame Count** is the number of frames in a tactile feedback message, while **Use Compression** is whether to use the encoder.

### 5.3 Encoder/Decoder Project
... Introduction still at work...

## 6. License
This project is licensed under the GPL 3.0 License - see the LICENSE file for details.

Referenced projects:

1. **UniTask** MIT https://github.com/Cysharp/UniTask
2. **LitJSON** unlicensed https://github.com/LitJSON/litjson
3. **steamvr_unity_plugin** BSD-3-Clause license https://github.com/ValveSoftware/steamvr_unity_plugin
4. **AvatarVR SDK** NeuroDigital Technologies https://www.neurodigital.es
