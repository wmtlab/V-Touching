/******************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018                            *
* Licensed under the Apache License, Version 2.0 (the "License");             *
* you may not use this file except in compliance with the License.            *
* You may obtain a copy of the License at                                     *
* http://www.apache.org/licenses/LICENSE-2.0                                  *
* Unless required by applicable law or agreed to in writing, software         *
* distributed under the License is distributed on an "AS IS" BASIS,           *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.    *
* See the License for the specific language governing permissions and         *
* limitations under the License.                                              *
*******************************************************************************/

using UnityEngine;
using UnityDLL.Core;
using UnityDLL.Contacts;
using UnityDLL.Haptic;

/// <summary>
/// Core script. This scripts creates an instance of NDService.
/// This script needs to be placed in the main scene in order to make the devices work.
/// </summary>
public class NDInitializer : MonoBehaviour
{
    /// <summary>
    /// Static instance
    /// </summary>
    private static NDInitializer _instance = null;

    /// <summary>
    /// True if contacts are enable for this scene
    /// </summary>
    public bool useContacts;
    /// <summary>
    /// True if haptics are enable for this scene
    /// </summary>
    public bool useHaptics;

    /// <summary>
    /// Reset key. When this key is pressed, it recalibrates the devices.
    /// </summary>
    public KeyCode resetKey;
    /// <summary>
    /// Reset flex key. When this key is pressed, it recalibrates the Gloveone fingers.
    /// </summary>
    public KeyCode resetFlexKey;

    /// <summary>
    /// Smoothness factor. 0 is raw data; 1 is smoothed.
    /// </summary>
    [Range(0, 1)]
    public float smoothness;

    /// <summary>
    /// Instance access
    /// </summary>
    public static NDInitializer Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<NDInitializer>();

            if (_instance == null)
            {
                GameObject go = new GameObject("NDObjects");
                _instance = go.AddComponent<NDInitializer>();
            }

            return _instance;
        }
    }

    /// <summary>
    /// When the application is closed, clean the instance
    /// </summary>
    private void OnApplicationQuit()
    {
        _instance = null;
    }


    /// <summary>
    /// Don't destroy on load the HapticSystem
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Wake up instances
        NDService service = NDService.Instance;
        service.transform.SetParent(transform);
        service.resetKey = resetKey;
        service.resetFlexKey = resetFlexKey;

        if (useContacts)
        {
            ContactsSystem contacts = ContactsSystem.Instance;
            contacts.transform.SetParent(transform);
        }

        if (useHaptics)
        {
            HapticSystem haptic = HapticSystem.Instance;
            haptic.transform.SetParent(transform);

            haptic.ReadSensations();
        }
    }

    private void Update()
    {
        NDService.SetSmoothness(smoothness);
    }
}
