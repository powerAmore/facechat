using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace com.tencent.imsdk.unity.utils {
    public class Utils {
        public static void  Log(string s){
            Debug.Log("TencentIMSDK："+s);
        }
        public static IntPtr string2intptr(string str){
            return Marshal.StringToHGlobalAnsi(str);
        }
        public static string intptr2string(IntPtr ptr){
            return Marshal.PtrToStringAnsi(ptr);
        }
        public static string getRandomStr(){
            
            return GetRandomString(20);
        }
        public static void toJson(){

        }

        public static void fromJson(){

        }

        public static string GetRandomString(int length)
        {
            string result = "";
            for (int i = 0; i < length; i++)
            {
                char c = (char)new System.Random(Guid.NewGuid().GetHashCode()).Next(97, 123);
                result += c;
            }
            return result;
        }

        
    }
}