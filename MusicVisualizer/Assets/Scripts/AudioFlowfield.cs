﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(NoiseFlowfield))]
//[RequireComponent(typeof(AudioVisualizer))] not sure if we should just put all the components on one object
public class AudioFlowfield : MonoBehaviour
{
    NoiseFlowfield noiseFlowfield;
    [Header("Speed")]
    public bool useSpeed;
    public Vector2 moveSpeedMinMax, rotateSpeedMinMax;
    [Header("Scale")]
    public bool useScale;
    public Vector2 scaleMinMax;
    [Header("Material")]
    public Material material;
    private Material[] audioMaterials;
    public bool useColor1, useColor2;
    public string colorName1, colorName2;
    public Gradient gradient1, gradient2;
    private Color[] color1, color2;
    [Range(0f, 1f)]
    public float colorThreshold1, colorThreshold2;
    public float colorMultiplier1, colorMultiplier2;

    // Start is called before the first frame update
    void Start()
    {
        noiseFlowfield = GetComponent<NoiseFlowfield>();
        audioMaterials = new Material[8];
        color1 = new Color[8];
        color2 = new Color[8];
        for(int i = 0; i < 8; i++)
        {
            color1[i] = gradient1.Evaluate((1f / 8f) * i);
            color2[i] = gradient2.Evaluate((1f / 8f) * i);
            audioMaterials[i] = new Material(material);
        }
        int pid = 0;
        foreach(FlowfieldParticle p in noiseFlowfield.particles)
        {
            noiseFlowfield.particleMeshRenderers[pid].material = audioMaterials[p.audioBand];
            pid++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (useSpeed)
        {
            noiseFlowfield.particleMoveSpeed = Mathf.Lerp(moveSpeedMinMax.x, moveSpeedMinMax.y, AudioVisualizer.amplitudeBuffer);
            noiseFlowfield.particleRotSpeed = Mathf.Lerp(rotateSpeedMinMax.x, rotateSpeedMinMax.y, AudioVisualizer.amplitudeBuffer);
        }
        for(int i = 0; i < noiseFlowfield.amountofParticles; i++)
        {
            if (useScale)
            {
                float scale = Mathf.Lerp(scaleMinMax.x, scaleMinMax.y, AudioVisualizer.audioBandBuffer[noiseFlowfield.particles[i].audioBand]);
                noiseFlowfield.particles[i].scale = scale;
            }
        }
        for(int j = 0; j < 8; j++)
        {
            if (useColor1)
            {
                if(AudioVisualizer.audioBandBuffer[j] > colorThreshold1)
                {
                    audioMaterials[j].SetColor(colorName1, color1[j] * AudioVisualizer.audioBandBuffer[j] * colorMultiplier1);
                }
                else
                {
                    audioMaterials[j].SetColor(colorName1, color1[j] * 0f);
                }
            }
            if (useColor2)
            {
                if (AudioVisualizer.audioBandBuffer[j] > colorThreshold2)
                {
                    audioMaterials[j].SetColor(colorName2, color2[j] * AudioVisualizer.audioBandBuffer[j] * colorMultiplier2);
                }
                else
                {
                    audioMaterials[j].SetColor(colorName2, color2[j] * 0f);
                }
            }
        }
    }
}
