using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int currentStageID = -1;
    [HideInInspector] public Stage currentStage;
    public StageDataBuffer stageDataBuffer;

    [SerializeField] private int roomCount;
    [SerializeField] private int stageEdgeLength;
    private readonly Dictionary<Vector2, Room> roomDic = new();
    private readonly List<RoomMold> roomMolds = new();
    private class RoomMold
    {
        public Vector2 Coordinate;
        public readonly bool[] IsConnectToNearbyRoom = new bool[4];
    }
    private List<Room> roomData = new();
    public Room CurrentRoom { get; private set; }
    [SerializeField] private GameObject stageGrid;

    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GridLayoutGroup mapGridLayoutGroup;
    [SerializeField] private RectTransform scrollRectBackGround;
    private Dictionary<Vector2, Transform> roomUiDic = new();

    [SerializeField] private Animator fadePanelAnimator;

    [SerializeField] private GameObject shortCut;

    private WaitForSeconds ws02;

    [SerializeField] private BoolVariable isFighting;
    [SerializeField] private BoolVariable isGaming;
    [SerializeField] private BoolVariable isFocusOnSomething;
    [SerializeField] private BoolVariable isLoading;

    [SerializeField] private Animator stageLoading;
    [SerializeField] private Animator stageLoading2;
    [SerializeField] private Image[] stageIcons;
    [SerializeField] private Image[] stamps;

    private void Awake()
    {
        Instance = this;
        ws02 = new WaitForSeconds(.2f);
    }

    private void Update()
    {
        if (isFocusOnSomething.RuntimeValue)
        {
            if (mapPanel.activeSelf) mapPanel.SetActive(false);
            if (shortCut.activeSelf) shortCut.SetActive(false);
        }
        else
        {
            if (isGaming.RuntimeValue)
            {
                if (Input.GetKeyDown(KeyCode.Tab) && mapPanel.activeSelf == false) MapDoor(true);
                else if (Input.GetKeyUp(KeyCode.Tab) && mapPanel.activeSelf) MapDoor(false);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Tab) && shortCut.activeSelf == false) shortCut.SetActive(true);
                else if (Input.GetKeyUp(KeyCode.Tab) && shortCut.activeSelf) shortCut.SetActive(false);
            }
        }
    }

    public void GenerateStage()
    {
        shortCut.SetActive(false);
        mapPanel.SetActive(false);

        DestroyStage();
        roomMolds.Clear();
        roomDic.Clear();

        currentStage = stageDataBuffer.items[++currentStageID];
        roomData = new List<Room>(currentStage.roomDatas);
        roomMolds.Add(new RoomMold { Coordinate = Vector2.zero });

        int _roomCount = currentStage.id == 666 ? 1 : roomCount;

        /* 스테이지 틀 만들기 */
        {
            Vector2 totalRoomMoldCoordinate;

            while (roomMolds.Count < _roomCount)
            {
                RoomMold originalRoomMold = roomMolds[Random.Range(0, roomMolds.Count)];
                int doorOpenIndex = Random.Range(0, 4);
                totalRoomMoldCoordinate = originalRoomMold.Coordinate +
                                           doorOpenIndex switch
                                           {
                                               0 => Vector2.up,
                                               1 => Vector2.down,
                                               2 => Vector2.left,
                                               _ => Vector2.right
                                           };

                if (roomMolds.Find(x => x.Coordinate == totalRoomMoldCoordinate) != null) continue;
                switch (doorOpenIndex)
                {
                    case 0 when originalRoomMold.Coordinate.y == (stageEdgeLength - 1) / 2:
                    case 1 when originalRoomMold.Coordinate.y == -(stageEdgeLength - 1) / 2:
                    case 2 when originalRoomMold.Coordinate.x == -(stageEdgeLength - 1) / 2:
                    case 3 when originalRoomMold.Coordinate.x == (stageEdgeLength - 1) / 2:
                        continue;
                }

                RoomMold totalRoomMold = new() { Coordinate = totalRoomMoldCoordinate };

                originalRoomMold.IsConnectToNearbyRoom[doorOpenIndex] = true;
                totalRoomMold.IsConnectToNearbyRoom[
                    doorOpenIndex switch
                    {
                        0 => 1,
                        1 => 0,
                        2 => 3,
                        _ => 2
                    }] = true;

                roomMolds.Add(totalRoomMold);
            }
        }

        {
            for (int i = 0; i < _roomCount; i++)
            {
                int roomMoldIndex = i == 0 ? 0 : Random.Range(0, roomMolds.Count);
                int roomDataIndex = i <= 3 ? 0 : Random.Range(0, roomData.Count);

                Room room = Instantiate(roomData[roomDataIndex].gameObject, stageGrid.transform).GetComponent<Room>();
                room.Initialize(roomMolds[roomMoldIndex].Coordinate, roomMolds[roomMoldIndex].IsConnectToNearbyRoom);

                roomMolds.RemoveAt(roomMoldIndex);
                roomData.RemoveAt(roomDataIndex);
                roomDic.Add(room.Coordinate, room);
            }
        }

        StartCoroutine(StartStage());
    }

    public void DestroyStage()
    {
        for (int i = 0; i < stageGrid.transform.childCount; i++)
            Destroy(stageGrid.transform.GetChild(i).gameObject);
    }

    private IEnumerator StartStage()
    {
        CurrentRoom = roomDic[Vector2.zero];
        CurrentRoom.Enter();
        InitialzeMap();

        UIManager.Instance.SetStageName(currentStage);
        Wakgood.Instance.transform.position = new Vector3(CurrentRoom.Coordinate.x, CurrentRoom.Coordinate.y, 0) * 100;

        if (currentStage.id != 666)
        {
            for (int i = 0; i < stageIcons.Length; i++)
            {
                if (i < currentStageID)
                {
                    stageIcons[i].color = new Color(1, 1, 1, 1);
                    stamps[i].enabled = true;
                }
                else if (i == currentStageID)
                {
                    stageIcons[i].color = new Color(1, 1, 1, 1);
                    stamps[i].enabled = false;
                }
                else
                {
                    stageIcons[i].color = new Color(0, 0, 0, 1);
                    stamps[i].enabled = false;
                }
            }

            stageLoading.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            stageLoading.GetComponent<Animator>().SetTrigger("IN");
            yield return new WaitForSeconds(.5f);
            stageLoading.gameObject.SetActive(false);
        }
        else
        {
            stageLoading2.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            stageLoading2.GetComponent<Animator>().SetTrigger("IN");
            yield return new WaitForSeconds(.5f);
            stageLoading2.gameObject.SetActive(false);
        }

        isLoading.RuntimeValue = false;
        isGaming.RuntimeValue = true;
        StartCoroutine(UIManager.Instance.SpeedWagon_Stage());

        fadePanelAnimator.SetTrigger("IN");
    }

    private void InitialzeMap()
    {
        mapGridLayoutGroup.constraintCount = stageEdgeLength;
        roomUiDic = new Dictionary<Vector2, Transform>();

        int x = -(stageEdgeLength - 1) / 2;
        int y = (stageEdgeLength - 1) / 2;

        for (int i = 0; i < mapGridLayoutGroup.transform.childCount; i++)
        {
            if (i <= stageEdgeLength * stageEdgeLength - 1)
            {
                Transform targetRoomUI = mapGridLayoutGroup.transform.GetChild(i);
                Vector2 targetRoomCoordinate = new(x, y);
                targetRoomUI.GetComponent<RoomSlot>().coordinate = targetRoomCoordinate;
                targetRoomUI.GetComponent<Image>().enabled = false;
                targetRoomUI.GetChild(0).gameObject.SetActive(false); // Door
                targetRoomUI.GetChild(1).gameObject.SetActive(false); // Property
                targetRoomUI.GetChild(2).gameObject.SetActive(false); // Icon

                if (roomDic.ContainsKey(targetRoomCoordinate))
                {
                    Room targetRoom = roomDic[targetRoomCoordinate];
                    roomUiDic.Add(targetRoomCoordinate, targetRoomUI);
                    Transform temp = targetRoomUI.GetChild(0);
                    temp.GetChild(0).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[0]); // Door\Up
                    temp.GetChild(1).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[1]); // Door\Down
                    temp.GetChild(2).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[2]); // Door\Left
                    temp.GetChild(3).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[3]); // Door\Right
                    temp = targetRoomUI.GetChild(1); // Property
                    temp.GetChild(0).GetComponent<Image>().color = new Color(189 / 255f, 81 / 255f, 90 / 255f); // Property\CurrentRoom
                    temp = targetRoomUI.GetChild(2); // Icon
                    temp.GetChild(0).gameObject.SetActive(targetRoom is SpawnRoom);   // Property\Icon\Spawn
                    temp.GetChild(1).gameObject.SetActive(targetRoom is BossRoom);    // Property\Icon\Boss
                    if (targetRoom is ShopRoom)
                    {
                        temp.GetChild(2).gameObject.SetActive((targetRoom as ShopRoom).IsItemShop);  // Property\Icon\Item
                        temp.GetChild(3).gameObject.SetActive(!(targetRoom as ShopRoom).IsItemShop);  // Property\Icon\Food
                    }
                    else
                    {
                        temp.GetChild(2).gameObject.SetActive(false);
                        temp.GetChild(3).gameObject.SetActive(false);
                    }
                }

                x++;

                if (x <= (stageEdgeLength - 1) / 2)
                    continue;

                x = -(stageEdgeLength - 1) / 2;
                y--;
            }
            else if (i > stageEdgeLength * stageEdgeLength - 1)
            {
                mapGridLayoutGroup.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        UpdateMap();
    }

    private void UpdateMap()
    {
        scrollRectBackGround.localPosition = -CurrentRoom.Coordinate * (mapGridLayoutGroup.cellSize.x + mapGridLayoutGroup.spacing.x);
        roomUiDic[CurrentRoom.Coordinate].GetComponent<Image>().enabled = true;
        roomUiDic[CurrentRoom.Coordinate].GetChild(0).gameObject.SetActive(true); // Door
        roomUiDic[CurrentRoom.Coordinate].GetChild(1).gameObject.SetActive(true); // Property
        roomUiDic[CurrentRoom.Coordinate].GetChild(2).gameObject.SetActive(true); // Icon
        roomUiDic[CurrentRoom.Coordinate].GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0f / 255f, 200f / 255f, 255f / 255f); // Property\CurrentRoom
    }

    public void SeeAllMap()
    {
        foreach (var item in roomUiDic)
        {
            var t = item.Value;
            t.GetComponent<Image>().enabled = true;
            t.GetChild(0).gameObject.SetActive(true); // Door
            t.GetChild(1).gameObject.SetActive(true); // Property
            t.GetChild(2).gameObject.SetActive(true); // Icon
        }
    }

    public IEnumerator MigrateRoom(Vector2 moveDirection, int spawnDirection)
    {
        if (isFocusOnSomething.RuntimeValue) yield break;

        if (roomUiDic.ContainsKey(CurrentRoom.Coordinate) == false || roomUiDic.ContainsKey(CurrentRoom.Coordinate + moveDirection) == false)
        {
            if (roomUiDic.ContainsKey(CurrentRoom.Coordinate) == false)
            {
                Debug.LogError("0 _ 현재 방이 존재하지 않는 좌표로 설정되어 있습니다. ");
            }
            else
            {
                Debug.LogError("1 _ 존재하지 않는 좌표의 방으로 포탈 이동을 시도했습니다 ");
            }

            isFocusOnSomething.RuntimeValue = false;
            yield break;
        }

        isFocusOnSomething.RuntimeValue = true;
        fadePanelAnimator.SetTrigger("OUT");
        yield return ws02;

        roomUiDic[CurrentRoom.Coordinate].GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f); // Property\CurrentRoom

        CurrentRoom.particle.SetActive(false);
        CurrentRoom = roomDic[CurrentRoom.Coordinate + moveDirection];
        CurrentRoom.particle.SetActive(true);

        UIManager.Instance.SetRoomName($"{CurrentRoom.gameObject.name}");

        Wakgood.Instance.transform.position = CurrentRoom.Doors[spawnDirection].transform.position + (Vector3)moveDirection * 4;

        UpdateMap();

        if (CurrentRoom.IsVisited == false) mapPanel.SetActive(false);

        CurrentRoom.Enter();

        yield return ws02;
        fadePanelAnimator.SetTrigger("IN");
        yield return ws02;
        isFocusOnSomething.RuntimeValue = false;
    }

    public IEnumerator MigrateRoom(Vector2 coordinate)
    {
        if (isFocusOnSomething.RuntimeValue) yield break;

        if (roomUiDic.ContainsKey(CurrentRoom.Coordinate) == false || roomUiDic.ContainsKey(coordinate) == false)
        {
            if (roomUiDic.ContainsKey(CurrentRoom.Coordinate) == false)
            {
                Debug.LogError("0 _ 현재 방이 존재하지 않는 좌표로 설정되어 있습니다. ");
            }
            else
            {
                Debug.LogError("2 _ 존재하지 않는 좌표의 방으로 맵 순간 이동을 시도했습니다 ");
            }

            isFocusOnSomething.RuntimeValue = false;
            yield break;
        }

        isFocusOnSomething.RuntimeValue = true;
        fadePanelAnimator.SetTrigger("OUT");
        yield return ws02;

        roomUiDic[CurrentRoom.Coordinate].GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f); // Property\CurrentRoom

        if (GameManager.Instance.curArea != AreaType.Normal)
        {
            GameManager.Instance.SwitchCurAreaToNormal();
        }

        CurrentRoom.particle.SetActive(false);
        CurrentRoom = roomDic[coordinate];
        CurrentRoom.particle.SetActive(true);

        Wakgood.Instance.transform.position = CurrentRoom.CenterSpawnPoint != null ? CurrentRoom.CenterSpawnPoint.position : CurrentRoom.transform.position;

        UpdateMap();

        if (CurrentRoom.IsVisited == false) mapPanel.SetActive(false);

        CurrentRoom.Enter();

        yield return ws02;
        fadePanelAnimator.SetTrigger("IN");
        yield return ws02;
        isFocusOnSomething.RuntimeValue = false;
    }

    private void MapDoor(bool bOpen)
    {
        if (isFighting.RuntimeValue || GameManager.Instance.isBossing.RuntimeValue)
        {
            UIManager.Instance.SpeedWagon_CantOpen();
        }
        else
        {
            scrollRectBackGround.localPosition = -CurrentRoom.Coordinate * (mapGridLayoutGroup.cellSize.x + mapGridLayoutGroup.spacing.x);
            mapPanel.SetActive(bOpen);
        }
    }
    
    public void CheckMonsterCount()
    {
        if (CurrentRoom is NormalRoom room) room.CheckMonsterCount();
    }
}