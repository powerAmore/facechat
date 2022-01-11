// AR_FOUNDATION_EDITOR_REMOTE: fix for Editor applied
#if UNITY_EDITOR
    #define IS_EDITOR
#endif
#undef UNITY_EDITOR
using ARFoundationRemote.Runtime;
// AR_FOUNDATION_EDITOR_REMOTE***

using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

namespace FaceChat
{
    // AR_FOUNDATION_EDITOR_REMOTE: fix for Editor applied
    #if IS_EDITOR
    using ARKitFaceSubsystem = ARFoundationRemote.Runtime.FaceSubsystem;
    #endif
    // AR_FOUNDATION_EDITOR_REMOTE***

    /// <summary>
    /// Populates the action unit coefficients for an <see cref="ARFace"/>.
    /// </summary>
    /// <remarks>
    /// If this <c>GameObject</c> has a <c>SkinnedMeshRenderer</c>,
    /// this component will generate the blend shape coefficients from the underlying <c>ARFace</c>.
    ///
    /// </remarks>
    [RequireComponent(typeof(ARFace))]
    public class ARKitBlendShapeCollector : MonoBehaviour
    {
        [SerializeField]
        float m_CoefficientScale = 100.0f;

        public float coefficientScale
        {
            get { return m_CoefficientScale; }
            set { m_CoefficientScale = value; }
        }

        public int m_encodeType;
        public byte[] m_byteFacialData;

#if UNITY_IOS && !UNITY_EDITOR
        ARKitFaceSubsystem m_ARKitFaceSubsystem;
    #endif

        ARFace m_Face;
        ARSessionOrigin m_Origin;

        void Awake()
        {
            m_Face = GetComponent<ARFace>();
            m_Origin = GameObject.Find("AR Session Origin").GetComponent<ARSessionOrigin>();
        }

        private void Start()
        {
            m_encodeType = GameObject.Find("AR Session Origin").GetComponent<BlendShapesDataContainer>().encodeType;
            m_byteFacialData = GameObject.Find("AR Session Origin").GetComponent<BlendShapesDataContainer>().byteFacialData;
        }

        void SetVisible(bool visible)
        {
            //if (skinnedMeshRenderer == null) return;

            //skinnedMeshRenderer.enabled = visible;
        }

        void UpdateVisibility()
        {
            var visible =
                enabled &&
                (m_Face.trackingState == TrackingState.Tracking) &&
                (ARSession.state > ARSessionState.Ready);

            SetVisible(visible);
        }

        void OnEnable()
        {
    #if UNITY_IOS && !UNITY_EDITOR
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null)
            {
                m_ARKitFaceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
            }
    #endif
            //UpdateVisibility();
            m_Face.updated += OnUpdated;
            ARSession.stateChanged += OnSystemStateChanged;
        }

        void OnDisable()
        {
            m_Face.updated -= OnUpdated;
            ARSession.stateChanged -= OnSystemStateChanged;
        }

        void OnSystemStateChanged(ARSessionStateChangedEventArgs eventArgs)
        {
            //UpdateVisibility();
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            //UpdateVisibility();
            UpdateFaceFeatures();
        }

        void UpdateFaceFeatures()
        {

    #if UNITY_IOS && !UNITY_EDITOR
            using (var blendShapes = m_ARKitFaceSubsystem.GetBlendShapeCoefficients(m_Face.trackableId, Allocator.Temp))
            {
                if (m_encodeType == 1)
                {
                    ushort[] ushortFacialDataArray = new ushort[3 + 52];

                    var rotationRelativeToCamera = Quaternion.Inverse(m_Origin.camera.transform.rotation) * m_Face.transform.rotation; // this quaternion represents a face rotation relative to camera
                    var eulerAnglesRelativeToCamera = rotationRelativeToCamera.eulerAngles;

                    // rotation信息为角度信息0~360度，将角度值除以1000再转换成半精度浮点数存储（unity里面半精度浮点数用ushort表示，除以1000用以将数值变小，以存储更多位数，提高精度）
                    ushortFacialDataArray[0] = Mathf.FloatToHalf(eulerAnglesRelativeToCamera.x / 1000);
                    ushortFacialDataArray[1] = Mathf.FloatToHalf(eulerAnglesRelativeToCamera.y / 1000);
                    ushortFacialDataArray[2] = Mathf.FloatToHalf(eulerAnglesRelativeToCamera.z / 1000);

                    foreach (var featureCoefficient in blendShapes)
                    {
                        int blendShapeLocationIndex = (int)featureCoefficient.blendShapeLocation;
                        ushortFacialDataArray[blendShapeLocationIndex + 3] = Mathf.FloatToHalf(featureCoefficient.coefficient); // blendshapes的值不用除以1000，因为blendshapes的值都是0到100间的数值，用半精度存储精度已经足够
                    }

                    Buffer.BlockCopy(ushortFacialDataArray, 0, m_byteFacialData, 1, m_byteFacialData.Length - 1);

                    //string strInfo = "";
                    //for (int i = 0; i < ushortFacialDataArray.Length; i++)
                    //{
                    //    if (i == 0)
                    //    {
                    //        float x = Mathf.HalfToFloat((ushort)ushortFacialDataArray.GetValue(i)) * 1000;
                    //        strInfo += x.ToString();
                    //    }
                    //    else if (i == 1)
                    //    {
                    //        float y = Mathf.HalfToFloat((ushort)ushortFacialDataArray.GetValue(i)) * 1000;
                    //        strInfo += ", " + y.ToString();
                    //    }
                    //    else if (i == 2)
                    //    {
                    //        float z = Mathf.HalfToFloat((ushort)ushortFacialDataArray.GetValue(i)) * 1000;
                    //        strInfo += ", " + z.ToString();
                    //    }
                    //    else
                    //    {
                    //        float coefficient = Mathf.HalfToFloat((ushort)ushortFacialDataArray.GetValue(i));
                    //        strInfo += ", " + coefficient.ToString();
                    //    }
                    //}

                    //string strInfo2 = "";
                    //for (int i = 0; i < m_byteFacialData.Length; i++)
                    //{
                    //    strInfo2 += ", " + m_byteFacialData.GetValue(i).ToString();
                    //}
                    //Debug.Log("strInfo2: " + strInfo2);
                }
                else if (m_encodeType == 2)
                {
                    ushort[] ushortFacialDataArray = new ushort[3];

                    var rotationRelativeToCamera = Quaternion.Inverse(m_Origin.camera.transform.rotation) * m_Face.transform.rotation; // this quaternion represents a face rotation relative to camera
                    var eulerAnglesRelativeToCamera = rotationRelativeToCamera.eulerAngles;

                    // rotation信息为角度信息0~360度，将角度值除以1000再转换成半精度浮点数存储（unity里面半精度浮点数用ushort表示，除以1000用以将数值变小，以存储更多位数，提高精度）
                    ushortFacialDataArray[0] = Mathf.FloatToHalf(eulerAnglesRelativeToCamera.x / 1000);
                    ushortFacialDataArray[1] = Mathf.FloatToHalf(eulerAnglesRelativeToCamera.y / 1000);
                    ushortFacialDataArray[2] = Mathf.FloatToHalf(eulerAnglesRelativeToCamera.z / 1000);

                    Buffer.BlockCopy(ushortFacialDataArray, 0, m_byteFacialData, 1, ushortFacialDataArray.Length * 2);

                    foreach (var featureCoefficient in blendShapes)
                    {
                        int blendShapeLocationIndex = (int)featureCoefficient.blendShapeLocation;
                        m_byteFacialData[blendShapeLocationIndex + 7] = (byte)((int)(featureCoefficient.coefficient * 100)); // blendshapes的值乘以100取整之后会是一个0~100的整数，可以用一个byte存储
                    }
                }

            }
#endif
        }
    }
}