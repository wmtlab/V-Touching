# V-Touching Client Configuration and Customization

Majority of the configuration and customization of the V-Touching client is done through the `App` object in the `Main` scene. Select the `App` object in the Hierarchy window and you will see the main configuration and customization options in the Inspector window.

## 1. Network Settings
Double click the value of `Net Json` on `App` or open `Assets/_Project/Properties/NetworkSetting.json`, you will see the configurations of IPs, ports and buffer sizes. Modify them according to your environment. Buffer sizes are in bytes, and not suggested to be modified smaller than the default values, unless you are sure that it would be OK.

## 2. Hand Pose Synchronization
expand `App>Pose Controller>Sync Objects` on `App`. Drag the objects to be synchronized into the list. Make sure the order is **same** as those in the server. Whether in the server or in the client, the index of a parent object **must** be smaller than that of its child object. Otherwise, the synchronization will be set with error values.

## 3. Tactile Decoder
1. Implement interface `IDecodeHelper`
2. `void IDecodeHelper.Start()` is used to initialize the codec. `void IDecodeHelper.Stop()` is used to release the codec. If the 2 methods are not needed, just leave them empty. 
3. `Task Decode(byte[] data, Action<float[][]> onFinished = null)` is the method to decode the encoded tactile data. `data` is the encoded data received from the server. `onFinished` is the callback, the 1st dimension of the parameter represents the tactile actuators, and the 2nd dimension of the parameter represents the tactile signals. This method is designed as an asynchronous method, in order to avoid blocking the main thread.
4. Assign `_decoderHelper` in `TouchMaterial.Client.TactileController.Init()` with the instance of the implemented class.

## 4. Tactile Feedback
1. Implement interface `ITactileActuatorController`
2. `ActuatorCount` property is used to get the number of tactile actuators.
3. `void Play(int actuatorIdx, float[] signals)` method is used to render tactile signals. `actuatorIdx` is the index of the tactile actuator. `signals` is the decoded tactile signals.
4. `void Stop(int actuatorIdx)` method is used to stop target tactile actuator.
5. `void StopAll()` method is used to stop all tactile actuators. It is called to avoid tactile actuators keep vibrating after the client is closed.
6. Assign `_actuatorController` in `TouchMaterial.Client.TactileController.Init()` with the instance of the implemented class.
7. If you are using the default tactile actuator controller (`TouchMaterial.Client.TactileActuatorController`), the order of the list `TouchMaterial.Client.TactileActuatorController._actuators` must be the same as the order of the tactile detectors in the server. 

## 5. Log
If you want to log the tactile signals after decoding, add `#define LOG_AFTER_CODEC` at the beginning of `Assets/_Project/Scripts/Controllers/TactileController.cs`. The logs are written into `Assets/StreamingAssets/Log/`. If `LOG_AFTER_CODEC` is defined, the log folder will be cleared and created every time the client starts. Note that the log files only contain the first decoded signals of the first actuator in the list `TouchMaterial.Client.TactileActuatorController._actuators`.