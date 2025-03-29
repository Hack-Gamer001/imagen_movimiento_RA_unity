using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems; // Agregar el namespace para TrackingState

public class ImageTrackerManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager aRTrackedImageManager;
    private VideoPlayer videoLayer;
    private bool IsImageTrackable = false; // Declarar la variable correctamente

    private void Awake()
    {
        if (aRTrackedImageManager == null)
        {
            aRTrackedImageManager = FindObjectOfType<ARTrackedImageManager>(); // Intentar asignarlo automáticamente
        }
    }

    private void OnEnable()
    {
        if (aRTrackedImageManager != null)
        {
            aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
        }
        else
        {
            Debug.LogError("ARTrackedImageManager no está asignado en el Inspector.");
        }
    }

    private void OnDisable()
    {
        if (aRTrackedImageManager != null)
        {
            aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        }
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventData)
    {
        foreach (var trackedImage in eventData.added)
        {
            videoLayer = trackedImage.GetComponentInChildren<VideoPlayer>();
            if (videoLayer != null)
            {
                videoLayer.Play();
            }
            else
            {
                Debug.LogWarning("No se encontró VideoLayer en la imagen rastreada.");
            }
        }

        foreach (var trackedImage in eventData.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                if (!IsImageTrackable) // Evita activaciones innecesarias
                {
                    IsImageTrackable = true;
                    if (videoLayer != null)
                    {
                        videoLayer.gameObject.SetActive(true);
                        videoLayer.Play();
                    }
                }
            }
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                if (IsImageTrackable)
                {
                    IsImageTrackable = false;
                    if (videoLayer != null)
                    {
                        videoLayer.gameObject.SetActive(false);
                        videoLayer.Pause();
                    }
                }
            }
        }
    }
}
