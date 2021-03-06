﻿using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Loads the X3D models and handles their instantiation
/// </summary>
public class ModelLoadManager
{

    public string baseUrl = "/resources/model/";
    private Shader shader;
    private GameObject boundingBoxPrefab;
    public Vector3 spawnPosition;
    public Vector3 spawnEulerAngles;
    public Transform globalSpawnParent;
    private X3DObj x3dObject;
    private BoundingBoxId boundingBoxId;
    private bool remotelySpawned;
    private GameObject localPrefab;

    private Action callback;

    public ModelLoadManager(Vector3 spawnPosition, Transform globalSpawnParent, BoundingBoxId boundingBoxId, bool remotelySpawned)
    {
        shader = ModelLoadSettings.Instance.shader;
        boundingBoxPrefab = ModelLoadSettings.Instance.boundingBox;
        spawnEulerAngles = new Vector3(0, 180, 0);
        this.spawnPosition = spawnPosition;
        this.globalSpawnParent = globalSpawnParent;
        this.remotelySpawned = remotelySpawned;
        this.boundingBoxId = boundingBoxId;
    }

    public ModelLoadManager(Vector3 spawnPosition, Transform globalSpawnParent, BoundingBoxId boundingBoxId, bool remotelySpawned, GameObject prefab) {
        shader = ModelLoadSettings.Instance.shader;
        boundingBoxPrefab = ModelLoadSettings.Instance.boundingBox;
        spawnEulerAngles = new Vector3(0, 180, 0);
        this.spawnPosition = spawnPosition;
        this.globalSpawnParent = globalSpawnParent;
        this.remotelySpawned = remotelySpawned;
        this.boundingBoxId = boundingBoxId;
        this.localPrefab = prefab;
    }

    /// <summary>
    /// Call this to load a new X3D object
    /// </summary>
    /// <param name="name">The name of the X3D object</param>
    public void Load(string name)
    {
        x3dObject = new X3DObj(RestManager.Instance, InformationManager.Instance.FullBackendAddress + baseUrl, name, shader);
        x3dObject.LoadGameObjects(OnFinished); // this automatically creates them
        WaitCursor.Show();
    }

    public void Load(string name, Action callback)
    {
        this.callback = callback;
        Load(name);
    }

    public ModelLoadManager LoadLocal() {
        if (localPrefab != null) {
            // Load object specific content
            GameObject localModel = GameObject.Instantiate(localPrefab);
            localModel.name = "X3D Parent";
            Collider collider = localModel.transform.GetChild(0).GetComponent<Collider>();
            ObjectInfo info = localModel.GetComponent<ObjectInfo>();

            info.Bounds = collider.bounds;

            AttachementManager attachmentManager = localModel.AddComponent<AttachementManager>();
            attachmentManager.Init();

            CreateBoundingBox(localModel, info.Bounds);

            CreateGamificationGame(info.ModelName.ToLower());

            if (callback != null) {
                callback();
            }
        } else {
            Debug.Log("Tried to load local null model");
        }
        return this;
    }

    /// <summary>
    /// Called when the X3D object has finished loading
    /// creates GameObjects from the X3D object
    /// and also instantiates a bounding box around the imported object
    /// </summary>
    private void OnFinished()
    {
        WaitCursor.Hide();
        GameObject obj = x3dObject.CreateGameObjects();
        CreateBoundingBox(obj, x3dObject.Bounds);

        CreateGamificationGame(x3dObject.ModelName.ToLower());

        if (callback != null)
        {
            callback();
        }
    }

    /// <summary>
    /// Creates a bounding box around a GameObject
    /// </summary>
    /// <param name="content">The GameObject which should be encompassed by the bounding box</param>
    /// <param name="bounds">The bounds to encompass (usually the GameObject's bounds or
    /// combined bounds with its children)</param>
    void CreateBoundingBox(GameObject content, Bounds bounds)
    {
        // create a bounding-box around the object
        GameObject box = GameObject.Instantiate(boundingBoxPrefab);
        box.transform.localScale = bounds.size;
        Transform contentHolder = box.transform.Find("Content");
        content.transform.parent = contentHolder;

        // set the correct position and rotation
        box.transform.position = spawnPosition;
        box.transform.localRotation = Quaternion.Euler(spawnEulerAngles);

        BoundingBoxInfo info = box.GetComponent<BoundingBoxInfo>();
        info.Id = boundingBoxId;

        if (globalSpawnParent != null)
        {
            box.transform.parent = globalSpawnParent;
        }

        if(remotelySpawned)
        {
            CustomTapToPlace tapToPlace = box.GetComponent<CustomTapToPlace>();
            tapToPlace.IsBeingPlaced = false;
        }
    }

    private void CreateGamificationGame(string modelName)
    {
        GamificationFramework.Instance.CreateGame(new Game(modelName),
            (createdGame, createcode) =>
            {
                GamificationFramework.Instance.AddUserToGame(modelName, respondeCode =>
                {
                    if (respondeCode != 200)
                    {
                        Debug.Log("Could not add user to the game");
                    }
                    else
                    {
                        Debug.Log("User successfully added to game " + modelName);
                    }
                });


                Badge defaultBadge = new Badge("defaultBadge", "default badge", "a default badge which is assigned if no other badge was selected");
                defaultBadge.Image = (Texture2D)Resources.Load("DefaultBadge");
                // create a default badge
                GamificationFramework.Instance.CreateBadge(modelName, defaultBadge, null);

                // default action which is created so that the list of quest actions can never be empty
                // if a quest with an emtpy action list exists in the Gamification Framework, the quest becomes inaccessible
                GameAction defaultAction = new GameAction("defaultAction", "default action", "a default action", 0);
                GamificationFramework.Instance.CreateAction(modelName, defaultAction, null);
            }
            );
    }
}
