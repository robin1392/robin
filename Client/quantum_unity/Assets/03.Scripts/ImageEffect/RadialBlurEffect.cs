using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RadialBlurEffect : MonoBehaviour
{
    Shader shader;
    public Material material;

    public float blurStrength = 2.2f;
    public float blurWidth = 1.0f;

    //private Material material = null;
    //private bool isOpenGL;

    public bool m_isStartBlur = false;

    private float m_fSpeed = 2.8f;

    private Material GetMaterial()
    {
        if (material == null)
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
        }
        return material;
    }

    void Start()
    {
        if (shader == null)
        {
            shader = Shader.Find("SSG Shader/RadialBlurFilter");
        }
        //isOpenGL = SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
        m_isStartBlur = false;
    }


    public void Update()
    {
        UpdateCheckBlur();
    }

    public void UpdateCheckBlur()
    {
        if(m_isStartBlur == true)
        {
            blurStrength += Time.deltaTime* m_fSpeed;
                        
            if(blurStrength > 3.0f)
            {
                blurStrength = 0.0f;
                m_isStartBlur = false;

                this.enabled = false;
            }
        }
    }

    public void RadialStart()
    {
        blurStrength = 0.0f;
        m_isStartBlur = true;
    }

    public void RadialEnd()
    {
        blurStrength = 0.0f;
        m_isStartBlur = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        //If we run in OpenGL mode, our UV coords are
        //not in 0-1 range, because of the texRECT sampler
        float ImageWidth = 1;
        float ImageHeight = 1;
        //if (isOpenGL)
        //{
        //    ImageWidth = source.width;
        //    ImageHeight = source.height;
        //}

        //GetMaterial().SetTexture("_MainTex", source);
        //GetMaterial().SetFloat("_SampleDist", blurWidth);
        //GetMaterial().SetFloat("_SampleStrength", blurStrength);        

        //GetMaterial().SetFloat("_BlurWidth", blurWidth);
        //GetMaterial().SetFloat("_BlurStrength", blurStrength);

        GetMaterial().SetFloat("_BlurStrength", blurStrength);
        GetMaterial().SetFloat("_BlurWidth", blurWidth);
        GetMaterial().SetFloat("_imgHeight", ImageWidth);
        GetMaterial().SetFloat("_imgWidth", ImageHeight);
        
        Graphics.Blit(source, dest, GetMaterial());
    }
}