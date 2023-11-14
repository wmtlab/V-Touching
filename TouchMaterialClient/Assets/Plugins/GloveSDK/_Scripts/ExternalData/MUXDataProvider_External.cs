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
using System;
using UnityEngine;

/// <summary>
/// MuxDataProvider created for external devices. Hand position and rotation can be set with this script.
/// </summary>
public class MUXDataProvider_External : PCI_MUXDataProvider
{
    /// <summary>
    /// Transform that establishes the position of the hand
    /// </summary>
    public Transform positionSource;
    /// <summary>
    /// Transform that establishes the rotation of the hand
    /// </summary>
    public Transform rotationSource;
    /// <summary>
    /// Position offset to apply
    /// </summary>
    public Vector3 positionOffset;
    /// <summary>
    /// Rotation offset to apply
    /// </summary>
    public Vector3 rotationOffset;

    protected Vector3 currentHandPosition = new Vector3();
    protected Vector3 previousPositionValue = new Vector3();

    protected Quaternion currentHandRotation = Quaternion.identity;
    protected Quaternion previousRotationValue = Quaternion.identity;

    public override void Start()
    {
        ResetDeltaPosition();
        base.Start();
        SetActive(true);
    }

    /// <summary>
    /// Trigger called everytime this mux provider is enabled
    /// </summary>
    public override void OnProviderEnabled()
    {
        ResetDeltaPosition();
    }

    /// <summary>
    /// Reset previous values for delta calculations
    /// </summary>
    private void ResetDeltaPosition()
    {
        switch (bonesToControlPositionType[0])
        {
            case Type_MUXValueType.deltaLocalSpace:
                previousPositionValue = positionSource.localPosition;
                break;
            case Type_MUXValueType.deltaWorldSpace:
                previousPositionValue = positionSource.position;
                break;
        }

        switch (bonesToControlRotationType[0])
        {
            case Type_MUXValueType.deltaLocalSpace:
                previousRotationValue = Quaternion.Euler(rotationSource.localEulerAngles);
                break;
            case Type_MUXValueType.deltaWorldSpace:
                previousRotationValue = Quaternion.Euler(rotationSource.eulerAngles);
                break;
        }

    }

    /// <summary>
    /// Calculate the hand rotation depending on the type of rotation to apply
    /// </summary>
    public override void CalculateHandRotation()
    {
        switch (bonesToControlRotationType[0])
        {
            case Type_MUXValueType.localSpace:
                currentHandRotation = Quaternion.Euler(rotationSource.localEulerAngles + rotationOffset);
                previousRotationValue = currentHandRotation;
                break;
            case Type_MUXValueType.worldSpace:
                currentHandRotation = Quaternion.Euler(rotationSource.eulerAngles + rotationOffset);
                previousRotationValue = currentHandRotation;
                break;
            case Type_MUXValueType.deltaLocalSpace:
                currentHandRotation = Quaternion.Euler(rotationSource.localEulerAngles + rotationOffset - previousRotationValue.eulerAngles);
                previousRotationValue = Quaternion.Euler(rotationSource.localEulerAngles + rotationOffset);
                break;
            case Type_MUXValueType.deltaWorldSpace:
                currentHandRotation = Quaternion.Euler(rotationSource.eulerAngles + rotationOffset - previousRotationValue.eulerAngles);
                previousRotationValue = Quaternion.Euler(rotationSource.eulerAngles + rotationOffset);
                break;
        }

        if (handModelController)
            SetMuxHandRotation(currentHandRotation);
    }

    /// <summary>
    /// Calculate the hand position depending of the type of position to apply
    /// </summary>
    public override void CalculateHandPosition()
    {
        switch (bonesToControlPositionType[0])
        {
            case Type_MUXValueType.localSpace:
                currentHandPosition = positionSource.localPosition + positionOffset;
                previousPositionValue = currentHandPosition;
                break;
            case Type_MUXValueType.worldSpace:
                currentHandPosition = positionSource.position + positionOffset;
                previousPositionValue = currentHandPosition;
                break;
            case Type_MUXValueType.deltaLocalSpace:
                currentHandPosition = positionSource.localPosition + positionOffset - previousPositionValue;
                previousPositionValue = positionSource.localPosition + positionOffset;
                break;
            case Type_MUXValueType.deltaWorldSpace:
                currentHandPosition = positionSource.position + positionOffset - previousPositionValue;
                previousPositionValue = positionSource.position + positionOffset;
                break;
        }

        if (handModelController)
            SetMuxHandPosition(currentHandPosition);
    }

    public virtual void Update()
    {
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    positionSource.localPosition += Vector3.forward * 0.01f;
        //}

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    positionSource.localPosition -= Vector3.forward * 0.01f;
        //}

        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    positionSource.localPosition -= Vector3.right * 0.01f;
        //}

        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    positionSource.localPosition += Vector3.right * 0.01f;
        //}

        //if (Input.GetKey(KeyCode.Q))
        //{
        //    positionOffset -= Vector3.up * 0.01f;
        //}

        //if (Input.GetKey(KeyCode.E))
        //{
        //    positionOffset += Vector3.up * 0.01f;
        //}

        CalculateHandPosition();
        CalculateHandRotation();
    }


    public virtual void SetIKPosition(Vector3 positionIK)
    {
        throw new NotImplementedException("This parent class doesn't use IK.");
    }

}
