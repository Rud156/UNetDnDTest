using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Extras;

public class SpaceshipSpawner : NetworkBehaviour
{
    #region Singleton

    public static SpaceshipSpawner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
        if(instance != this)
            Destroy(gameObject);
    }

    #endregion Singleton
    
    public GameObject smallSpaceship;
    public GameObject largeSpaceship;

    [Header("Debug")] public bool spawnSmallShipOnStart;
    public bool spawnLargeShipOnStart;

    private Transform _spaceshipHolder;
    
    private Vector3 _topLeft;
    private Vector3 _bottomRight;

    private void Start()
    {
        _spaceshipHolder = GameObject.FindGameObjectWithTag(TagManager.SpaceshipHolder)?.transform;
        
        Camera mainCamera = Camera.main;
        _topLeft = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
        _bottomRight =
            mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));
        
        if(spawnSmallShipOnStart)
            SpawnSmallSpaceship();
        
        if(spawnLargeShipOnStart)
            SpawnLargeSpaceship();
    }

    private void SpawnSmallSpaceship()
    {
        float randomY = ExtensionFunctions.Map(Random.value, 0, 1,
            _bottomRight.y, _topLeft.y);

        GameObject smallSpaceshipInstance = Instantiate(smallSpaceship,
            new Vector3(_topLeft.x - 2, randomY), Quaternion.identity);
        smallSpaceshipInstance.transform.SetParent(_spaceshipHolder);
        
        NetworkServer.Spawn(smallSpaceshipInstance);
    }

    private void SpawnLargeSpaceship()
    {
        float randomY = ExtensionFunctions.Map(Random.value, 0, 1,
            _bottomRight.y, _topLeft.y);

        GameObject largeSpaceshipInstance = Instantiate(largeSpaceship,
            new Vector3(_topLeft.x - 2, randomY), Quaternion.identity);
        largeSpaceshipInstance.transform.SetParent(_spaceshipHolder);
        
        NetworkServer.Spawn(largeSpaceshipInstance);
    }
}