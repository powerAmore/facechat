using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif
using trtc;
using FaceChat;
# if PLATFORM_ANDROID
using UnityEngine.Android;
# endif

using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.native;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using com.tencent.imsdk.unity.utils;
using com.tencent.imsdk.unity.callback;
using AOT;

namespace FaceChat
{
    public class FaceRoomSceneScript : MonoBehaviour, ITRTCCloudCallback
    {
        public RectTransform mainCanvas;
        public UserTableView userTableView;

        //public GameObject settingPrefab;
        public GameObject customCapturePrefab;
        //private SettingScript settingScript = null;
        private CustomCaptureScript customCaptureScript = null;

        private Toggle captureVideoToggle;
        private Toggle captureAudioToggle;
        private Toggle muteLocalVideoToggle;
        private Toggle muteLocalAudioToggle;

        private ITRTCCloud mTRTCCloud;
        private ARSession m_ARSession;

        //private Text m_infoText;
        //private Text m_infoText2;

        public byte[] m_byteFacialData;
        //public byte[] m_byteReceivedFacialData;
        private BlendShapesDataContainer m_BlendShapesDataContainer;

        private float time = 0;//记录已经经过多少秒
        private float during_time = 0;//记录已经经过多少秒
        private long timestamp = 0;
        private int during_count = 0;


        //[MonoPInvokeCallback(typeof(ValueCallback))]
        //void CustomValueCallback(int code, string desc, string json_param, string user_data)
        //{
        //    Utils.Log("code: " + code.ToString() + "; desc: " + desc + "; json_param: " + json_param + "; user_data: " + user_data);
        //}

        //[MonoPInvokeCallback(typeof(ValueCallback))]
        //void CustomLoginCallback(int code, string desc, string json_param, string user_data)
        //{
        //    Utils.Log("code: " + code.ToString() + "; desc: " + desc + "; json_param: " + json_param + "; user_data: " + user_data);
        //    if(code == 0)
        //    {
        //        GroupCreate();
        //    }
        //}

        //[MonoPInvokeCallback(typeof(ValueCallback))]
        //void CustomGroupCreateCallback(int code, string desc, string json_param, string user_data)
        //{
        //    Utils.Log("code: " + code.ToString() + "; desc: " + desc + "; json_param: " + json_param + "; user_data: " + user_data);
        //    if (code == 0 || code == 10021 || code == 10025)
        //    {
        //        // TIM加入群
        //        TIMResult resGroupJoin = TencentIMSDK.GroupJoin(DataManager.GetInstance().GetRoomID().ToString(), "Hello", CustomValueCallback);
        //        Utils.Log("TencentIMSDK.GroupJoin: " + resGroupJoin.ToString() + "; group_id: " + DataManager.GetInstance().GetRoomID().ToString());

        //        TencentIMSDK.AddRecvNewMsgCallback(RecvNewMsgCallback);
        //    }
        //}

        //[MonoPInvokeCallback(typeof(ValueCallback))]
        //void CustomOnDestoryLogoutCallback(int code, string desc, string json_param, string user_data)
        //{
        //    Utils.Log("code: " + code.ToString() + "; desc: " + desc + "; json_param: " + json_param + "; user_data: " + user_data);

        //    // TIM uninit
        //    TIMResult resUninit = TencentIMSDK.Uninit();
        //    Utils.Log("TencentIMSDK.Uninit: " + resUninit.ToString());
        //}


        //[MonoPInvokeCallback(typeof(RecvNewMsgCallback))]
        //void RecvNewMsgCallback(List<Message> message, string user_data)
        //{
        //    //Utils.Log("RecvNewMsgCallback 触发 消息条数: " + message.Count.ToString());
        //    foreach (Message msg in message)
        //    {
        //        //Utils.Log("RecvNewMsgCallback 触发 消息ID：" + msg.message_msg_id +
        //        //"; 消息发送者: " + msg.message_sender +
        //        //"; 消息元素列表条数: " + msg.message_elem_array.Count.ToString());
        //        if (msg.message_sender != "@TIM#SYSTEM")
        //        {
        //            foreach (Elem msg_elem in msg.message_elem_array)
        //            {
        //                //Utils.Log("RecvNewMsgCallback 触发 消息ID：" + msg.message_msg_id +
        //                //            "; 消息发送者: " + msg.message_sender +
        //                //            "; 消息元素类型: " + msg_elem.elem_type.ToString() +
        //                //            "; 消息元素类型: " + msg_elem.text_elem_content);
        //                if (msg_elem.elem_type == TIMElemType.kTIMElem_Text)
        //                {
        //                    //m_infoText2.text = msg_elem.text_elem_content;
        //                }
        //            }
        //        }

        //    }

        //}

        //[MonoPInvokeCallback(typeof(LogCallback))]
        //void LogCallback(TIMLogLevel logLevel, string log, string user_data)
        //{
        //    Utils.Log("LogCallback 触发 log：" + log + "user_data :" + user_data);
        //}

        //[MonoPInvokeCallback(typeof(ConvEventCallback))]
        //void ConvEventCallback(TIMConvEvent conv_event, List<ConvInfo> conv_list, string user_data)
        //{
        //    Utils.Log("ConvEventCallback 触发 " + "user_data :" + user_data);
        //}

        void Awake()
        {
            //Application.targetFrameRate = 30;
        }

        void Start()
        {
            m_ARSession = GameObject.Find("AR Session").GetComponent<ARSession>();
            m_ARSession.enabled = false;
            //m_infoText = GameObject.Find("DataTunnel").GetComponent<Text>();
            //m_infoText2 = GameObject.Find("DataTunnel2").GetComponent<Text>();
            m_byteFacialData = GameObject.Find("AR Session Origin").GetComponent<BlendShapesDataContainer>().byteFacialData;
            m_BlendShapesDataContainer = GameObject.Find("AR Session Origin").GetComponent<BlendShapesDataContainer>();

            //Toggle toggleSetting = transform.Find("PanelTest/Viewport/Content/ToggleSetting").gameObject.GetComponent<Toggle>();
            //toggleSetting.onValueChanged.AddListener(this.OnToggleSetting);

            Toggle toggleSendSEIMsg = transform.Find("PanelTest/Viewport/Content/ToggleSendSEIMsg").gameObject.GetComponent<Toggle>();
            toggleSendSEIMsg.onValueChanged.AddListener(this.OnToggleSendSEIMsg);

            //Toggle toggleStartPublishing = transform.Find("PanelTest/Viewport/Content/ToggleStartPublishing").gameObject.GetComponent<Toggle>();
            //toggleStartPublishing.onValueChanged.AddListener(this.OnTogglePublishing);

            //Toggle toggleCustomCapture = transform.Find("PanelOperate/Viewport/Content/ToggleCustomCapture").gameObject.GetComponent<Toggle>();
            //toggleCustomCapture.onValueChanged.AddListener(this.OnToggleCustomCapture);

            //Toggle beautySet = transform.Find("PanelOperate/Viewport/Content/Beauty").gameObject.GetComponent<Toggle>();
            //beautySet.onValueChanged.AddListener(this.OnToggleBeauty);

            //Toggle videoMirror = transform.Find("PanelOperate/Viewport/Content/VideoMirror").gameObject.GetComponent<Toggle>();
            //videoMirror.onValueChanged.AddListener(this.OnToggleVideoMirror);

            //Toggle screenCapture = transform.Find("PanelOperate/Viewport/Content/ToggleScreenCapture").gameObject.GetComponent<Toggle>();
            //screenCapture.onValueChanged.AddListener(this.OnToggleScreenCapture);

            Button leaveRoomButton = transform.Find("PanelOperate/Viewport/Content/BtnLeaveRoom").gameObject.GetComponent<Button>();
            leaveRoomButton.onClick.AddListener(this.OnLeaveRoomClick);

            captureAudioToggle = transform.Find("PanelOperate/Viewport/Content/ToggleMic").gameObject.GetComponent<Toggle>();
            captureAudioToggle.onValueChanged.AddListener(this.OnToggleMic);

            muteLocalAudioToggle = transform.Find("PanelOperate/Viewport/Content/ToggleMuteLocalAudio").gameObject.GetComponent<Toggle>();
            muteLocalAudioToggle.onValueChanged.AddListener(this.OnToggleMuteLocalAudio);

            Toggle toggleMuteRemoteAudio = transform.Find("PanelOperate/Viewport/Content/ToggleMuteRemoteAudio").gameObject.GetComponent<Toggle>();
            toggleMuteRemoteAudio.onValueChanged.AddListener(this.OnToggleMuteRemoteAudio);

            captureVideoToggle = transform.Find("PanelOperate/Viewport/Content/ToggleCamera").gameObject.GetComponent<Toggle>();
            captureVideoToggle.onValueChanged.AddListener(this.OnToggleCamera);

            //muteLocalVideoToggle = transform.Find("PanelOperate/Viewport/Content/ToggleMuteLocalVideo").gameObject.GetComponent<Toggle>();
            //muteLocalVideoToggle.onValueChanged.AddListener(this.OnToggleMuteLocalVideo);

            //Toggle toggleMuteRemoteVideo = transform.Find("PanelOperate/Viewport/Content/ToggleMuteRemoteVideo").gameObject.GetComponent<Toggle>();
            //toggleMuteRemoteVideo.onValueChanged.AddListener(this.OnToggleMuteRemoteVideo);

            Toggle toggleShowConsole = transform.Find("PanelTest/Viewport/Content/ToggleShowConsole").gameObject.GetComponent<Toggle>();
            toggleShowConsole.onValueChanged.AddListener(this.OnToggleShowConsole);

            Toggle toggleShowUserVolume = transform.Find("PanelTest/Viewport/Content/ToggleShowUserVolume").gameObject.GetComponent<Toggle>();
            toggleShowUserVolume.onValueChanged.AddListener(this.OnToggleShowUserVolume);

            Toggle toggleShowStatis = transform.Find("PanelTest/Viewport/Content/ToggleShowStatis").gameObject.GetComponent<Toggle>();
            toggleShowStatis.onValueChanged.AddListener(this.OnToggleShowStatis);

            //Toggle toggleSwitchCamera = transform.Find("PanelTest/Viewport/Content/ToggleSwitchCamera").gameObject.GetComponent<Toggle>();
            //toggleSwitchCamera.onValueChanged.AddListener(this.OnToggleSwitchCamera);

            //Toggle toggleSetMixTranscodingConfig = transform.Find("PanelTest/Viewport/Content/ToggleSetMixTranscodingConfig").gameObject.GetComponent<Toggle>();
            //toggleSetMixTranscodingConfig.onValueChanged.AddListener(this.OnToggleSetMixTranscodingConfig);

            mTRTCCloud = ITRTCCloud.getTRTCShareInstance();
            mTRTCCloud.addCallback(this);

            string version = mTRTCCloud.getSDKVersion();
            LogManager.Log("trtc sdk version is : " + version);

            TRTCParams trtcParams = new TRTCParams();
            trtcParams.sdkAppId = GenerateTestUserSig.SDKAPPID;
            trtcParams.roomId = uint.Parse(DataManager.GetInstance().GetRoomID());
            trtcParams.strRoomId = trtcParams.roomId.ToString();
            trtcParams.userId = DataManager.GetInstance().GetUserID();
            Debug.Log("FaceRoomSenceStart，用户ID: " + trtcParams.userId);
            Debug.Log("FaceRoomSenceStart，房间ID: " + trtcParams.roomId);

            trtcParams.userSig = GenerateTestUserSig.GetInstance().GenTestUserSig(DataManager.GetInstance().GetUserID());
            Debug.Log("FaceRoomSenceStart，userSig: " + trtcParams.userSig);
            // 如果您有进房权限保护的需求，则参考文档{https://cloud.tencent.com/document/product/647/32240}完成该功能。
            // 在有权限进房的用户中的下面字段中添加在服务器获取到的privateMapKey。
            trtcParams.privateMapKey = "";
            trtcParams.businessInfo = "";
            trtcParams.role = DataManager.GetInstance().roleType;
            Debug.Log("FaceRoomSenceStart，roleType: " + DataManager.GetInstance().roleType);
            TRTCAppScene scene = DataManager.GetInstance().appScene;
            Debug.Log("FaceRoomSenceStart，appScene: " + DataManager.GetInstance().appScene);
            mTRTCCloud.enterRoom(ref trtcParams, scene);
            SetLocalAVStatus();
            TRTCVideoEncParam videoEncParams = DataManager.GetInstance().videoEncParam;
            mTRTCCloud.setVideoEncoderParam(ref videoEncParams);

            TRTCNetworkQosParam qosParams = DataManager.GetInstance().qosParams;      // 网络流控相关参数设置
            mTRTCCloud.setNetworkQosParam(ref qosParams);

            LogManager.Log("Scene:" + scene + ", Role:" + trtcParams.role + ", Qos-Prefer:" + qosParams.preference + ", Qos-CtrlMode:" + qosParams.controlMode);

            userTableView.DoMuteAudio += new UserTableView.MuteAudioHandler(OnMuteRemoteAudio);
            //userTableView.DoMuteVideo += new UserTableView.MuteVideoHandler(OnMuteRemoteVideo);
            DataManager.GetInstance().DoRoleChange += new DataManager.ChangeRoleHandler(OnRoleChanged);
            DataManager.GetInstance().DoVideoEncParamChange += new DataManager.ChangeVideoEncParamHandler(OnVideoEncParamChanged);
            DataManager.GetInstance().DoQosParamChange += new DataManager.ChangeQosParamHandler(OnQosParamChanged);

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
            // TIM初始化
            //SdkConfig sdkConfig = new SdkConfig();
            //TIMResult resInit = TencentIMSDK.Init(GenerateTestUserSig.SDKAPPID, sdkConfig);
            //Utils.Log("TencentIMSDK.Init: " + resInit.ToString());

            // TIM登录
            //TIMLoginStatus resLoginStatus = TencentIMSDK.GetLoginStatus();
            //Utils.Log("GetLoginStatus: " + resLoginStatus.ToString());
            //if (resLoginStatus == TIMLoginStatus.kTIMLoginStatus_Logouting || resLoginStatus == TIMLoginStatus.kTIMLoginStatus_UnLogined)
            //{
            //    TIMResult resLogin = TencentIMSDK.Login(trtcParams.userId, trtcParams.userSig, CustomLoginCallback);
            //    Utils.Log("TencentIMSDK.Login: " + resInit.ToString());
            //}
            //else //用户已登录直接创建房间；若未登录则在登录回调里创建房间
            //{
            //    GroupCreate();
            //}

         }

        //// TIM创建群
        //void GroupCreate()
        //{
        //    CreateGroupParam param = new CreateGroupParam(); // 这个message可以是业务的其他message实例
        //    param.create_group_param_group_id = DataManager.GetInstance().GetRoomID().ToString();
        //    param.create_group_param_group_name = DataManager.GetInstance().GetRoomID().ToString();
        //    param.create_group_param_group_type = TIMGroupType.kTIMGroup_AVChatRoom;
        //    param.create_group_param_add_option = TIMGroupAddOption.kTIMGroupAddOpt_Any;
        //    TIMResult resGroupCreate = TencentIMSDK.GroupCreate(param, CustomGroupCreateCallback);
        //    Utils.Log("TencentIMSDK.GroupCreate: " + resGroupCreate.ToString() + "; group_id: " + param.create_group_param_group_id);

        //    TencentIMSDK.MsgSetGroupReceiveMessageOpt(DataManager.GetInstance().GetRoomID().ToString(), TIMReceiveMessageOpt.kTIMRecvMsgOpt_Not_Notify, CustomValueCallback);
        //}

        void Update()
        {
            var fps = 1.0f / Time.deltaTime;
            Debug.Log("fps: " + fps.ToString());
            //Debug.Log("m_byteFacialData.Length: " + m_byteFacialData.Length);
            time += Time.deltaTime;
            if (time > 0.03)
            {
                Debug.Log("time: " + time);
                time = 0;
                CustomSendSEIMsg();
                //SendCustomCmdMsg();
            }

        }

        void CustomSendSEIMsg()
        {
            if (DataManager.GetInstance().captureAudio && m_ARSession.enabled)
            {
                //byte[] seiMsg = new byte[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                byte[] seiMsg = m_byteFacialData.ToArray();

                string strInfo = "";
                for (int i = 0; i < seiMsg.Length; i++)
                {
                    strInfo += seiMsg[i].ToString() + ", ";
                }
                Debug.Log("seiMsg.Length: " + seiMsg.Length);
                Debug.Log("seiMsg strInfo: " + strInfo);

                var result = mTRTCCloud.sendSEIMsg(seiMsg, seiMsg.Length, 1);
                //var result = mTRTCCloud.sendSEIMsg(System.Text.Encoding.Default.GetBytes(m_byteFacialData), System.Text.Encoding.Default.GetByteCount(seiMsg), 3);

                //Debug.Log("m_byteFacialData.Length: " + m_byteFacialData.Length + ", m_byteFacialData[0]: " + m_byteFacialData[0]);
            }
            
        }

        void SendCustomCmdMsg()
        {
            if (DataManager.GetInstance().captureAudio && m_ARSession.enabled)
            {
                //byte[] seiMsg = new byte[] {2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                byte[] cmdMsg = m_byteFacialData.ToArray();

                string strInfo = "";
                for (int i = 0; i < cmdMsg.Length; i++)
                {
                    strInfo += cmdMsg[i].ToString() + ", ";
                }
                Debug.Log("cmdMsg.Length: " + cmdMsg.Length);
                Debug.Log("cmdMsg strInfo: " + strInfo);

                var result = mTRTCCloud.sendCustomCmdMsg(1, cmdMsg, cmdMsg.Length, true, true);
                Debug.Log("result: " + result);
            }
        }

        void MsgSendMessage()
        {
            string conv_id = DataManager.GetInstance().GetRoomID().ToString();
            Message message = new Message();
            message.message_conv_id = conv_id;
            message.message_conv_type = TIMConvType.kTIMConv_Group;
            List<Elem> messageElems = new List<Elem>();
            Elem textMessage = new Elem();
            textMessage.elem_type = TIMElemType.kTIMElem_Text;
            //textMessage.text_elem_content = m_infoText.text.ToString();
            messageElems.Add(textMessage);
            message.message_elem_array = messageElems;
            StringBuilder messageId = new StringBuilder(512);

            //TIMResult res = TencentIMSDK.MsgSendMessage(conv_id, TIMConvType.kTIMConv_Group, message, messageId, CustomValueCallback);
            //Utils.Log(((int)res).ToString());
            //Utils.Log(messageId.ToString()); // 同步返回消息ID
        }

        void OnDestroy()
        {
            DataManager.GetInstance().DoRoleChange -= new DataManager.ChangeRoleHandler(OnRoleChanged);
            DataManager.GetInstance().DoVideoEncParamChange -= new DataManager.ChangeVideoEncParamHandler(OnVideoEncParamChanged);
            DataManager.GetInstance().DoQosParamChange -= new DataManager.ChangeQosParamHandler(OnQosParamChanged);
            userTableView.DoMuteAudio -= new UserTableView.MuteAudioHandler(OnMuteRemoteAudio);
            //userTableView.DoMuteVideo -= new UserTableView.MuteVideoHandler(OnMuteRemoteVideo);

            mTRTCCloud.removeCallback(this);
            ITRTCCloud.destroyTRTCShareInstance();
            DataManager.GetInstance().ResetLocalAVFlag();

            //// 取消接收新消息回调
            //TencentIMSDK.RemoveRecvNewMsgCallback();
            //// TIM退出登录, 在退出登录的回调里进行TIM uninit
            //TIMResult resLogout = TencentIMSDK.Logout(CustomOnDestoryLogoutCallback);
            //Utils.Log("TencentIMSDK.Logout: " + resLogout.ToString());
        }

        void OnLeaveRoomClick()
        {
            LogManager.Log("OnLeaveRoomClick");
            mTRTCCloud.exitRoom();
            DataManager.GetInstance().ResetLocalAVFlag();
        }

        //void OnToggleBeauty(bool value)
        //{
        //    if(value) {
        //        mTRTCCloud.setBeautyStyle(TRTCBeautyStyle.TRTCBeautyStyleSmooth, 9, 9, 9);
        //    } else {
        //        mTRTCCloud.setBeautyStyle(TRTCBeautyStyle.TRTCBeautyStyleSmooth, 0, 0, 0);
        //    }
        //}

        //void OnToggleVideoMirror(bool value)
        //{
        //    if(value) {
        //        mTRTCCloud.setVideoEncoderMirror(true);
        //    } else {
        //        mTRTCCloud.setVideoEncoderMirror(false);
        //    }
        //}


        private void SetLocalAVStatus()
        {
            TRTCRoleType role = DataManager.GetInstance().roleType;
            bool captureVideo = DataManager.GetInstance().captureVideo;
            bool muteLocalVideo = DataManager.GetInstance().muteLocalVideo;
            bool captureAudio = DataManager.GetInstance().captureAudio;
            bool muteLocalAudio = DataManager.GetInstance().muteLocalAudio;
            bool isAudience = (role == TRTCRoleType.TRTCRoleAudience);
            if (isAudience)
            {
                captureVideo = false;
                captureAudio = false;
            }

            if (captureVideo)
            {
                mTRTCCloud.startLocalPreview(true, null);
                userTableView.UpdateVideoAvailable("", TRTCVideoStreamType.TRTCVideoStreamTypeBig, true);
            }
            else
            {
                mTRTCCloud.stopLocalPreview();
                userTableView.UpdateVideoAvailable("", TRTCVideoStreamType.TRTCVideoStreamTypeBig, false);
            }
            mTRTCCloud.muteLocalVideo(muteLocalVideo);

            if (captureAudio)
            {
                mTRTCCloud.startLocalAudio(TRTCAudioQuality.TRTCAudioQualityDefault);
            }
            else
            {
                mTRTCCloud.stopLocalAudio();
            }
            mTRTCCloud.muteLocalAudio(muteLocalAudio);
            //captureVideoToggle.interactable = !isAudience;
            //captureVideoToggle.SetIsOnWithoutNotify(captureVideo);
            //captureAudioToggle.interactable = !isAudience;
            //captureAudioToggle.SetIsOnWithoutNotify(captureAudio);
            //muteLocalVideoToggle.interactable = !isAudience;
            //muteLocalVideoToggle.SetIsOnWithoutNotify(muteLocalVideo);
            //muteLocalAudioToggle.interactable = !isAudience;
            //muteLocalAudioToggle.SetIsOnWithoutNotify(muteLocalAudio);
        }

        void OnToggleMic(bool value)
        {
            LogManager.Log("OnToggleMic: " + value);
            if (value)
            {
                mTRTCCloud.startLocalAudio(TRTCAudioQuality.TRTCAudioQualityDefault);
                m_ARSession.enabled = true;
                LogManager.Log("m_ARSession.enabled: " + m_ARSession.enabled.ToString());
                mTRTCCloud.callExperimentalAPI("{\"api\":\"enableBlackStream\", \"params\": {\"enable\":1}}");
            }
            else
            {
                mTRTCCloud.callExperimentalAPI("{\"api\":\"enableBlackStream\", \"params\": {\"enable\":0}}");
                mTRTCCloud.stopLocalAudio();
                m_ARSession.enabled = false;
            }
            DataManager.GetInstance().captureAudio = value;
        }

        void OnToggleMuteLocalAudio(bool value)
        {
            LogManager.Log("OnToggleMuteLocalAudio: " + value);
            mTRTCCloud.muteLocalAudio(value);
            DataManager.GetInstance().muteLocalAudio = value;
        }

        void OnToggleMuteRemoteAudio(bool value)
        {
            LogManager.Log("OnToggleMuteRemoteAudio: " + value);
            mTRTCCloud.muteAllRemoteAudio(value);
        }

        void OnToggleCamera(bool value)
        {
            LogManager.Log("OnToggleCamera: " + value);
            if (value)
            {
                mTRTCCloud.startLocalAudio(TRTCAudioQuality.TRTCAudioQualityDefault);
                m_ARSession.enabled = true;
                LogManager.Log("m_ARSession.enabled: " + m_ARSession.enabled.ToString());

                var customCapture = Instantiate(customCapturePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                customCapture.transform.SetParent(mainCanvas.transform, false);
                customCaptureScript = customCapture.GetComponent<CustomCaptureScript>();
                customCaptureScript.AudioCallback += new CustomCaptureScript.OnCustomCaptureAudioCallback(CustomCaptureAudioCallback);
                customCaptureScript.VideoCallback += new CustomCaptureScript.OnCustomCaptureVideoCallback(CustomCaptureVideoCallback);
                customCaptureScript.StartCustomBlackVideo();
            }
            else
            {
                customCaptureScript.StopCustomBlackVideo();
                if (customCaptureScript != null)
                {
                    Transform.Destroy(customCaptureScript.gameObject);
                    customCaptureScript = null;
                }

                mTRTCCloud.stopLocalAudio();
                m_ARSession.enabled = false;

            }
            DataManager.GetInstance().captureAudio = value;
        }

        //void OnToggleCamera(bool value)
        //{
        //    LogManager.Log("OnToggleCamera: " + value);
        //    if (value)
        //    {
        //        mTRTCCloud.startLocalPreview(true, null);
        //        userTableView.UpdateVideoAvailable("", TRTCVideoStreamType.TRTCVideoStreamTypeBig, true);
        //        m_ARSession.enabled = true;
        //        LogManager.Log("m_ARSession.enabled: " + m_ARSession.enabled.ToString());
        //    }
        //    else
        //    {
        //        mTRTCCloud.stopLocalPreview();
        //        userTableView.UpdateVideoAvailable("", TRTCVideoStreamType.TRTCVideoStreamTypeBig, false);
        //        m_ARSession.enabled = false;
        //        LogManager.Log("m_ARSession.enabled: " + m_ARSession.enabled.ToString());
        //    }
        //    DataManager.GetInstance().captureAudio = value;
        //}

        //void OnToggleMuteLocalVideo(bool value)
        //{
        //    LogManager.Log("OnToggleMuteLocalVideo: " + value);
        //    mTRTCCloud.muteLocalVideo(value);
        //    DataManager.GetInstance().muteLocalVideo = value;
        //}

        //void OnToggleMuteRemoteVideo(bool value)
        //{
        //    LogManager.Log("OnToggleMuteRemoteVideo: " + value);
        //    mTRTCCloud.muteAllRemoteVideoStreams(value);
        //}

        void OnToggleShowConsole(bool value)
        {
            transform.Find("LogDisplayView").gameObject.SetActive(value);
        }

        void OnToggleShowUserVolume(bool value)
        {
            if (value)
            {
                mTRTCCloud.enableAudioVolumeEvaluation(300);
            }
            else
            {
                mTRTCCloud.enableAudioVolumeEvaluation(0);
            }
            userTableView.UpdateAudioVolumeVisible(value);
        }

        void OnToggleShowStatis(bool value)
        {
            userTableView.UpdateUserStatisVisible(value);
        }

        //void OnToggleSwitchCamera(bool value)
        //{
        //    LogManager.Log("OnToggleSwitchCamera: " + value );
        //    mTRTCCloud.getDeviceManager().switchCamera(!value);
        //}

        //void OnToggleSetMixTranscodingConfig(bool value)
        //{
        //    TRTCTranscodingConfig transcodingConfig =new TRTCTranscodingConfig();
        //    transcodingConfig.appId=1252463788;
        //    transcodingConfig.bizId =3891;
        //    transcodingConfig.videoWidth =360;
        //    transcodingConfig.mode = TRTCTranscodingConfigMode.TRTCTranscodingConfigMode_Manual;
        //    transcodingConfig.videoHeight =640;
        //    transcodingConfig.videoFramerate =15;
        //    transcodingConfig.videoGOP = 2;
        //    transcodingConfig.videoBitrate = 1000;
        //    transcodingConfig.audioBitrate = 64;
        //    transcodingConfig.audioSampleRate = 48000;
        //    transcodingConfig.audioChannels = 2;
        //    //查看
        //    //http://liteavapp.qcloud.com/live/streamIdtest.flv
        //    transcodingConfig.streamId = "streamIdtest";
        //    TRTCMixUser[] mixUsersArray = new TRTCMixUser[2];
        //    mixUsersArray[0].userId = DataManager.GetInstance().GetUserID();
        //    mixUsersArray[0].zOrder = 4; // zOrder 为0代表主播画面位于最底层
        //    mixUsersArray[0].streamType = 0;
        //    mixUsersArray[0].rect.left = 0;
        //    mixUsersArray[0].rect.top = 0;
        //    mixUsersArray[0].rect.right = 360;
        //    mixUsersArray[0].rect.bottom = 640;

        //    mixUsersArray[1].userId = "110";
        //    mixUsersArray[1].zOrder = 5;
        //    mixUsersArray[1].streamType = 0;
        //    mixUsersArray[1].rect.left = 100; //仅供参考
        //    mixUsersArray[1].rect.top = 100; //仅供参考
        //    mixUsersArray[1].rect.right = 100; //仅供参考
        //    mixUsersArray[1].rect.bottom = 100; //仅供参考
        //    mixUsersArray[1].roomId = DataManager.GetInstance().GetRoomID(); // 本地用户不用填写 roomID，远程需要

        //    transcodingConfig.mixUsersArray = mixUsersArray;
        //    transcodingConfig.mixUsersArraySize = 2;
        //    if(value)
        //        mTRTCCloud.setMixTranscodingConfig(transcodingConfig);
        //    else
        //        mTRTCCloud.setMixTranscodingConfig(null);
        //}

        void OnMuteRemoteAudio(string userId, bool mute)
        {
            LogManager.Log("MuteRemoteAudio: " + userId + "-" + mute);
            mTRTCCloud.muteRemoteAudio(userId, mute);
        }

        //void OnMuteRemoteVideo(string userId, bool mute)
        //{
        //    LogManager.Log("MuteRemoteVideo: " + userId + "-" + mute);
        //    mTRTCCloud.muteRemoteVideoStream(userId, mute);
        //}

        //void OnToggleSetting(bool value)
        //{
        //    if (value)
        //    {
        //        var setting = Instantiate(settingPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //        setting.transform.SetParent(mainCanvas.transform, false);
        //        settingScript = setting.GetComponent<SettingScript>();
        //    }
        //    else
        //    {
        //        if(settingScript!=null){
        //            Transform.Destroy(settingScript.gameObject);
        //            settingScript = null;
        //        }
        //    }
        //}

        void OnToggleSendSEIMsg(bool value)
        {
            if (value)
            {
                string seiMsg = "test sei message";
                var result = mTRTCCloud.sendSEIMsg(System.Text.Encoding.Default.GetBytes(seiMsg), System.Text.Encoding.Default.GetByteCount(seiMsg), 3);
                LogManager.Log("SendSEIMsg: " + result);
            }
        }

        //void OnTogglePublishing(bool value)
        //{
        //    Toggle toggleStartPublishing = transform.Find("PanelTest/Viewport/Content/ToggleStartPublishing").gameObject.GetComponent<Toggle>();
        //    if (value)
        //    {
        //        mTRTCCloud.startPublishing("test", TRTCVideoStreamType.TRTCVideoStreamTypeBig);
        //    }
        //    else
        //    {
        //        mTRTCCloud.stopPublishing();

        //    }
        //}

//        void OnToggleScreenCapture(bool value)
//        {
//#if UNITY_STANDALONE_WIN
//            if (value)
//            {
//                TRTCScreenCaptureSourceInfo[] sources = mTRTCCloud.getScreenCaptureSources();
//                if (sources.Length > 0)
//                {
//                    mTRTCCloud.selectScreenCaptureTarget(sources[0], new Rect(0, 0, 640, 360), new TRTCScreenCaptureProperty());
//                    TRTCVideoEncParam videoEncParam = new TRTCVideoEncParam()
//                    {
//                        videoResolution = TRTCVideoResolution.TRTCVideoResolution_640_360,
//                        resMode = TRTCVideoResolutionMode.TRTCVideoResolutionModeLandscape,
//                        videoFps = 15,
//                        videoBitrate = 550,
//                        minVideoBitrate = 550
//                    };
//                    mTRTCCloud.startScreenCapture(TRTCVideoStreamType.TRTCVideoStreamTypeSub, ref videoEncParam);
//                    userTableView.AddUser("", TRTCVideoStreamType.TRTCVideoStreamTypeSub);
//                    userTableView.UpdateVideoAvailable("", TRTCVideoStreamType.TRTCVideoStreamTypeSub, true);
//                }
//            }
//            else
//            {
//                mTRTCCloud.stopScreenCapture();
//                userTableView.UpdateVideoAvailable("", TRTCVideoStreamType.TRTCVideoStreamTypeSub, false);
//                userTableView.RemoveUser("", TRTCVideoStreamType.TRTCVideoStreamTypeSub);
//            }
//#endif
//        }

        void OnToggleCustomCapture(bool value)
        {
            if (value)
            {
                var customCapture = Instantiate(customCapturePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                customCapture.transform.SetParent(mainCanvas.transform, false);
                customCaptureScript = customCapture.GetComponent<CustomCaptureScript>();
                customCaptureScript.AudioCallback += new CustomCaptureScript.OnCustomCaptureAudioCallback(CustomCaptureAudioCallback);
                customCaptureScript.VideoCallback += new CustomCaptureScript.OnCustomCaptureVideoCallback(CustomCaptureVideoCallback);
            }
            else
            {
                if (customCaptureScript != null)
                {
                    Transform.Destroy(customCaptureScript.gameObject);
                    customCaptureScript = null;
                }
            }
        }

        void OnRoleChanged()
        {
            SetLocalAVStatus();
            mTRTCCloud.switchRole(DataManager.GetInstance().roleType);
        }

        void OnVideoEncParamChanged()
        {
            TRTCVideoEncParam videoEncParams = DataManager.GetInstance().videoEncParam;
            mTRTCCloud.setVideoEncoderParam(ref videoEncParams);
        }

        void OnQosParamChanged()
        {
            TRTCNetworkQosParam qosParams = DataManager.GetInstance().qosParams;      // 网络流控相关参数设置
            mTRTCCloud.setNetworkQosParam(ref qosParams);
        }

        void CustomCaptureAudioCallback(bool stop)
        {
            if (!stop)
            {
                mTRTCCloud.stopLocalAudio();
            }
            else
            {
                Toggle toggleMic = transform.Find("PanelOperate/Viewport/Content/ToggleMic").gameObject.GetComponent<Toggle>();
                if (!toggleMic.isOn)
                {
                    return;
                }
                mTRTCCloud.startLocalAudio(TRTCAudioQuality.TRTCAudioQualityDefault);
                Toggle toggleMuteLocalAudio = transform.Find("PanelOperate/Viewport/Content/ToggleMuteLocalAudio").gameObject.GetComponent<Toggle>();
                if (toggleMuteLocalAudio.isOn)
                {
                    mTRTCCloud.muteLocalAudio(true);
                }
                else
                {
                    mTRTCCloud.muteLocalAudio(false);
                }
            }
        }

        void CustomCaptureVideoCallback(bool stop)
        {
            if (!stop)
            {
                mTRTCCloud.stopLocalPreview();
            }
            else
            {
                Toggle toggelCamera = transform.Find("PanelOperate/Viewport/Content/ToggleCamera").gameObject.GetComponent<Toggle>();
                if (!toggelCamera.isOn)
                {
                    return;
                }

                mTRTCCloud.startLocalPreview(true, null);

                Toggle toggleMuteLocalVideo = transform.Find("PanelOperate/Viewport/Content/ToggleMuteLocalVideo").gameObject.GetComponent<Toggle>();
                if (toggleMuteLocalVideo.isOn)
                {
                    mTRTCCloud.muteLocalVideo(true);
                }
                else
                {
                    mTRTCCloud.muteLocalVideo(false);
                }
            }
        }

#region ITRTCCloudCallback
        public void onError(TXLiteAVError errCode, String errMsg, IntPtr arg)
        {
            LogManager.Log(String.Format("onError {0}, {1}", errCode, errMsg));
        }

        public void onWarning(TXLiteAVWarning warningCode, String warningMsg, IntPtr arg)
        {
            LogManager.Log(String.Format("onWarning {0}, {1}", warningCode, warningMsg));
        }

        public void onEnterRoom(int result)
        {
            LogManager.Log(String.Format("onEnterRoom {0}", result));
            userTableView.AddUser("", TRTCVideoStreamType.TRTCVideoStreamTypeBig);
        }

        public void onExitRoom(int reason)
        {
            LogManager.Log(String.Format("onExitRoom {0}", reason));
            userTableView.RemoveUser("", TRTCVideoStreamType.TRTCVideoStreamTypeBig);

            SceneManager.LoadScene("FaceHomeScene", LoadSceneMode.Single);
        }

        public void onSwitchRole(TXLiteAVError errCode, String errMsg)
        {
            LogManager.Log(String.Format("onSwitchRole {0}, {1}", errCode, errMsg));
        }

        public void onRemoteUserEnterRoom(String userId)
        {
            LogManager.Log(String.Format("onRemoteUserEnterRoom {0}", userId));
        }

        public void onRemoteUserLeaveRoom(String userId, int reason)
        {
            LogManager.Log(String.Format("onRemoteUserLeaveRoom {0}, {1}", userId, reason));
            userTableView.RemoveUser(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig);
        }

        public void onUserVideoAvailable(String userId, bool available)
        {
            LogManager.Log(String.Format("onUserVideoAvailable {0}, {1}", userId, available));
            userTableView.AddUser(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig);
            // Important: startRemoteView is needed for receiving video stream.
            if (available)
            {
                mTRTCCloud.startRemoteView(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig, null);
            }
            else
            {
                mTRTCCloud.stopRemoteView(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig);
            }
            userTableView.UpdateVideoAvailable(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig, available);
        }

        public void onUserSubStreamAvailable(String userId, bool available)
        {
            LogManager.Log(String.Format("onUserSubStreamAvailable {0}, {1}", userId, available));
            // Important: startRemoteView is needed for receiving video stream.
            if (available)
            {
                userTableView.AddUser(userId, TRTCVideoStreamType.TRTCVideoStreamTypeSub);
                userTableView.UpdateVideoAvailable(userId, TRTCVideoStreamType.TRTCVideoStreamTypeSub, available);
                mTRTCCloud.startRemoteView(userId, TRTCVideoStreamType.TRTCVideoStreamTypeSub, null);
            }
            else
            {
                mTRTCCloud.stopRemoteView(userId, TRTCVideoStreamType.TRTCVideoStreamTypeSub);
                userTableView.RemoveUser(userId, TRTCVideoStreamType.TRTCVideoStreamTypeSub);
            }
        }

        public void onUserAudioAvailable(String userId, bool available)
        {
            LogManager.Log(String.Format("onUserAudioAvailable {0}, {1}", userId, available));
            userTableView.AddUser(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig);
            userTableView.UpdateAudioAvailable(userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig, available);
        }

        //public void onLocalProcessedAudioFrame(TRTCAudioFrame frame)
        //{
        //    byte[] message = m_byteFacialData.ToArray();

        //    string strInfo = "";
        //    for (int i = 0; i < message.Length; i++)
        //    {
        //        strInfo += message[i].ToString() + ", ";
        //    }
        //    Debug.Log("seiMsg.Length: " + message.Length);
        //    Debug.Log("seiMsg strInfo: " + strInfo);

        //    frame.extraData = message;

        //}


        public void onFirstVideoFrame(String userId, TRTCVideoStreamType streamType, int width, int height)
        {
            LogManager.Log(String.Format("onFirstVideoFrame {0}, {1}, {2}, {3}", userId, streamType, width, height));
        }

        public void onFirstAudioFrame(String userId)
        {
            LogManager.Log(String.Format("onFirstAudioFrame {0}", userId));
        }

        public void onSendFirstLocalVideoFrame(TRTCVideoStreamType streamType)
        {
            LogManager.Log(String.Format("onSendFirstLocalVideoFrame {0}", streamType));
        }

        public void onSendFirstLocalAudioFrame()
        {
            LogManager.Log(String.Format("onSendFirstLocalAudioFrame"));
        }

        public void onNetworkQuality(TRTCQualityInfo localQuality, TRTCQualityInfo[] remoteQuality, UInt32 remoteQualityCount)
        {
            // LogManager.Log(String.Format("onNetworkQuality {0}, {1}, {2}", localQuality, remoteQuality, remoteQualityCount));
        }

        public void onStatistics(TRTCStatistics statis)
        {
            // LogManager.Log(String.Format("onStatistics {0}", statis));
            string localStatisText = "";
            foreach (TRTCLocalStatistics local in statis.localStatisticsArray)
            {
                localStatisText = string.Format("width: {0}\r\nheight: {1}\r\nvideoframerate: {2}\r\nvideoBitrate: {3}\r\naudioSampleRate: {4}\r\naudioBitrate:{5}\r\nstreamType:{6}\r\n",
                        local.width, local.height,
                        local.frameRate, local.videoBitrate, local.audioSampleRate, local.audioBitrate, local.streamType);
                userTableView.updateUserStatistics("", local.streamType, localStatisText);
            }
            foreach (TRTCRemoteStatistics remote in statis.remoteStatisticsArray)
            {
                string remoteStatisText = "";
                remoteStatisText = string.Format("finalLoss: {7}\r\njitterBufferDelay: {8}\r\nwidth: {0}\r\nheight: {1}\r\nvideoframerate: {2}\r\nvideoBitrate: {3}\r\naudioSampleRate: {4}\r\naudioBitrate:{5}\r\nstreamType:{6}\r\n",
                        remote.width, remote.height,
                        remote.frameRate, remote.videoBitrate, remote.audioSampleRate, remote.audioBitrate, remote.streamType,
                        remote.finalLoss, remote.jitterBufferDelay);
                userTableView.updateUserStatistics(remote.userId, remote.streamType, remoteStatisText);
            }
        }

        public void onConnectionLost()
        {
            LogManager.Log(String.Format("onConnectionLost"));
        }

        public void onTryToReconnect()
        {
            LogManager.Log(String.Format("onTryToReconnect"));
        }

        public void onConnectionRecovery()
        {
            LogManager.Log(String.Format("onConnectionRecovery"));
        }

        public void onCameraDidReady()
        {
            LogManager.Log(String.Format("onCameraDidReady"));
        }

        public void onMicDidReady()
        {
            LogManager.Log(String.Format("onMicDidReady"));
        }

        public void onUserVoiceVolume(TRTCVolumeInfo[] userVolumes, UInt32 userVolumesCount, UInt32 totalVolume)
        {
            
            foreach (TRTCVolumeInfo userVolume in userVolumes)
            {
                userTableView.UpdateAudioVolume(userVolume.userId, TRTCVideoStreamType.TRTCVideoStreamTypeBig, userVolume.volume);
            }
        }

        public void onDeviceChange(String deviceId, TRTCDeviceType type, TRTCDeviceState state)
        {
            LogManager.Log(String.Format("onSwitchRole {0}, {1}, {2}", deviceId, type, state));
        }

        public void onRecvCustomCmdMsg(String userId, int cmdID, int seq, Byte[] message, int messageSize)
        {

            LogManager.Log(String.Format("onRecvCustomCmdMsg {0}, {1} ,{2}, {3}", userId, cmdID, seq, messageSize));
            string strInfo = "";
            for (int i = 0; i < messageSize; i++)
            {
                strInfo += message[i].ToString() + ", ";
            }
            Debug.Log("strInfo: " + strInfo);


            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            if (timestamp == 0)
            {
                timestamp = unixTimeMilliseconds;
            }
            else
            {
                var delta_time = unixTimeMilliseconds - timestamp;
                timestamp = unixTimeMilliseconds;

                during_time += delta_time;
                if (during_time >= 1000)
                {
                    Debug.Log("during_time: " + during_time + ", during_count: " + during_count);
                    during_time = 0;
                    during_count = 0;
                }
                else
                {
                    during_count += 1;
                }

            }
        }

        public void onRecvSEIMsg(String userId, Byte[] message, UInt32 msgSize)
        {
            //string seiMessage = System.Text.Encoding.UTF8.GetString(message, 0, (int)msgSize);
            
            //Debug.Log(String.Format("onRecvSEIMsg {0}, {1}, {2}", userId, seiMessage, msgSize));
            Debug.Log("onRecvSEIMsg: " + userId + ", " + msgSize);
            //Debug.Log(String.Format("onRecvSEIMsg {0}, {1}, {2}", userId, msgSize, 59));

            //string strInfo = "";
            //for (int i = 0; i < msgSize; i++)
            //{
            //    strInfo += message[i].ToString() + ", ";
            //}
            //Debug.Log("strInfo: " + strInfo);

            if (msgSize == 111 || msgSize == 59)
            {
                m_BlendShapesDataContainer.byteReceivedFacialData = message.ToArray();
            }


            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            if (timestamp == 0)
            {
                timestamp = unixTimeMilliseconds;
            }
            else
            {
                var delta_time = unixTimeMilliseconds - timestamp;
                timestamp = unixTimeMilliseconds;

                during_time += delta_time;
                if (during_time >= 1000)
                {
                    Debug.Log("during_time: " + during_time + ", during_count: " + during_count);
                    during_time = 0;
                    during_count = 0;
                }
                else
                {
                    during_count += 1;
                }
            }
        }

        public void onStartPublishing(int err, string errMsg)
        {
            LogManager.Log(String.Format("onStartPublishing {0}, {1}", err, errMsg));
        }

        public void onStopPublishing(int err, string errMsg)
        {
            LogManager.Log(String.Format("onStopPublishing {0}, {1}", err, errMsg));
        }

        public void onScreenCaptureStarted()
        {
            LogManager.Log(String.Format("onScreenCaptureStarted"));
        }

        public void onScreenCapturePaused(int reason)
        {
            LogManager.Log(String.Format("onScreenCapturePaused {0}", reason));
        }

        public void onScreenCaptureResumed(int reason)
        {
            LogManager.Log(String.Format("onScreenCaptureResumed {0}", reason));
        }

        public void onScreenCaptureStoped(int reason)
        {
            LogManager.Log(String.Format("onScreenCaptureStoped {0}", reason));
        }

        public void onStartPublishCDNStream(int err, string errMsg)
        {
            LogManager.Log(String.Format("onStartPublishCDNStream {0}, {1}", err, errMsg));
        }

        public void onStopPublishCDNStream(int err, string errMsg)
        {
            LogManager.Log(String.Format("onStopPublishCDNStream {0}, {1}", err, errMsg));
        }

        public void onConnectOtherRoom(string userId, TXLiteAVError errCode, string errMsg)
        {
            LogManager.Log(String.Format("onConnectOtherRoom {0}, {1}, {2}", userId, errCode , errMsg));
        }

        public void onDisconnectOtherRoom(TXLiteAVError errCode, string errMsg)
        {
            LogManager.Log(String.Format("onDisconnectOtherRoom {0}, {1}",  errCode , errMsg));
        }

        public void onSwitchRoom(TXLiteAVError errCode, string errMsg)
        {
            LogManager.Log(String.Format("onSwitchRoom {0}, {1}",  errCode , errMsg));
        }

        public void onSpeedTest(TRTCSpeedTestResult currentResult, int finishedCount, int totalCount)
        {
            LogManager.Log(String.Format("onSpeedTest {0}, {1} ,{2}",  currentResult.upLostRate , finishedCount , totalCount));
        }

        public void onTestMicVolume(int volume)
        {
            LogManager.Log(String.Format("onTestMicVolume {0}",  volume ));
        }

        public void onTestSpeakerVolume(int volume)
        {
            LogManager.Log(String.Format("onTestSpeakerVolume {0}",  volume ));
        }

        public void onAudioDeviceCaptureVolumeChanged(int volume, bool muted)
        {
            LogManager.Log(String.Format("onAudioDeviceCaptureVolumeChanged {0} , {1}",  volume ,muted));
        }

        public void onAudioDevicePlayoutVolumeChanged(int volume, bool muted)
        {
            LogManager.Log(String.Format("onAudioDevicePlayoutVolumeChanged {0} , {1}",  volume ,muted));
        }
       
        public void onMissCustomCmdMsg(string userId, int cmdID, int errCode, int missed)
        {
            LogManager.Log(String.Format("onMissCustomCmdMsg {0}, {1}", userId, cmdID));
        }
        public void onSnapshotComplete(string userId, TRTCVideoStreamType type, byte[] data, int length, int width, int height, TRTCVideoPixelFormat format)
        {
            LogManager.Log(String.Format("onSnapshotComplete {0} , {1}", userId, type));
        }

        public void onSetMixTranscodingConfig(int errCode, String errMsg)
        {
            LogManager.Log(String.Format("onSetMixTranscodingConfig {0} , {1}", errCode, errMsg));
        }
#endregion
    }
}