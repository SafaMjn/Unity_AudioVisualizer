using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Visualizer : MonoBehaviour
{
    public AudioSource _audioSource;
    public GameObject cubePrefab;
    float[] _samples = new float[512];
    float[] _frequencyBands = new float[8];

    [Header("Full Spectrum")]
    public float minScale_f;
    public float maxScale_f;
    public float radius_f;
    public GameObject fullSpectrum_Parent;

    [Header("Reduced Spectrum")]
    public float minScale_r;
    public float maxScale_r;
    public float radius_r;
    public GameObject reducedSpectrum_Parent;

    private GameObject[] full_sampleCubes = new GameObject[512];
    private GameObject[] reduced_sampleCubes = new GameObject[8];

    // Start is called before the first frame update
    void Start()
    {
       // _audioSource = GetComponent<AudioSource>();

        CreateLargeSpectrum();
        CreateSmallSpectrum();
    }


    // Update is called once per frame
    void Update()
    {
        GetSpetrumAudioSource();
        if (fullSpectrum_Parent.activeInHierarchy)
        {
            
            ScaleLargeSpectrum();
        }

        if (reducedSpectrum_Parent.activeInHierarchy)
        {
            GetFrequencyBands();
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
        for (int i = 0; i < reduced_sampleCubes.Length; i++)
        {
            var angle = i * (Mathf.PI/2) / reduced_sampleCubes.Length;
            var rot = i * Mathf.PI*2 / reduced_sampleCubes.Length;
            var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius_r;

            GameObject cubeInstance = Instantiate(cubePrefab);

            cubeInstance.GetComponent<MeshRenderer>().material.color = Color.red;

            cubeInstance.name = cubePrefab.name + i;
            cubeInstance.transform.eulerAngles = new Vector3(0, rot, 0);
            cubeInstance.transform.position = pos;
            cubeInstance.transform.localScale = Vector3.one * minScale_r;
            cubeInstance.transform.parent = reducedSpectrum_Parent.transform;

            reduced_sampleCubes[i] = cubeInstance;

        }
        reducedSpectrum_Parent.transform.eulerAngles = new Vector3(0, 45f, 0);
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
        for (int i = 0; i < reduced_sampleCubes.Length; i++)
        {
            if (reduced_sampleCubes[i] != null)
            {
                reduced_sampleCubes[i].transform.localScale = new Vector3(minScale_r,(_frequencyBands[i]*maxScale_r)+minScale_r, minScale_r);
            }

        }
    }

    void GetFrequencyBands()
    {
        // _frequencyBands
        int count = 0;
        for (int i = 0; i < _frequencyBands.Length; i++)
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

    void GetSpetrumAudioSource()
    {
        if (_audioSource != null)
        {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);
        }
    }
}
