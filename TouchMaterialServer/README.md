# V-Touching Server Configuration and Customization

Majority of the configuration and customization of the V-Touching server is done through the `App` object in the `Main` scene. Select the `App` object in the Hierarchy window and you will see the main configuration and customization options in the Inspector window.

## 1. Network Settings
Double click the value of `Net Json` on `App` or open `Assets/_Project/Properties/NetworkSetting.json`, you will see the configurations of IPs, ports and buffer sizes. Modify them according to your environment. Buffer sizes are in bytes, and not suggested to be modified smaller than the default values, unless you are sure that it would be OK.

## 2. Hand Pose Synchronization
Expand `App>Pose Controller>Sync Objects` on `App`. Drag the objects to be synchronized into the list. Make sure the order is **same** as those in the client. Whether in the server or in the client, the index of a parent object **must** be smaller than that of its child object. Otherwise, the synchronization will be set with error values.

## 3. Tactile Data Source and Tactile Object
1. The format of tactile data sources: 2 binary files named with `header.bin` and `data.bin`. 
2. `header.bin` contains the split information of `data.bin` and is filled with **little-endian 4-byte integers**. The first 2 integers are the height and width of the tactile data matrix. Followed by the shape information, each integer represents the beginning index of a taxel's signals. The count of indices equals the height multiplied by the width. In other words, `header.bin` is the flattened beginning index matrix of the tactile data matrix.
3. `data.bin` contains the flattened tactile data matrix and is filled with **little-endian 4-byte floats**.
4. Purpose of the tactile data storage format: to reduce the size of the tactile data files and to store the tactile signals with uncertain length on each taxel.
5. Import the tactile data files into Unity: create a folder in `Assets/StreamingAssets/TactileData` and name it. Put the 2 tactile data files into the folder. Create a cube as a child of `App/DUT` object and name it with the same name as the folder. Create a material and configure its texture (`Albedo`) with the picture of the tactile object. Drag the material to the cube and adjust the size and position of the cube to your satisfaction. Add `TactileObject` component to the cube. The cube is added with `BoxCollider` by default. If there is no collider, add one to the cube. Make sure the `BoxCollider` is checked with `Is Trigger`. Create an empty object as the cube's child, add `MeshCollider` component and set the layer as `TactileObject`. Select the empty object, click the circle on the right of `Mesh` property and select the cube in the dialog. The `BoxCollider` is utilized for collision detection and the `MeshCollider` is utilized for raycast. 
6. `Fixed Size` in `TactileObject` component means the expected fixed size of the signals in a taxel. Although the tactile signals are stored with uncertain length, the signals will be cached and interpolated to the fixed size after the tactile data is loaded in the runtime. Make sure the value is same as `App>Tactile Controller>Tactile Fixed Size` on `App`.

## 4. Tactile Data Acquisition
Select an object expected to touch the tactile objects. Make sure it's added with a collider, checked with `Is Trigger`, and add `Tactile Detector` component. Add the object to the list called `App>Tactile Controller>Detectors` on `App`. The order of the objects in the list is same as the order of the tactile actuator list in the client.

## 5. Tactile Encoder
1. Implement interface `IEncodeHelper`
2. `void IEncodeHelper.Start()` is used to initialize the codec. `void IEncodeHelper.Stop()` is used to release the codec. If the 2 methods are not needed, just leave them empty. 
3. `Task<(byte[], int)> EncodeAsync(float[][] tactileCaches)` is the method to encode the tactile data. The 1st dimension of the `tactileCaches` array represents the tactile actuators. The 2nd dimension represents the tactile signals. The return value is a tuple. The 1st element is the encoded tactile data. The 2nd element is the size of the encoded tactile data. This method is designed as an asynchronous method, in order to avoid blocking the main thread.
4. Assign `_encoderHelper` in `TouchMaterial.Server.TactileController.Init()` with the instance of the implemented class.

## 6. Log
If you want to log the tactile signals before encoding, add `#define LOG_BEFORE_CODEC` at the beginning of `Assets/_Project/Scripts/Controllers/TactileController.cs`. The logs are written into `Assets/StreamingAssets/Log/`. If `LOG_BEFORE_CODEC` is defined, the log folder will be cleared and created every time the server starts. Note that the log files only contain the obtained signals of the first detector in the list of `App>Tactile Controller>Detectors` on `App`.
