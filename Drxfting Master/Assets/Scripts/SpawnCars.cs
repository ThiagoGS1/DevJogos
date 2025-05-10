using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnCars : MonoBehaviour
{
    int numberOfCarsSpawned = 0;
    private GameObject car;
    private GameObject playerCar; // Reference to player's car
    public Camera cam;
    [Header("Camera Settings")]
    public float cameraZoom = 5f;  // Para câmera ortográfica (valor menor = mais zoom)
    public float cameraFieldOfView = 40f;  // Para câmera perspectiva (valor menor = mais zoom)

    // Start is called before the first frame update
    void Start()
    {
        // Aplicar configurações de zoom à câmera
        ApplyCameraZoom();
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //Ensure that the spawn points are sorted by name
        spawnPoints = spawnPoints.ToList().OrderBy(s => s.name).ToArray();

        //Load the car data
        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");

        //Driver info
        List<DriverInfo> driverInfoList = new List<DriverInfo>(GameManager.instance.GetDriverList());

        //Sort the drivers based on last position
        driverInfoList = driverInfoList.OrderBy(s => s.lastRacePosition).ToList();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i].transform;

            if (driverInfoList.Count == 0)
                return;

            DriverInfo driverInfo = driverInfoList[0];

            int selectedCarID = driverInfo.carUniqueID;

            //Find the selected car
            foreach (CarData cardata in carDatas)
            {
                //We found the car data for the player
                if (cardata.CarUniqueID == selectedCarID)
                {
                    //Now spawn it on the spawnpoint
                    car = Instantiate(cardata.CarPrefab, spawnPoint.position, spawnPoint.rotation);

                    car.name = driverInfo.name;

                    car.GetComponent<CarInputHandler>().playerNumber = driverInfo.playerNumber;

                    if (driverInfo.isAI)
                    {
                        car.GetComponent<CarInputHandler>().enabled = false;
                        car.tag = "AI";
                    }
                    else
                    {
                        car.GetComponent<CarAIHandler>().enabled = false;
                        car.GetComponent<AStarLite>().enabled = false;
                        car.tag = "Player";
                        playerCar = car; // Store reference to player's car
                    }

                    numberOfCarsSpawned++;

                    break;
                }
            }

            //Remove the spawned driver
            driverInfoList.Remove(driverInfo);
        }
    }

    public int GetNumberOfCarsSpawned()
    {
        return numberOfCarsSpawned;
    }

    void Update()
    {
        // Find object with Player tag if we don't have the reference
        if (playerCar == null)
        {
            playerCar = GameObject.FindGameObjectWithTag("Player");
        }

        // Only update camera position if we found the player
        if (playerCar != null)
        {
            cam.transform.position = new Vector3(playerCar.transform.position.x, playerCar.transform.position.y, cam.transform.position.z);
        }
    }

    private void ApplyCameraZoom()
    {
        if (cam == null)
            return;

        if (cam.orthographic)
        {
            // Para câmera ortográfica
            cam.orthographicSize = cameraZoom;
        }
        else
        {
            // Para câmera perspectiva
            cam.fieldOfView = cameraFieldOfView;
        }
    }

}
