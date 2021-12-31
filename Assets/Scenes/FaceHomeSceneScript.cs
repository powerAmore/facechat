using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
# if PLATFORM_ANDROID
using UnityEngine.Android;
# endif
using trtc;
using FaceChat;

using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.native;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using com.tencent.imsdk.unity.utils;
using com.tencent.imsdk.unity.callback;
using AOT;

namespace FaceChat
{
    public class FaceHomeSceneScript : MonoBehaviour
    {
        public GameObject settingPrefab;
        public RectTransform mainCanvas;

        void Start()
        {
            #if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
            #endif
            
            transform.Find("editUserID").GetComponent<InputField>().text = DataManager.GetInstance().GetUserID();
            transform.Find("editRoomID").GetComponent<InputField>().text = DataManager.GetInstance().GetRoomID().ToString();

            Button enterRoomBtn = transform.Find("btnEnterRoom").gameObject.GetComponent<Button>();
            enterRoomBtn.onClick.AddListener(this.OnEnterRoomClick);

            //Button showSettingBtn = transform.Find("btnShowSetting").gameObject.GetComponent<Button>();
            //showSettingBtn.onClick.AddListener(this.OnShowSettingClick);

            //Button showApiTestBtn = transform.Find("BtnApiTest").gameObject.GetComponent<Button>();
            //showApiTestBtn.onClick.AddListener(this.OnShowApiTestClick);

            ITRTCCloud mTRTCCloud = ITRTCCloud.getTRTCShareInstance();
            string version = mTRTCCloud.getSDKVersion();
            transform.Find("lblTextVersion").GetComponent<Text>().text = "version:"+version;
        }

        void Update()
        {

        }

        void OnDestroy()
        {

        }

        [MonoPInvokeCallback(typeof(ValueCallback))]
        void CustomValueCallback(int code, string desc, string json_param, string user_data)
        {
            Utils.Log("code:" + code.ToString() + "; desc:" + desc + "; json_param:" + json_param + "; user_data:" + user_data);
        }

        void OnEnterRoomClick()
        {
            //ITRTCCloud mTRTCCloud = ITRTCCloud.getTRTCShareInstance();
            //ITXDeviceManager tXDeviceManager = mTRTCCloud.getDeviceManager();
            //ITXDeviceInfo ls = tXDeviceManager.GetCurrentDevice(TXMediaDeviceType.TXMediaDeviceTypeCamera);
            //transform.Find("lblTextVersion").GetComponent<Text>().text = "DevicePID:" + ls.DeviceName;
            //return;
            string userID = transform.Find("editUserID").GetComponent<InputField>().text;
            string roomID = transform.Find("editRoomID").GetComponent<InputField>().text;

            Debug.Log("OnEnterRoomClick，用户ID: " + userID);
            Debug.Log("OnEnterRoomClick，房间ID: " + roomID);

            DataManager.GetInstance().SetUserID(userID);
            DataManager.GetInstance().SetRoomID(roomID);
            
            if (GenerateTestUserSig.SDKAPPID != 0 && !string.IsNullOrEmpty(GenerateTestUserSig.SECRETKEY)) 
            {
                SceneManager.LoadScene("FaceRoomScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.Assert(false, "Please fill in your sdkappid && secretkey first");
            }
        }

        //void OnShowSettingClick()
        //{
        //    var setting = Instantiate(settingPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //    setting.transform.SetParent(mainCanvas.transform, false);
        //}

        //void OnShowApiTestClick()
        //{
        //    SceneManager.LoadScene("AudioApiTest", LoadSceneMode.Single);
        //}
    }
}