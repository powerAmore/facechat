using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapesDataContainer : MonoBehaviour
{
    public byte[] byteFacialData = new byte[1 + 3 * 2 + 52 * 2]; // 第一个byte是编码type，接下来6个bytes是rotation数据，最后104个bytes是blendshapes数据
    public byte[] byteReceivedFacialData;


    // Start is called before the first frame update
    void Start()
    {
        byteFacialData[0] = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("byteFacialData.Length: " + byteFacialData.Length);
        //Debug.Log("byteReceivedFacialData.Length: " + byteReceivedFacialData.Length);

        //var ushortArray2 = new ushort[(byteFacialData.Length - 1) / 2];
        //Buffer.BlockCopy(byteFacialData, 1, ushortArray2, 0, byteFacialData.Length - 1);

        //string strInfo = "";
        //for (int i = 0; i < ushortArray2.Length; i++)
        //{
        //    if (i == 0)
        //    {
        //        float x = Mathf.HalfToFloat((ushort)ushortArray2.GetValue(i)) * 1000;
        //        strInfo += x.ToString();
        //    }
        //    else if (i == 1)
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
        //Debug.Log("BlendShapesDataContainer, strInfo: " + strInfo);

        var fps = 1.0f / Time.deltaTime;
        //Debug.Log("fps: " + fps.ToString());
    }
}
