using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Visualizer : MonoBehaviour
{
    public enum BandOptions
    {
       O8=8,
       O64=64
    }

    

    public AudioSource _audioSource;
    public GameObject cubePrefab;
    float[] _samples = new float[512];
    float[] _frequencyBands;
   

    [Header("Full Spectrum")]
    public float minScale_f=2f;
    public float maxScale_f=150f;
    public float radius_f=200f;
    public GameObject fullSpectrum_Parent;

    [Header("Reduced Spectrum")]
    public BandOptions reducedBand_size;
    public float minScale_r=10f;
    public float maxScale_r=100f;
    public float radius_r=150f;
    public GameObject reducedSpectrum_Parent;

    private GameObject[] full_sampleCubes = new GameObject[512];
    private GameObject[] reduced_sampleCubes;

    void Start()
    {
       if (reducedBand_size !=0)
        {
            _frequencyBands = new float[(int)reducedBand_size];
            reduced_sampleCubes = new GameObject[(int)reducedBand_size];
        }
        CreateLargeSpectrum();
        CreateSmallSpectrum();
    }


    void Update()
    {
        GetSpetrumAudioSource();
        if (fullSpectrum_Parent.activeInHierarchy)
        {
            ScaleLargeSpectrum();
        }

        if (reducedSpectrum_Parent.activeInHierarchy)
        {
            if ((int)reducedBand_size == 8)
            {
                GetFrequencyBand8();
            }else
            {
                GetFrequencyBand64();
            }

            ScaleReducedSpectrum();
            
        }
        
    }

    
    void CreateLargeSpectrum()
    {
        for (int i = 0; i < full_sampleCubes.Length; i++)
        {
            var angle = i * Mathf.PI * 2 / full_sampleCubes.Length;
            var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius_f;

            GameObject cubeInstance = (GameObject)Instantiate(cubePrefab);


            cubeInstance.name = cubePrefab.name + i;
            cubeInstance.transform.eulerAngles = new Vector3(0, angle, 0);
            cubeInstance.transform.position = pos;

            cubeInstance.transform.parent = fullSpectrum_Parent.transform;
            
            full_sampleCubes[i] = cubeInstance;

        }
        fullSpectrum_Parent.transform.eulerAngles = new Vector3(0, 30f, 0);
    }
    void CreateSmallSpectrum()
    {
        for (int i = 0; i < (int)reducedBand_size; i++)
        {
            var angle = i * (Mathf.PI/2) / reduced_sampleCubes.Length;
            var rot = i * Mathf.PI*2 / reduced_sampleCubes.Length;
            var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius_r;

            GameObject cubeInstance = Instantiate(cubePrefab);

            cubeInstance.GetComponent<MeshRenderer>().material.color = Color.red;

            cubeInstance.name = cubePrefab.name + i;
            cubeInstance.transform.eulerAngles = new Vector3(0, rot, 0);
            cubeInstance.transform.position = pos;
            cubeInstance.transform.localScale = ((int)reducedBand_size==8)? Vector3.one * minScale_r : Vector3.one;
            cubeInstance.transform.parent = reducedSpectrum_Parent.transform;

            reduced_sampleCubes[i] = cubeInstance;

        }
        reducedSpectrum_Parent.transform.eulerAngles = new Vector3(0, 50f, 0);
    }

    void ScaleLargeSpectrum()
    {
        for (int i = 0; i < full_sampleCubes.Length; i++)
        {
            if (full_sampleCubes[i] != null)
            {
                full_sampleCubes[i].transform.localScale = new Vector3(1,(_samples[i]*maxScale_f)+minScale_f, 1);
            }

        }
    }

    void ScaleReducedSpectrum()
    {
        float min = ((int)reducedBand_size == 8) ? minScale_r : 1.5f;

        for (int i = 0; i < reduced_sampleCubes.Length; i++)
        {
            reduced_sampleCubes[i].transform.localScale = new Vector3(min, (_frequencyBands[i] * maxScale_r) + min, min);
        }
    }

    void GetFrequencyBand8()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i==7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }
            average /= count;
            _frequencyBands[i] = average;
        }
    }

    /*
     0-15 : 1 sample (16)    0-15
     16-31 : 2 samples (32) 16-47
     32-39 : 4 sample (32)   48-79
     40-47 : 6 sample (48)   80-127
     48-55 : 16 sample (128) 128-255
     56-63:  32 sample (256)  256-511
         */
    void GetFrequencyBand64()
    {
        
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for (int i = 0; i < 64; i++)
        {
            float average = 0;

            //sampleCount = (int)Mathf.Pow(2, power) ;

            if (i == 16 || i==32 || i==40 ||i == 48 || i==56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power==3)
                    sampleCount -= 2;
                
            }

            for (int j = 0; j < sampleCount; j++)
            {
                //if (count > 511) break;
                
                average += _samples[count] * (count + 1);

                count++;
            }
            
            average /= count;
            _frequencyBands[i] = average;
        }
    }

    void GetSpetrumAudioSource()
    {
        if (_audioSource != null)
        {
            //0 is the left channel, 1 is the right
            //for stereo spectrum combine both channels
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);
        }
    }
}
