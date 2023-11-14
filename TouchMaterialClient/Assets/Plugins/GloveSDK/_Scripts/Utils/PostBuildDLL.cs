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

using System.IO;
using UnityEngine;
using System;

#if UNITY_5_6_OR_NEWER && UNITY_EDITOR && !UNITY_ANDROID
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

class PostBuildDLL : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        // Gets the app folder path.
        string appPath = report.summary.outputPath.Substring(0, report.summary.outputPath.LastIndexOf("/") + 1);
        // Gets the app name.
        string appName = report.summary.outputPath.Substring(report.summary.outputPath.LastIndexOf("/") + 1,
            report.summary.outputPath.LastIndexOf(".") - (report.summary.outputPath.LastIndexOf("/") + 1));
        // Copy the NDAPI dll to the app folder.
        try
        {
            File.Copy(appPath + "\\" + appName + "_Data\\Plugins\\NDAPI_x64.dll", appPath + "\\NDAPI_x64.dll");
            Debug.Log("API export succesful");
        }
        catch (Exception)
        {
            Debug.Log("DLL copy error");
        }
    }
}

#else
class PostBuildDLL
{
}
#endif