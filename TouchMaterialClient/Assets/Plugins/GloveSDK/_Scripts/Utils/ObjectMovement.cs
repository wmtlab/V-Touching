/******************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018							  *
* Licensed under the Apache License, Version 2.0 (the "License");			  *
* you may not use this file except in compliance with the License.			  *
* You may obtain a copy of the License at 									  *
* http://www.apache.org/licenses/LICENSE-2.0								  *
* Unless required by applicable law or agreed to in writing, software		  *
* distributed under the License is distributed on an "AS IS" BASIS,			  *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.	  *
* See the License for the specific language governing permissions and		  *
* limitations under the License.										      *
*******************************************************************************/

using UnityEngine;
using NDAPIUtils;

/// <summary>
/// Auxiliar class for grabbing classes
/// </summary>
public class ObjectMovement : MonoBehaviour
{
    /// <summary>
    /// Circular buffer that stores position
    /// </summary>
    private TimedCircularBuffer<Vector3> posBuffer;
    /// <summary>
    /// Averaged circular buffer that stores averaged velocity
    /// </summary>
    private TimedAveragedCircularBufferV3 avgVelBuffer;
    /// <summary>
    /// Averaged circular buffer that stores averaged position
    /// </summary>
    private TimedAveragedCircularBufferV3 avgPosBuffer;

    [HideInInspector]
    public Vector3 objectSmoothVelocity = Vector3.zero;
    [HideInInspector]
    public Vector3 objectPreviousVelocity = Vector3.zero;
    private Vector3 objectPos;
    private Vector3 objectVelocity = Vector3.zero;

    /// <summary>
	/// Acceleration delta time.
	/// </summary>
	public float accDeltaTime = 0.05f;
    /// <summary>
    /// Velocity delta time.
    /// </summary>
    public float velDeltaTime = 0.05f;
    /// <summary>
    /// Position delta time.
    /// </summary>
    public float posDeltaTime = 0.05f;

    void Start()
    {
        avgVelBuffer = new TimedAveragedCircularBufferV3();
        avgPosBuffer = new TimedAveragedCircularBufferV3();
        posBuffer = new TimedCircularBuffer<Vector3>();
    }

    void FixedUpdate()
    {
        float curTime = Time.time;

        // Get object position and stabilize it
        objectPos = transform.position;
        if (avgPosBuffer != null)
            avgPosBuffer.addValue(curTime, objectPos);
        Vector3 avgObjectPos = avgPosBuffer.getAverage(curTime - posDeltaTime);

        // Add stabilized position to a buffer to calculate the speed
        if (posBuffer != null)
            posBuffer.addValue(curTime, avgObjectPos);

        float oldTime;
        Vector3 oldPosition = posBuffer.getValue(curTime - accDeltaTime, out oldTime);

        if (curTime - oldTime > 0)
        {
            objectVelocity = (avgObjectPos - oldPosition) / (curTime - oldTime);
        }

        if (avgVelBuffer != null)
            avgVelBuffer.addValue(curTime, objectVelocity);

        objectSmoothVelocity = avgVelBuffer.getAverage(curTime - velDeltaTime);
        objectPreviousVelocity = avgVelBuffer.getAverage(curTime - velDeltaTime - velDeltaTime);
    }
}
