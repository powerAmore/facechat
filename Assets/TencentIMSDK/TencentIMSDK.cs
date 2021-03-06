
using com.tencent.imsdk.unity.native;
using com.tencent.imsdk.unity.utils;
using com.tencent.imsdk.unity.callback;
using com.tencent.imsdk.unity.enums;
using com.tencent.imsdk.unity.types;
using Newtonsoft.Json;
using UnityEngine;
using AOT;
using System.Text;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace com.tencent.imsdk.unity
{
    public class TencentIMSDK
    {
        private static SynchronizationContext mainSyncContext = SynchronizationContext.Current;



        private static Dictionary<string, ValueCallback> ValuecallbackStore = new Dictionary<string, ValueCallback>();

        private static RecvNewMsgCallback RecvNewMsgCallbackStore;

        private static MsgReadedReceiptCallback MsgReadedReceiptCallbackStore;
        private static MsgRevokeCallback MsgRevokeCallbackStore;
        private static MsgElemUploadProgressCallback MsgElemUploadProgressCallbackStore;
        private static GroupTipsEventCallback GroupTipsEventCallbackStore;

        private static GroupAttributeChangedCallback GroupAttributeChangedCallbackStore;

        private static ConvEventCallback ConvEventCallbackStore;

        private static ConvTotalUnreadMessageCountChangedCallback ConvTotalUnreadMessageCountChangedCallbackStore;
        private static NetworkStatusListenerCallback NetworkStatusListenerCallbackStore;
        private static KickedOfflineCallback KickedOfflineCallbackStore;
        private static UserSigExpiredCallback UserSigExpiredCallbackStore;
        private static OnAddFriendCallback OnAddFriendCallbackStore;
        private static OnDeleteFriendCallback OnDeleteFriendCallbackStore;
        private static UpdateFriendProfileCallback UpdateFriendProfileCallbackStore;
        private static FriendAddRequestCallback FriendAddRequestCallbackStore;
        private static FriendApplicationListDeletedCallback FriendApplicationListDeletedCallbackStore;
        private static FriendApplicationListReadCallback FriendApplicationListReadCallbackStore;
        private static FriendBlackListAddedCallback FriendBlackListAddedCallbackStore;
        private static FriendBlackListDeletedCallback FriendBlackListDeletedCallbackStore;
        private static LogCallback LogCallbackStore;
        private static MsgUpdateCallback MsgUpdateCallbackStore;



        [MonoPInvokeCallback(typeof(ValueCallback))]
        public static void CallExperimentalAPICallback(int code, string desc, string json_param, string user_data) { }

        /// <summary>
        /// ?????????IM SDK
        /// </summary>
        /// <param name="sdk_app_id">sdk_app_id??????????????????????????? IM??????????????????????????????</param>
        /// <param name="json_sdk_config"><see cref="SdkConfig"/></param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult Init(long sdk_app_id, SdkConfig json_sdk_config)
        {
            ExperimentalAPIReqeustParam param = new ExperimentalAPIReqeustParam();

            param.request_internal_operation = TIMInternalOperation.internal_operation_set_ui_platform.ToString();

            param.request_set_ui_platform_param = "unity";

            TIMResult res = CallExperimentalAPI(param, CallExperimentalAPICallback);

            string configString = JsonConvert.SerializeObject(json_sdk_config);

            Utils.Log(configString);

            int timSucc = IMNativeSDK.TIMInit(sdk_app_id, Utils.string2intptr(configString));

            return (TIMResult)timSucc;
        }
        /// <summary>
        /// ????????????IM SDK
        /// </summary>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult Uninit()
        {
            RemoveRecvNewMsgCallback();

            SetConvEventCallback(null);

            SetConvTotalUnreadMessageCountChangedCallback(null);

            SetFriendAddRequestCallback(null);

            SetFriendApplicationListDeletedCallback(null);

            SetFriendApplicationListReadCallback(null);

            SetFriendBlackListAddedCallback(null);

            SetFriendBlackListDeletedCallback(null);

            SetGroupAttributeChangedCallback(null);

            SetGroupTipsEventCallback(null);

            SetKickedOfflineCallback(null);

            SetLogCallback(null);

            SetMsgElemUploadProgressCallback(null);

            SetMsgReadedReceiptCallback(null);

            SetMsgRevokeCallback(null);

            SetMsgUpdateCallback(null);

            SetNetworkStatusListenerCallback(null);

            SetOnAddFriendCallback(null);

            SetOnDeleteFriendCallback(null);

            SetUpdateFriendProfileCallback(null);


            SetUserSigExpiredCallback(null);


            int timSucc = IMNativeSDK.TIMUninit();

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????
        /// </summary>
        /// <param name="user_id">??????ID</param>
        /// <param name="user_sig">??????sdk_app_id???secret?????????????????? https://cloud.tencent.com/document/product/269/32688</param>
        /// <param name="callback">?????? <see cref="ValueCallback"/> </param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult Login(string user_id, string user_sig, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMLogin(Utils.string2intptr(user_id), Utils.string2intptr(user_sig), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????IM SDK???????????????
        /// </summary>
        /// <returns>string version</returns>
        public static string GetSDKVersion()
        {


            IntPtr version = IMNativeSDK.TIMGetSDKVersion();

            return Utils.intptr2string(version);
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="config">?????? SetConfig</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult SetConfig(SetConfig config, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMSetConfig(Utils.string2intptr(JsonConvert.SerializeObject(config)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        /// <returns>???????????????</returns>
        public static long GetServerTime()
        {
            return IMNativeSDK.TIMGetServerTime();
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult Logout(ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMLogout(ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <returns>TIMLoginStatus</returns>
        public static TIMLoginStatus GetLoginStatus()
        {

            int timSucc = IMNativeSDK.TIMGetLoginStatus();

            return (TIMLoginStatus)timSucc;
        }

        /// <summary>
        /// ????????????????????????ID
        /// </summary>
        /// <param name="user_id">???????????????user_id???StringBuilder</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GetLoginUserID(StringBuilder user_id)
        {
            int timSucc = IMNativeSDK.TIMGetLoginUserID(user_id);
            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ConvGetConvList(ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMConvGetConvList(ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID???c2c?????????user_id???????????????group_id</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ConvDelete(string conv_id, TIMConvType conv_type, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMConvDelete(Utils.string2intptr(conv_id), (int)conv_type, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="conv_list_param">???????????????????????? ConvParam??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ConvGetConvInfo(List<ConvParam> conv_list_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMConvGetConvInfo(Utils.string2intptr(JsonConvert.SerializeObject(conv_list_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="param">DraftParam</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ConvSetDraft(string conv_id, TIMConvType conv_type, DraftParam param)
        {




            int timSucc = IMNativeSDK.TIMConvSetDraft(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(param)));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ConvCancelDraft(string conv_id, TIMConvType conv_type)
        {

            int timSucc = IMNativeSDK.TIMConvCancelDraft(conv_id, (int)conv_type);

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="is_pinned">??????????????????</param> 
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ConvPinConversation(string conv_id, TIMConvType conv_type, bool is_pinned, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMConvPinConversation(conv_id, (int)conv_type, is_pinned, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ConvGetTotalUnreadMessageCount(ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMConvGetTotalUnreadMessageCount(ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message">????????? Message</param>
        /// <param name="message_id">????????????ID???StringBuilder</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgSendMessage(string conv_id, TIMConvType conv_type, Message message, StringBuilder message_id, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgSendMessage(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message)), message_id, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message_id">??????ID</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgCancelSend(string conv_id, TIMConvType conv_type, string message_id, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgCancelSend(conv_id, (int)conv_type, Utils.string2intptr(message_id), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="message_id_array">???????????????id??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgFindMessages(List<string> message_id_array, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgFindMessages(Utils.string2intptr(JsonConvert.SerializeObject(message_id_array)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message">????????? Message</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgReportReaded(string conv_id, TIMConvType conv_type, Message message, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgReportReaded(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgMarkAllMessageAsRead(ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgMarkAllMessageAsRead(ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message">????????? Message</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns> 
        public static TIMResult MsgRevoke(string conv_id, TIMConvType conv_type, Message message, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgRevoke(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ?????????????????????????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message_locator">??????????????? MsgLocator</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgFindByMsgLocatorList(string conv_id, TIMConvType conv_type, MsgLocator message_locator, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgFindByMsgLocatorList(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message_locator)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message_list">???????????? Message??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgImportMsgList(string conv_id, TIMConvType conv_type, List<Message> message_list, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgImportMsgList(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message_list)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message">?????????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgSaveMsg(string conv_id, TIMConvType conv_type, Message message, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgSaveMsg(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="get_message_list_param">???????????????????????? MsgGetMsgListParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgGetMsgList(string conv_id, TIMConvType conv_type, MsgGetMsgListParam get_message_list_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgGetMsgList(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(get_message_list_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message_delete_param">?????????????????? MsgDeleteParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgDelete(string conv_id, TIMConvType conv_type, MsgDeleteParam message_delete_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgDelete(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message_delete_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="message_delete_param">?????????????????? MsgDeleteParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgListDelete(string conv_id, TIMConvType conv_type, List<Message> message_list, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();


            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgListDelete(conv_id, (int)conv_type, Utils.string2intptr(JsonConvert.SerializeObject(message_list)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="conv_id">??????ID</param>
        /// <param name="conv_type">???????????? TIMConvType</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgClearHistoryMessage(string conv_id, TIMConvType conv_type, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgClearHistoryMessage(conv_id, (int)conv_type, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="user_id_list">??????ID??????</param>
        /// <param name="opt">?????????????????? TIMReceiveMessageOpt</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgSetC2CReceiveMessageOpt(List<string> user_id_list, TIMReceiveMessageOpt opt, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgSetC2CReceiveMessageOpt(Utils.string2intptr(JsonConvert.SerializeObject(user_id_list)), (int)opt, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????C2C???????????????
        /// </summary>
        /// <param name="user_id_list">??????ID??????</param>
        /// <param name="opt">?????????????????? TIMReceiveMessageOpt</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgGetC2CReceiveMessageOpt(List<string> user_id_list, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgGetC2CReceiveMessageOpt(Utils.string2intptr(JsonConvert.SerializeObject(user_id_list)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="group_id">??????ID??????</param>
        /// <param name="opt">?????????????????? TIMReceiveMessageOpt</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgSetGroupReceiveMessageOpt(string group_id, TIMReceiveMessageOpt opt, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgSetGroupReceiveMessageOpt(Utils.string2intptr(group_id), (int)opt, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="download_param">???????????? DownloadElemParam</param>
        /// <param name="path">????????????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgDownloadElemToPath(DownloadElemParam download_param, string path, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgDownloadElemToPath(Utils.string2intptr(JsonConvert.SerializeObject(download_param)), Utils.string2intptr(path), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="message">????????? Message</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgDownloadMergerMessage(Message message, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgDownloadMergerMessage(Utils.string2intptr(JsonConvert.SerializeObject(message)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="json_batch_send_param">??????????????? MsgBatchSendParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgBatchSend(MsgBatchSendParam json_batch_send_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgBatchSend(Utils.string2intptr(JsonConvert.SerializeObject(json_batch_send_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="message_search_param">?????????????????? MessageSearchParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgSearchLocalMessages(MessageSearchParam message_search_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgSearchLocalMessages(Utils.string2intptr(JsonConvert.SerializeObject(message_search_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="message">????????? Message</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult MsgSetLocalCustomData(Message message, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMMsgSetLocalCustomData(Utils.string2intptr(JsonConvert.SerializeObject(message)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ?????????
        /// </summary>
        /// <param name="group">??????????????? CreateGroupParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupCreate(CreateGroupParam group, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMGroupCreate(Utils.string2intptr(JsonConvert.SerializeObject(group)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ?????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupDelete(string group_id, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int timSucc = IMNativeSDK.TIMGroupDelete(Utils.string2intptr(group_id), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)timSucc;
        }

        /// <summary>
        /// ?????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="hello_message">?????????????????????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupJoin(string group_id, string hello_message, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupJoin(Utils.string2intptr(group_id), Utils.string2intptr(hello_message), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupQuit(string group_id, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupQuit(Utils.string2intptr(group_id), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="param">?????????????????? GroupInviteMemberParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupInviteMember(GroupInviteMemberParam param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupInviteMember(Utils.string2intptr(JsonConvert.SerializeObject(param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ???????????????
        /// </summary>
        /// <param name="param">?????????????????? GroupDeleteMemberParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupDeleteMember(GroupDeleteMemberParam param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupDeleteMember(Utils.string2intptr(JsonConvert.SerializeObject(param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupGetJoinedGroupList(ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupGetJoinedGroupList(ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ???????????????
        /// </summary>
        /// <param name="group_id_list">???ID??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupGetGroupInfoList(List<string> group_id_list, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupGetGroupInfoList(Utils.string2intptr(JsonConvert.SerializeObject(group_id_list)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ???????????????
        /// </summary>
        /// <param name="json_group_modifyinfo_param">?????????????????? GroupModifyInfoParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupModifyGroupInfo(GroupModifyInfoParam json_group_modifyinfo_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupModifyGroupInfo(Utils.string2intptr(JsonConvert.SerializeObject(json_group_modifyinfo_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="json_group_getmeminfos_param">?????????????????? GroupGetMemberInfoListParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupGetMemberInfoList(GroupGetMemberInfoListParam json_group_getmeminfos_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupGetMemberInfoList(Utils.string2intptr(JsonConvert.SerializeObject(json_group_getmeminfos_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="json_group_modifymeminfo_param">?????????????????? GroupModifyMemberInfoParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupModifyMemberInfo(GroupModifyMemberInfoParam json_group_modifymeminfo_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupModifyMemberInfo(Utils.string2intptr(JsonConvert.SerializeObject(json_group_modifymeminfo_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="json_group_getpendence_list_param">?????????????????? GroupPendencyOption</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupGetPendencyList(GroupPendencyOption json_group_getpendence_list_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupGetPendencyList(Utils.string2intptr(JsonConvert.SerializeObject(json_group_getpendence_list_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="time_stamp">?????????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupReportPendencyReaded(long time_stamp, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupReportPendencyReaded(time_stamp, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="json_group_handle_pendency_param">??????????????????????????? GroupHandlePendencyParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupHandlePendency(GroupHandlePendencyParam json_group_handle_pendency_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupHandlePendency(Utils.string2intptr(JsonConvert.SerializeObject(json_group_handle_pendency_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupGetOnlineMemberCount(string group_id, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupGetOnlineMemberCount(Utils.string2intptr(group_id), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupSearchGroups(GroupSearchParam json_group_search_groups_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupSearchGroups(Utils.string2intptr(JsonConvert.SerializeObject(json_group_search_groups_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ???????????????
        /// </summary>
        /// <param name="json_group_search_group_members_param">????????????????????? GroupMemberSearchParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupSearchGroupMembers(GroupMemberSearchParam json_group_search_group_members_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupSearchGroupMembers(Utils.string2intptr(JsonConvert.SerializeObject(json_group_search_group_members_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="json_group_atrributes">??????????????? GroupAttributes</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupInitGroupAttributes(string group_id, GroupAttributes json_group_atrributes, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupInitGroupAttributes(Utils.string2intptr(group_id), Utils.string2intptr(JsonConvert.SerializeObject(json_group_atrributes)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="json_keys">??????key??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupDeleteGroupAttributes(string group_id, List<string> json_keys, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupDeleteGroupAttributes(Utils.string2intptr(group_id), Utils.string2intptr(JsonConvert.SerializeObject(json_keys)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="group_id">???ID</param>
        /// <param name="json_keys">??????key??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult GroupGetGroupAttributes(string group_id, List<string> json_keys, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMGroupGetGroupAttributes(Utils.string2intptr(group_id), Utils.string2intptr(JsonConvert.SerializeObject(json_keys)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="json_get_user_profile_list_param">???????????????????????? FriendShipGetProfileListParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ProfileGetUserProfileList(FriendShipGetProfileListParam json_get_user_profile_list_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMProfileGetUserProfileList(Utils.string2intptr(JsonConvert.SerializeObject(json_get_user_profile_list_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="json_modify_self_user_profile_param">???????????????????????? UserProfileItem</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult ProfileModifySelfUserProfile(UserProfileItem json_modify_self_user_profile_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMProfileModifySelfUserProfile(Utils.string2intptr(JsonConvert.SerializeObject(json_modify_self_user_profile_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipGetFriendProfileList(ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipGetFriendProfileList(ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="param">?????????????????? FriendshipAddFriendParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipAddFriend(FriendshipAddFriendParam param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipAddFriend(Utils.string2intptr(JsonConvert.SerializeObject(param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="param">???????????????????????? FriendRespone</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipHandleFriendAddRequest(FriendRespone json_handle_friend_add_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipHandleFriendAddRequest(Utils.string2intptr(JsonConvert.SerializeObject(json_handle_friend_add_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="json_modify_friend_info_param">???????????????????????? FriendshipModifyFriendProfileParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipModifyFriendProfile(FriendshipModifyFriendProfileParam json_modify_friend_info_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            Utils.Log(JsonConvert.SerializeObject(json_modify_friend_info_param));

            int res = IMNativeSDK.TIMFriendshipModifyFriendProfile(Utils.string2intptr(JsonConvert.SerializeObject(json_modify_friend_info_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="json_delete_friend_param">?????????????????? FriendshipDeleteFriendParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipDeleteFriend(FriendshipDeleteFriendParam json_delete_friend_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipDeleteFriend(Utils.string2intptr(JsonConvert.SerializeObject(json_delete_friend_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="json_check_friend_list_param">???????????????????????? FriendshipCheckFriendTypeParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipCheckFriendType(FriendshipCheckFriendTypeParam json_check_friend_list_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipCheckFriendType(Utils.string2intptr(JsonConvert.SerializeObject(json_check_friend_list_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="json_create_friend_group_param">???????????????????????? FriendGroupInfo</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipCreateFriendGroup(FriendGroupInfo json_create_friend_group_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipCreateFriendGroup(Utils.string2intptr(JsonConvert.SerializeObject(json_create_friend_group_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="json_get_friend_group_list_param">?????????????????????userID??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipGetFriendGroupList(List<string> json_get_friend_group_list_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipGetFriendGroupList(Utils.string2intptr(JsonConvert.SerializeObject(json_get_friend_group_list_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="json_modify_friend_group_param">?????????????????? FriendshipModifyFriendGroupParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipModifyFriendGroup(FriendshipModifyFriendGroupParam json_modify_friend_group_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipModifyFriendGroup(Utils.string2intptr(JsonConvert.SerializeObject(json_modify_friend_group_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="json_delete_friend_group_param">?????????????????? ???userID??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipDeleteFriendGroup(List<string> json_delete_friend_group_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipDeleteFriendGroup(Utils.string2intptr(JsonConvert.SerializeObject(json_delete_friend_group_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="json_delete_friend_group_param">?????????????????? ???userID??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipAddToBlackList(List<string> json_add_to_blacklist_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipAddToBlackList(Utils.string2intptr(JsonConvert.SerializeObject(json_add_to_blacklist_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipGetBlackList(ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipGetBlackList(ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        /// <param name="json_delete_from_blacklist_param">userID??????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipDeleteFromBlackList(List<string> json_delete_from_blacklist_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipDeleteFromBlackList(Utils.string2intptr(JsonConvert.SerializeObject(json_delete_from_blacklist_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="json_get_pendency_list_param">???????????????????????? FriendshipGetPendencyListParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipGetPendencyList(FriendshipGetPendencyListParam json_get_pendency_list_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipGetPendencyList(Utils.string2intptr(JsonConvert.SerializeObject(json_get_pendency_list_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="json_delete_pendency_param">?????????????????????????????? FriendshipDeletePendencyParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipDeletePendency(FriendshipDeletePendencyParam json_delete_pendency_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipDeletePendency(Utils.string2intptr(JsonConvert.SerializeObject(json_delete_pendency_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        /// <param name="time_stamp">???????????????</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipReportPendencyReaded(long time_stamp, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipReportPendencyReaded(time_stamp, ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="json_search_friends_param">???????????? FriendSearchParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipSearchFriends(FriendSearchParam json_search_friends_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipSearchFriends(Utils.string2intptr(JsonConvert.SerializeObject(json_search_friends_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="json_get_friends_info_param">???????????????????????????userIDs</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult FriendshipGetFriendsInfo(List<string> json_get_friends_info_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.TIMFriendshipGetFriendsInfo(Utils.string2intptr(JsonConvert.SerializeObject(json_get_friends_info_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }

        /// <summary>
        /// ?????????????????????????????????????????????????????????????????????
        /// </summary>
        /// <param name="json_param">????????????????????? ExperimentalAPIReqeustParam</param>
        /// <param name="callback">?????? ValueCallback</param>
        /// <returns><see cref="TIMResult"/></returns>
        public static TIMResult CallExperimentalAPI(ExperimentalAPIReqeustParam json_param, ValueCallback callback)
        {
            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();

            ValuecallbackStore.Add(user_data, callback);

            int res = IMNativeSDK.callExperimentalAPI(Utils.string2intptr(JsonConvert.SerializeObject(json_param)), ValueCallbackInstance, Utils.string2intptr(user_data));

            return (TIMResult)res;
        }





        /// <summary>
        /// ???????????????????????????
        /// <para>??????????????????????????????ImSDK???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????? </para>
        /// <para>???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// <para>????????????????????????ImSDK???????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? RecvNewMsgCallback</param>
        public static void AddRecvNewMsgCallback(RecvNewMsgCallback callback)
        {

            string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string user_data = fn_name + "_" + Utils.getRandomStr();


            RecvNewMsgCallbackStore = callback;


            IMNativeSDK.TIMAddRecvNewMsgCallback(TIMRecvNewMsgCallbackInstance, Utils.string2intptr(user_data));


        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        public static void RemoveRecvNewMsgCallback()
        {

            IMNativeSDK.TIMRemoveRecvNewMsgCallback(TIMRecvNewMsgCallbackInstance);
        }


        /// <summary>
        /// ??????????????????????????????
        /// <para>?????????????????????????????????????????????[TIMMsgReportReaded]()?????????????????????????????????ImSDK??????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? MsgReadedReceiptCallback</param>
        public static void SetMsgReadedReceiptCallback(MsgReadedReceiptCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                MsgReadedReceiptCallbackStore = callback;

                IMNativeSDK.TIMSetMsgReadedReceiptCallback(TIMMsgReadedReceiptCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetMsgReadedReceiptCallback(null, Utils.string2intptr(""));
            }

        }

        /// <summary>
        /// ????????????????????????????????????
        /// <para>???????????????????????????????????????????????????????????????????????????[TIMMsgRevoke]()??????????????????????????????ImSDK??????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? MsgRevokeCallback</param>
        public static void SetMsgRevokeCallback(MsgRevokeCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                MsgRevokeCallbackStore = callback;

                IMNativeSDK.TIMSetMsgRevokeCallback(TIMMsgRevokeCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetMsgRevokeCallback(null, Utils.string2intptr(""));
            }



        }


        /// <summary>
        /// ???????????????????????????????????????????????????
        /// <para>??????????????????????????????????????????????????????????????????????????????????????????????????????ImSDK????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? MsgElemUploadProgressCallback</param>
        public static void SetMsgElemUploadProgressCallback(MsgElemUploadProgressCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                MsgElemUploadProgressCallbackStore = callback;

                IMNativeSDK.TIMSetMsgElemUploadProgressCallback(TIMMsgElemUploadProgressCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetMsgElemUploadProgressCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ??????????????????????????????
        /// <para>?????????????????????????????? ??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? GroupTipsEventCallback</param>
        public static void SetGroupTipsEventCallback(GroupTipsEventCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                GroupTipsEventCallbackStore = callback;

                IMNativeSDK.TIMSetGroupTipsEventCallback(TIMGroupTipsEventCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetGroupTipsEventCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ??????????????????????????????
        /// <para>???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? GroupAttributeChangedCallback</param>
        public static void SetGroupAttributeChangedCallback(GroupAttributeChangedCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                GroupAttributeChangedCallbackStore = callback;

                IMNativeSDK.TIMSetGroupAttributeChangedCallback(TIMGroupAttributeChangedCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetGroupAttributeChangedCallback(null, Utils.string2intptr(""));
            }


        }


        /// <summary>
        /// ????????????????????????
        /// <para>?????????????????????</para>
        /// <para>????????????</para>
        /// <para>????????????</para>
        /// <para>????????????</para>
        /// <para>????????????</para>
        /// <para>????????????</para>
        /// <para>???????????????????????????????????????????????????????????????????????????????????????[TIMConvCreate]()????????????????????????????????????????????????????????????</para>
        /// <para>?????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// <para>????????????[TIMConvDelete]()???????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? ConvEventCallback</param>
        public static void SetConvEventCallback(ConvEventCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                ConvEventCallbackStore = callback;

                IMNativeSDK.TIMSetConvEventCallback(TIMConvEventCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetConvEventCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ?????????????????????????????????????????????
        /// </summary>
        /// <param name="callback">?????? ConvTotalUnreadMessageCountChangedCallback</param>
        public static void SetConvTotalUnreadMessageCountChangedCallback(ConvTotalUnreadMessageCountChangedCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                ConvTotalUnreadMessageCountChangedCallbackStore = callback;

                IMNativeSDK.TIMSetConvTotalUnreadMessageCountChangedCallback(TIMConvTotalUnreadMessageCountChangedCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetConvTotalUnreadMessageCountChangedCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ????????????????????????????????????
        /// <para>??????????????? Init() ??????ImSDK????????????????????????????????????????????????????????????????????????????????????</para>
        /// <para>????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????ImSDK?????????????????????IM???Server???????????????</para>
        /// <para>?????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// <para>?????????????????????????????????ImSDK???????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? NetworkStatusListenerCallback</param>
        public static void SetNetworkStatusListenerCallback(NetworkStatusListenerCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                NetworkStatusListenerCallbackStore = callback;

                IMNativeSDK.TIMSetNetworkStatusListenerCallback(TIMNetworkStatusListenerCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetNetworkStatusListenerCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ??????????????????????????????
        ///  <para>???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        ///  <para>???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????ERR_IMSDK_KICKED_BY_OTHERS???6208????????????????????????????????????????????????????????????????????????</para>
        ///  <para>???????????????????????????????????????</para>
        ///  <para>???????????????1??????????????????????????????????????????????????????2?????????????????????????????????1???????????????????????? KickedOfflineCallback ?????????</para>
        ///  <para>???????????????1???????????????????????????????????????????????????login?????????????????????2????????????????????????????????????????????????</para>
        ///  <para>????????????????????????:</para>
        ///  <para>???????????????1?????????????????????logout??????????????????????????????????????????2???????????????????????????????????????????????????????????????</para>
        ///  <para>?????????????????????????????????????????????????????????????????????1??????????????????????????????ERR_IMSDK_KICKED_BY_OTHERS???6208?????????????????????????????????????????????????????????????????????</para>
        ///  <para>??????????????????????????????login?????????????????????2?????????????????????????????? KickedOfflineCallback ?????????</para>
        /// </summary>
        /// <param name="callback">?????? KickedOfflineCallback</param>
        public static void SetKickedOfflineCallback(KickedOfflineCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                KickedOfflineCallbackStore = callback;

                IMNativeSDK.TIMSetKickedOfflineCallback(TIMKickedOfflineCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetKickedOfflineCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ????????????????????????
        /// <para>???????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// <para>Login()???????????????70001?????????????????????????????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? UserSigExpiredCallback</param>
        public static void SetUserSigExpiredCallback(UserSigExpiredCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                UserSigExpiredCallbackStore = callback;

                IMNativeSDK.TIMSetUserSigExpiredCallback(TIMUserSigExpiredCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetUserSigExpiredCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ???????????????????????????
        /// <para>???????????????????????????????????????A?????????B?????????????????????????????????ImSDK???A????????????????????????B??????ImSDK?????????????????????????????????ImSDK?????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? OnAddFriendCallback</param>
        public static void SetOnAddFriendCallback(OnAddFriendCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                OnAddFriendCallbackStore = callback;


            }
            else
            {
                IMNativeSDK.TIMSetOnAddFriendCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ???????????????????????????
        /// <para>???????????????????????????????????????A?????????B?????????????????????????????????ImSDK???A????????????????????????B??????ImSDK?????????????????????????????????ImSDK?????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? OnDeleteFriendCallback</param>
        public static void SetOnDeleteFriendCallback(OnDeleteFriendCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                OnDeleteFriendCallbackStore = callback;


            }
            else
            {
                IMNativeSDK.TIMSetOnDeleteFriendCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ?????????????????????????????????
        /// <para>???????????????????????????????????????A?????????B?????????????????????????????????ImSDK???A??????????????????????????????B??????ImSDK???????????????????????????????????????ImSDK?????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? UpdateFriendProfileCallback</param>
        public static void SetUpdateFriendProfileCallback(UpdateFriendProfileCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                UpdateFriendProfileCallbackStore = callback;


            }
            else
            {
                IMNativeSDK.TIMSetUpdateFriendProfileCallback(null, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ?????????????????????????????????
        /// <para>???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????ImSDK????????????????????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? FriendAddRequestCallback</param>
        public static void SetFriendAddRequestCallback(FriendAddRequestCallback callback)
        {
            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                FriendAddRequestCallbackStore = callback;

                IMNativeSDK.TIMSetFriendAddRequestCallback(TIMFriendAddRequestCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetFriendAddRequestCallback(null, Utils.string2intptr(""));
            }

        }

        /// <summary>
        /// ????????????????????????????????????
        /// <para>1. ????????????????????????</para>
        /// <para>2. ??????????????????</para>
        /// <para>3. ??????????????????</para>
        /// <para>4. ??????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? FriendApplicationListDeletedCallback</param>
        public static void SetFriendApplicationListDeletedCallback(FriendApplicationListDeletedCallback callback)
        {

            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                FriendApplicationListDeletedCallbackStore = callback;

                IMNativeSDK.TIMSetFriendApplicationListDeletedCallback(TIMFriendApplicationListDeletedCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetFriendApplicationListDeletedCallback(null, Utils.string2intptr(""));
            }

        }

        /// <summary>
        /// ?????????????????????????????????
        /// <para>???????????? setFriendApplicationRead ????????????????????????????????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? FriendApplicationListReadCallback</param>
        public static void SetFriendApplicationListReadCallback(FriendApplicationListReadCallback callback)
        {

            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                FriendApplicationListReadCallbackStore = callback;

                IMNativeSDK.TIMSetFriendApplicationListReadCallback(TIMFriendApplicationListReadCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetFriendApplicationListReadCallback(null, Utils.string2intptr(""));
            }

        }

        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        /// <param name="callback">?????? FriendBlackListAddedCallback</param>
        public static void SetFriendBlackListAddedCallback(FriendBlackListAddedCallback callback)
        {



            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                FriendBlackListAddedCallbackStore = callback;
                IMNativeSDK.TIMSetFriendBlackListAddedCallback(TIMFriendBlackListAddedCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetFriendBlackListAddedCallback(TIMFriendBlackListAddedCallbackInstance, Utils.string2intptr(""));
            }


        }

        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        /// <param name="callback">?????? FriendBlackListDeletedCallback</param>
        public static void SetFriendBlackListDeletedCallback(FriendBlackListDeletedCallback callback)
        {



            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                FriendBlackListDeletedCallbackStore = callback;
                IMNativeSDK.TIMSetFriendBlackListDeletedCallback(TIMFriendBlackListDeletedCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetFriendBlackListDeletedCallback(null, Utils.string2intptr(""));
            }



        }

        /// <summary>
        /// ??????????????????
        /// <para>????????????????????????????????????ImSDK??????????????????????????????????????????????????????</para>
        /// <para>???????????????????????????SetConfig()?????????????????????????????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? FriendBlackListDeletedCallback</param>
        public static void SetLogCallback(LogCallback callback)
        {

            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                LogCallbackStore = callback;

                IMNativeSDK.TIMSetLogCallback(TIMLogCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetLogCallback(null, Utils.string2intptr(""));
            }

        }

        /// <summary>
        /// ????????????????????????????????????????????????????????????????????????
        /// <para> ????????????????????????????????????????????????ImSDK?????????????????????????????? </para>
        /// <para> ????????????????????????????????????????????????????????????IM?????? [???????????????????????????](https://cloud.tencent.com/document/product/269/1632)</para>
        /// <para> ?????????????????????????????????IM?????????????????????????????????????????????????????????????????????????????????????????????</para>
        /// <para> ????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????ImSDK?????????????????????????????????</para>
        /// </summary>
        /// <param name="callback">?????? FriendBlackListDeletedCallback</param>
        public static void SetMsgUpdateCallback(MsgUpdateCallback callback)
        {

            if (callback != null)
            {
                string fn_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

                string user_data = fn_name + "_" + Utils.getRandomStr();

                MsgUpdateCallbackStore = callback;

                IMNativeSDK.TIMSetMsgUpdateCallback(TIMMsgUpdateCallbackInstance, Utils.string2intptr(user_data));
            }
            else
            {
                IMNativeSDK.TIMSetMsgUpdateCallback(null, Utils.string2intptr(""));
            }

        }


        
        [MonoPInvokeCallback(typeof(IMNativeSDK.CommonValueCallback))]
        private static void ValueCallbackInstance(int code, IntPtr desc, IntPtr json_param, IntPtr user_data)
        {

            string user_data_string = Utils.intptr2string(user_data);
            string desc_string = Utils.intptr2string(desc);
            string json_param_string = Utils.intptr2string(json_param);

            mainSyncContext.Send(threadOperation, new CallbackConvert(code, "ValueCallback", json_param_string, user_data_string, desc_string));
        }
        static private void threadOperation(object obj)
        {
            CallbackConvert data = (CallbackConvert)obj;
            switch (data.type)
            {
                case "ValueCallback":
                    if (ValuecallbackStore.ContainsKey(data.user_data))
                    {
                        if (ValuecallbackStore.TryGetValue(data.user_data, out ValueCallback callback))
                        {
                            callback(data.code, data.desc, data.data, data.user_data);

                            ValuecallbackStore.Remove(data.user_data);
                        }

                    };
                    break;
                case "TIMRecvNewMsgCallback":
                    if (RecvNewMsgCallbackStore != null)
                    {
                        RecvNewMsgCallbackStore(JsonConvert.DeserializeObject<List<Message>>(data.data), data.user_data);
                    }
                    break;
                case "TIMMsgReadedReceiptCallback":
                    if (MsgReadedReceiptCallbackStore != null)
                    {
                        MsgReadedReceiptCallbackStore(JsonConvert.DeserializeObject<List<MessageReceipt>>(data.data), data.user_data);

                    }
                    break;
                case "TIMMsgRevokeCallback":
                    if (MsgRevokeCallbackStore != null)
                    {
                        MsgRevokeCallbackStore(JsonConvert.DeserializeObject<List<MsgLocator>>(data.data), data.user_data);

                    }
                    break;
                case "TIMMsgElemUploadProgressCallback":
                    if (MsgElemUploadProgressCallbackStore != null)
                    {
                        MsgElemUploadProgressCallbackStore(JsonConvert.DeserializeObject<Message>(data.data), data.index, data.cur_size, data.total_size, data.user_data);

                    }
                    break;
                case "TIMGroupTipsEventCallback":

                    if (GroupTipsEventCallbackStore != null)
                    {
                        GroupTipsEventCallbackStore(JsonConvert.DeserializeObject<List<GroupTipsElem>>(data.data), data.user_data);

                    }
                    break;
                case "TIMGroupAttributeChangedCallback":

                    if (GroupAttributeChangedCallbackStore != null)
                    {
                        GroupAttributeChangedCallbackStore(data.group_id, JsonConvert.DeserializeObject<List<GroupAttributes>>(data.data), data.user_data);

                    }
                    break;
                case "TIMConvEventCallback":

                    if (ConvEventCallbackStore != null)
                    {
                        ConvEventCallbackStore((TIMConvEvent)data.conv_event, JsonConvert.DeserializeObject<List<ConvInfo>>(data.data), data.user_data);

                    }


                    break;
                case "TIMConvTotalUnreadMessageCountChangedCallback":

                    if (ConvTotalUnreadMessageCountChangedCallbackStore != null)
                    {
                        ConvTotalUnreadMessageCountChangedCallbackStore(data.code, data.user_data);

                    }
                    break;
                case "TIMNetworkStatusListenerCallback":

                    if (NetworkStatusListenerCallbackStore != null)
                    {
                        NetworkStatusListenerCallbackStore((TIMNetworkStatus)data.code, data.index, data.desc, data.user_data);

                    }
                    break;
                case "TIMKickedOfflineCallback":

                    if (KickedOfflineCallbackStore != null)
                    {
                        KickedOfflineCallbackStore(data.user_data);

                    }
                    break;
                case "TIMUserSigExpiredCallback":

                    if (UserSigExpiredCallbackStore != null)
                    {
                        UserSigExpiredCallbackStore(data.user_data);

                    }

                    break;
                case "TIMOnAddFriendCallback":

                    if (OnAddFriendCallbackStore != null)
                    {
                        OnAddFriendCallbackStore(JsonConvert.DeserializeObject<List<string>>(data.data), data.user_data);

                    }
                    break;
                case "TIMOnDeleteFriendCallback":
                    if (OnDeleteFriendCallbackStore != null)
                    {
                        OnDeleteFriendCallbackStore(JsonConvert.DeserializeObject<List<string>>(data.data), data.user_data);

                    }
                    break;
                case "TIMUpdateFriendProfileCallback":

                    if (UpdateFriendProfileCallbackStore != null)
                    {
                        UpdateFriendProfileCallbackStore(JsonConvert.DeserializeObject<List<FriendProfileItem>>(data.data), data.user_data);

                    }
                    break;

                case "TIMFriendAddRequestCallback":

                    if (FriendAddRequestCallbackStore != null)
                    {
                        FriendAddRequestCallbackStore(JsonConvert.DeserializeObject<List<FriendAddPendency>>(data.data), data.user_data);

                    }
                    break;
                case "TIMFriendApplicationListDeletedCallback":

                    if (FriendApplicationListDeletedCallbackStore != null)
                    {
                        FriendApplicationListDeletedCallbackStore(JsonConvert.DeserializeObject<List<string>>(data.data), data.user_data);

                    }
                    break;
                case "TIMFriendApplicationListReadCallback":

                    if (FriendApplicationListReadCallbackStore != null)
                    {
                        FriendApplicationListReadCallbackStore(data.user_data);

                    }
                    break;
                case "TIMFriendBlackListAddedCallback":

                    if (FriendBlackListAddedCallbackStore != null)
                    {
                        FriendBlackListAddedCallbackStore(JsonConvert.DeserializeObject<List<FriendProfile>>(data.data), data.user_data);

                    }
                    break;
                case "TIMFriendBlackListDeletedCallback":

                    if (FriendBlackListDeletedCallbackStore != null)
                    {
                        FriendBlackListDeletedCallbackStore(JsonConvert.DeserializeObject<List<string>>(data.data), data.user_data);

                    }
                    break;
                case "TIMLogCallback":

                    if (LogCallbackStore != null)
                    {
                        LogCallbackStore((TIMLogLevel)data.code, data.data, data.user_data);
                    }
                    break;
                case "TIMMsgUpdateCallback":

                    if (MsgUpdateCallbackStore != null)
                    {
                        MsgUpdateCallbackStore(JsonConvert.DeserializeObject<List<Message>>(data.data), data.user_data);
                    }
                    break;

            }

        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMRecvNewMsgCallback))]
        private static void TIMRecvNewMsgCallbackInstance(IntPtr json_msg_array, IntPtr user_data)
        {

            try
            {
                string json_msg_array_string = Utils.intptr2string(json_msg_array);

                string user_data_string = Utils.intptr2string(user_data);




                mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMRecvNewMsgCallback", json_msg_array_string, user_data_string, ""));
            }
            catch (Exception e)
            {
                Utils.Log("?????????????????????????????????" + e.ToString());
            }

        }


        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMMsgReadedReceiptCallback))]
        private static void TIMMsgReadedReceiptCallbackInstance(IntPtr json_msg_readed_receipt_array, IntPtr user_data)
        {

            string json_msg_readed_receipt_array_string = Utils.intptr2string(json_msg_readed_receipt_array);

            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMMsgReadedReceiptCallback", json_msg_readed_receipt_array_string, user_data_string, ""));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMMsgRevokeCallback))]
        private static void TIMMsgRevokeCallbackInstance(IntPtr json_msg_locator_array, IntPtr user_data)
        {
            string json_msg_locator_array_string = Utils.intptr2string(json_msg_locator_array);

            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMMsgRevokeCallback", json_msg_locator_array_string, user_data_string, ""));


        }


        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMMsgElemUploadProgressCallback))]
        private static void TIMMsgElemUploadProgressCallbackInstance(IntPtr json_msg, int index, int cur_size, int total_size, IntPtr user_data)
        {
            string json_msg_string = Utils.intptr2string(json_msg);

            string user_data_string = Utils.intptr2string(user_data);




            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMMsgElemUploadProgressCallback", json_msg_string, user_data_string, "", index, cur_size, total_size));


        }
        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMGroupTipsEventCallback))]
        private static void TIMGroupTipsEventCallbackInstance(IntPtr json_group_tip_array, IntPtr user_data)
        {
            string json_group_tip_array_string = Utils.intptr2string(json_group_tip_array);

            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMGroupTipsEventCallback", json_group_tip_array_string, user_data_string, ""));



        }


        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMGroupAttributeChangedCallback))]
        private static void TIMGroupAttributeChangedCallbackInstance(IntPtr group_id, IntPtr json_group_attibute_array, IntPtr user_data)
        {
            string json_group_attibute_array_string = Utils.intptr2string(json_group_attibute_array);

            string group_id_string = Utils.intptr2string(group_id);

            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMGroupAttributeChangedCallback", json_group_attibute_array_string, user_data_string, "", 0, 0, 0, group_id_string));


        }
        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMConvEventCallback))]
        private static void TIMConvEventCallbackInstance(int conv_event, IntPtr json_conv_array, IntPtr user_data)
        {
            string json_conv_array_string = Utils.intptr2string(json_conv_array);

            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMConvEventCallback", json_conv_array_string, user_data_string, "", 0, 0, 0, "", conv_event));



        }


        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMConvTotalUnreadMessageCountChangedCallback))]
        private static void TIMConvTotalUnreadMessageCountChangedCallbackInstance(int total_unread_count, IntPtr user_data)
        {

            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(total_unread_count, "TIMConvTotalUnreadMessageCountChangedCallback", "", user_data_string));



        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMNetworkStatusListenerCallback))]
        private static void TIMNetworkStatusListenerCallbackInstance(int status, int code, IntPtr desc, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string desc_string = Utils.intptr2string(desc);



            mainSyncContext.Send(threadOperation, new CallbackConvert(status, "TIMNetworkStatusListenerCallback", "", user_data_string, desc_string, code));



        }
        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMKickedOfflineCallback))]
        private static void TIMKickedOfflineCallbackInstance(IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMKickedOfflineCallback", "", user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMUserSigExpiredCallback))]
        private static void TIMUserSigExpiredCallbackInstance(IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);




            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMUserSigExpiredCallback", "", user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMOnAddFriendCallback))]
        private static void TIMOnAddFriendCallbackInstance(IntPtr json_identifier_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_identifier_array_string = Utils.intptr2string(json_identifier_array);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMOnAddFriendCallback", json_identifier_array_string, user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMOnDeleteFriendCallback))]
        private static void TIMOnDeleteFriendCallbackInstance(IntPtr json_identifier_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_identifier_array_string = Utils.intptr2string(json_identifier_array);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMOnDeleteFriendCallback", json_identifier_array_string, user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMUpdateFriendProfileCallback))]
        private static void TIMUpdateFriendProfileCallbackInstance(IntPtr json_friend_profile_update_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_friend_profile_update_array_string = Utils.intptr2string(json_friend_profile_update_array);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMUpdateFriendProfileCallback", json_friend_profile_update_array_string, user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMFriendAddRequestCallback))]
        private static void TIMFriendAddRequestCallbackInstance(IntPtr json_friend_add_request_pendency_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_friend_add_request_pendency_array_string = Utils.intptr2string(json_friend_add_request_pendency_array);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMFriendAddRequestCallback", json_friend_add_request_pendency_array_string, user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMFriendApplicationListDeletedCallback))]
        private static void TIMFriendApplicationListDeletedCallbackInstance(IntPtr json_identifier_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_identifier_array_string = Utils.intptr2string(json_identifier_array);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMFriendApplicationListDeletedCallback", json_identifier_array_string, user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMFriendApplicationListReadCallback))]
        private static void TIMFriendApplicationListReadCallbackInstance(IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMFriendApplicationListReadCallback", "", user_data_string));


        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMFriendBlackListAddedCallback))]
        private static void TIMFriendBlackListAddedCallbackInstance(IntPtr json_friend_black_added_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_friend_black_added_array_string = Utils.intptr2string(json_friend_black_added_array);




            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMFriendBlackListAddedCallback", json_friend_black_added_array_string, user_data_string));



        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMFriendBlackListDeletedCallback))]
        private static void TIMFriendBlackListDeletedCallbackInstance(IntPtr json_identifier_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_identifier_array_string = Utils.intptr2string(json_identifier_array);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMFriendBlackListDeletedCallback", json_identifier_array_string, user_data_string));




        }

        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMLogCallback))]
        private static void TIMLogCallbackInstance(int level, IntPtr log, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);


            string log_string = Utils.intptr2string(log);



            mainSyncContext.Send(threadOperation, new CallbackConvert(level, "TIMLogCallback", log_string, user_data_string));


        }





        [MonoPInvokeCallback(typeof(IMNativeSDK.TIMMsgUpdateCallback))]
        public static void TIMMsgUpdateCallbackInstance(IntPtr json_msg_array, IntPtr user_data)
        {
            string user_data_string = Utils.intptr2string(user_data);

            string json_msg_array_string = Utils.intptr2string(json_msg_array);



            mainSyncContext.Send(threadOperation, new CallbackConvert(0, "TIMMsgUpdateCallback", json_msg_array_string, user_data_string));


        }
    }
}

