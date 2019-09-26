using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 渲染箱子  渲染对象本身和立方体的八个顶点
/// </summary>
public struct RenderBoundingBox
{
    public Renderer prenderer;
    public Vector3 p000;
    public Vector3 p001;
    public Vector3 p010;
    public Vector3 p011;
    public Vector3 p100;
    public Vector3 p101;
    public Vector3 p110;
    public Vector3 p111;

}



public class RenderBoundsManager : MonoBehaviour
{
    /// <summary>
    /// 渲染集合
    /// </summary>
    public GameObject RenderAssets;
    /// <summary>
    /// 分割面
    /// </summary>
    public Transform SplitFace;

    /// <summary>
    /// 渲染列表
    /// </summary>
    List<Renderer> mRenderers = new List<Renderer>();
    /// <summary>
    /// 渲染箱子列表
    /// </summary>
    List<RenderBoundingBox> mRenderBoundingBoxs = new List<RenderBoundingBox>();
    /// <summary>
    /// 查询渲染集合下的所有参与渲染的对象并初始化 渲染箱子列表
    /// </summary>
    public void InitializedRenderBoundingBox()
    {
        if(RenderAssets)
        {
            Renderer[] renderers = RenderAssets.GetComponentsInChildren<Renderer>();
            mRenderers.Clear();
            mRenderers.AddRange(renderers);


            mRenderBoundingBoxs.Clear();

            for(int i=0;i< renderers.Length;i++)
            {
                RenderBoundingBox boundingBox= GetRenderBoundingBox(renderers[i]);
                mRenderBoundingBoxs.Add(boundingBox);
            }


        }
        else
        {
            Debug.LogError("RenderAssets is null");
        }
    }
    /// <summary>
    /// 获取一个静态对象的渲染包裹箱
    /// </summary>
    /// <param name="renderer"></param>
    /// <returns></returns>
    public RenderBoundingBox GetRenderBoundingBox(Renderer renderer)
    {
        RenderBoundingBox boundingBox = new RenderBoundingBox();
        boundingBox.prenderer = renderer;
        // A sphere that fully encloses the bounding box.
        Vector3 center = renderer.bounds.center - renderer.transform.position;
        Vector3 halfsize = renderer.bounds.size * 0.5f;
        boundingBox. p000 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-halfsize.x, -halfsize.y, -halfsize.z));
        boundingBox.p001 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-halfsize.x, -halfsize.y, halfsize.z));
        boundingBox.p010 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-halfsize.x, halfsize.y, -halfsize.z));
        boundingBox.p011 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-halfsize.x, halfsize.y, halfsize.z));
        boundingBox.p100 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(halfsize.x, -halfsize.y, -halfsize.z));
        boundingBox.p101 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(halfsize.x, -halfsize.y, halfsize.z));
        boundingBox.p110 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(halfsize.x, halfsize.y, -halfsize.z));
        boundingBox.p111 = renderer.transform.localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(halfsize.x, halfsize.y, halfsize.z));
        return boundingBox;
    }

    /// <summary>
    /// 检查对象是否在分割面的前方（是否在视角前方）
    /// </summary>
    /// <param name="mSplitFace">分割面</param>
    /// <param name="boundingBox">每个对象包裹的箱子</param>
    /// <returns></returns>
    public bool RenderBoundsOnRender(Transform mSplitFace, RenderBoundingBox  boundingBox)
    {
 
        Vector3 p000 = boundingBox.p000;
        Vector3 p001 = boundingBox.p001;
        Vector3 p010 = boundingBox.p010;
        Vector3 p011 = boundingBox.p011;
        Vector3 p100 = boundingBox.p100;
        Vector3 p101 = boundingBox.p101;
        Vector3 p110 = boundingBox.p110;
        Vector3 p111 = boundingBox.p111;

        Plane plane = new Plane(mSplitFace.forward, mSplitFace.position);
 
        bool bp000 = plane.GetSide(p000);
        bool bp001 = plane.GetSide(p001);
        bool bp010 = plane.GetSide(p010);
        bool bp011 = plane.GetSide(p011);
        bool bp100 = plane.GetSide(p100);
        bool bp101 = plane.GetSide(p101);
        bool bp110 = plane.GetSide(p110);
        bool bp111 = plane.GetSide(p111);

        if (!bp000 && !bp001 && !bp010 && !bp011 && !bp100 && !bp101 && !bp110 && !bp111)
        {
            return false;
        }
        else
        {
            return true;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        InitializedRenderBoundingBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (mRenderBoundingBoxs.Count > 0 && SplitFace)
        {
            for (int i = 0; i < mRenderBoundingBoxs.Count; i++)
            {
                RenderBoundingBox boundingBox = mRenderBoundingBoxs[i];
                bool EnableFlag = RenderBoundsOnRender(SplitFace, boundingBox);

                if (boundingBox.prenderer)
                {
                    if (boundingBox.prenderer.gameObject)
                    {
                        boundingBox.prenderer.gameObject.SetActive(EnableFlag);
                    }
                }

            }
        }

    }
}
