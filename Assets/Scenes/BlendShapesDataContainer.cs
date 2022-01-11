using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapesDataContainer : MonoBehaviour
{
    public int encodeType;
    public byte[] byteFacialData;

    //public byte[] byteFacialData = new byte[1 + 3 * 2 + 52 * 2]; // 第一个byte是编码type，接下来6个bytes是rotation数据，最后104个bytes是blendshapes数据
    public byte[] byteReceivedFacialData;

    void Awake()
    {
        encodeType = 2;
        if (encodeType == 1)
        {
            byteFacialData = new byte[1 + 3 * 2 + 52 * 2]; // 第一个byte是编码type，接下来6个bytes是rotation数据，最后104个bytes是blendshapes数据
            byteFacialData[0] = 1;
        }
        else if (encodeType == 2)
        {
            byteFacialData = new byte[1 + 3 * 2 + 52]; // 第一个byte是编码type，接下来6个bytes是rotation数据，最后52个bytes是blendshapes数据，这里的blendshapes数据只包含0~100的整数
            byteFacialData[0] = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (encodeType == 1)
        {
            //var ushortArray2 = new ushort[(byteFacialData.Length-1) / 2];
            //Buffer.BlockCopy(byteFacialData, 1, ushortArray2, 0, byteFacialData.Length-1);

            //string strInfo = "";
            //for (int i = 0; i < ushortArray2.Length; i++)
            //{
            //    if (i == 0)
            //    {
            //        float x = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(i)) * 1000;
            //        strInfo += x.ToString();
            //    }
            //    else if(i == 1)
            //    {
            //        float y = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(i)) * 1000;
            //        strInfo += ", " + y.ToString();
            //    }
            //    else if (i == 2)
            //    {
            //        float z = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(i)) * 1000;
            //        strInfo += ", " + z.ToString();
            //    }
            //    else
            //    {
            //        float coefficient = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(i));
            //        strInfo += ", " + coefficient.ToString();
            //    }
            //}
            //Debug.Log(strInfo);
        }
        else if (encodeType == 2)
        {
            var ushortArray2 = new ushort[3];
            Buffer.BlockCopy(byteFacialData, 1, ushortArray2, 0, ushortArray2.Length * 2);

            string strInfo = "";
            float x = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(0)) * 1000;
            strInfo += x.ToString();
            float y = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(1)) * 1000;
            strInfo += ", " + y.ToString();
            float z = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(2)) * 1000;
            strInfo += ", " + z.ToString();

            for (int i = 7; i < byteFacialData.Length; i++)
            {
                int coefficient = byteFacialData[i];
                strInfo += ", " + coefficient.ToString();
            }
            //Debug.Log(strInfo);
        }

        var fps = 1.0f / Time.deltaTime;
        //Debug.Log("fps: " + fps.ToString());
    }
}
