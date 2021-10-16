using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    private static StageManager instance;
    [HideInInspector] public static StageManager Instance { get { return instance; } }

    public int currentStageID = -1;
    [SerializeField] private StageDataBuffer stageDataBuffer;

    [SerializeField] private int roomCount = 0;
    [SerializeField] private int stageEdgeLength = 0;
    private Dictionary<Vector2, Room> roomDic = new();
    private List<RoomMold> roomMolds = new();
    private class RoomMold
    {
        public Vector2 coordinate;
        public bool[] isConnectToNearbyRoom = new bool[4];
    }
    private List<Room> roomDatas = new();
    public Room CurrentRoom { get; private set; }
    [SerializeField] private GameObject stageGrid;

    [SerializeField] private GameObject miniMapCamera;

    public GameObject mapPanel;
    [SerializeField] private GridLayoutGroup mapGridLayoutGroup;
    [SerializeField] private RectTransform scrollRectBackGround;
    private Dictionary<Vector2, Transform> roomUiDic = new();

    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Animator fadePanelAnimator;
    [SerializeField] private TextMeshProUGUI noticeText;

    [SerializeField] private TextMeshProUGUI stageNumberText, stageNameCommentText;
    [SerializeField] private GameObject stageSpeedWagon;

    private IEnumerator canOpenText;
    private WaitForSeconds ws02;

    private void Awake()
    {
        instance = this;
        ws02 = new WaitForSeconds(.2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && mapPanel.activeSelf == false) MapDoor(true);
        else if (Input.GetKeyUp(KeyCode.Tab) && mapPanel.activeSelf == true) MapDoor(false);
    }

    public void GenerateStage()
    {
        DestroyStage();
        roomMolds.Clear();
        roomDic.Clear();

        roomDatas = new(stageDataBuffer.Items[++currentStageID].roomDatas);
        roomMolds.Add(new() { coordinate = Vector2.zero });

        /* 스테이지 틀 만들기 */
        {
            RoomMold originalRoomMold, totalRoomMold;
            Vector2 totalRoomMoldCoordinante;
            int doorOpenIndex;

            while (roomMolds.Count < roomCount)
            {
                originalRoomMold = roomMolds[Random.Range(0, roomMolds.Count)];
                doorOpenIndex = Random.Range(0, 4);
                totalRoomMoldCoordinante = originalRoomMold.coordinate +
                    ((doorOpenIndex == 0) ? Vector2.up : (doorOpenIndex == 1) ? Vector2.down : (doorOpenIndex == 2) ? Vector2.left : Vector2.right);

                if (roomMolds.Find(x => x.coordinate == totalRoomMoldCoordinante) != null) continue;
                if (doorOpenIndex == 0 && originalRoomMold.coordinate.y == (stageEdgeLength - 1) / 2) continue;
                else if (doorOpenIndex == 1 && originalRoomMold.coordinate.y == -(stageEdgeLength - 1) / 2) continue;
                else if (doorOpenIndex == 2 && originalRoomMold.coordinate.x == -(stageEdgeLength - 1) / 2) continue;
                else if (doorOpenIndex == 3 && originalRoomMold.coordinate.x == (stageEdgeLength - 1) / 2) continue;

                totalRoomMold = new() { coordinate = totalRoomMoldCoordinante };

                originalRoomMold.isConnectToNearbyRoom[doorOpenIndex] = true;
                totalRoomMold.isConnectToNearbyRoom[(doorOpenIndex == 0) ? 1 : (doorOpenIndex == 1) ? 0 : (doorOpenIndex == 2) ? 3 : 2] = true;

                roomMolds.Add(totalRoomMold);
            }
        }

        /* 스테이지 채우기 */
        {
            int roomMoldIndex, roomDataIndex;
            Room room;

            for (int i = 0; i < roomCount; i++)
            {
                roomMoldIndex = (i == 0) ? 0 : Random.Range(0, roomMolds.Count);
                roomDataIndex = (i <= 2) ? 0 : Random.Range((DataManager.Instance.curGameData.isNPCRescued) ? 1 : 0, roomDatas.Count);

                // Todo : 스테이지 별 룸 데이타 딕셔너리 혹은 배열만들어야 함
                room = Instantiate(roomDatas[roomDataIndex].gameObject, stageGrid.transform).GetComponent<Room>();
                room.Initialize(roomMolds[roomMoldIndex].coordinate, roomMolds[roomMoldIndex].isConnectToNearbyRoom);

                roomMolds.RemoveAt(roomMoldIndex);
                roomDatas.RemoveAt(roomDataIndex);
                roomDic.Add(room.Coordinate, room);
            }
        }

        StartCoroutine(StartStage());
    }

    public void DestroyStage()
    {
        for (int i = 0; i < stageGrid.transform.childCount; i++)
        {
            Destroy(stageGrid.transform.GetChild(i).gameObject);
        }
    }

    private IEnumerator StartStage()
    {
        CurrentRoom = roomDic[Vector2.zero];
        CurrentRoom.Enter();
        InitialzeMap();

        Wakgood.Instance.transform.position = new Vector3(CurrentRoom.Coordinate.x, CurrentRoom.Coordinate.y, 0) * 100;
        miniMapCamera.transform.position = new Vector3(CurrentRoom.Coordinate.x, CurrentRoom.Coordinate.y, -1) * 100;

        fadePanelAnimator.SetTrigger("FadeIn");
        StartCoroutine("StageSpeedWagon");
        yield return ws02;
        fadePanel.SetActive(false);
    }

    private void InitialzeMap()
    {
        mapGridLayoutGroup.constraintCount = stageEdgeLength;
        roomUiDic = new();

        int x = -(stageEdgeLength - 1) / 2;
        int y = (stageEdgeLength - 1) / 2;

        Transform targetRoomUI;
        Vector2 targetRoomCoordinate;
        Room targetRoom;
        Transform temp;

        for (int i = 0; i < mapGridLayoutGroup.transform.childCount; i++)
        {
            if (i <= stageEdgeLength * stageEdgeLength - 1)
            {
                targetRoomUI = mapGridLayoutGroup.transform.GetChild(i);
                targetRoomCoordinate = new(x, y);
                // Debug.Log(targetRoomCoordinate);
                targetRoomUI.GetComponent<Image>().enabled = false;
                targetRoomUI.GetChild(0).gameObject.SetActive(false); // Door
                targetRoomUI.GetChild(1).gameObject.SetActive(false); // Property

                if (roomDic.ContainsKey(targetRoomCoordinate))
                {
                    targetRoom = roomDic[targetRoomCoordinate];
                    roomUiDic.Add(targetRoomCoordinate, targetRoomUI);
                    temp = targetRoomUI.GetChild(0); // Door
                    temp.GetChild(0).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[0]); // Door\Up
                    temp.GetChild(1).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[1]); // Door\Down
                    temp.GetChild(2).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[2]); // Door\Left
                    temp.GetChild(3).gameObject.SetActive(targetRoom.IsConnectToNearbyRoom[3]); // Door\Right
                    temp = targetRoomUI.GetChild(1); // Property
                    temp.GetChild(0).GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f); // Property\CurrentRoom
                    temp = temp.GetChild(2); // Icon
                    temp.GetChild(0).gameObject.SetActive(targetRoom.roomType == RoomType.Boss); // Property\Icon\Boss
                    temp.GetChild(1).gameObject.SetActive(targetRoom.roomType == RoomType.Spawn); // Property\Icon\Spawn
                    temp.GetChild(2).gameObject.SetActive(targetRoom.roomType == RoomType.Shop); // Property\Icon\Shop
                    temp.GetChild(2).gameObject.SetActive(targetRoom.roomType == RoomType.Shop); // Property\Icon\Shop
                }

                x++;
                if (x > (stageEdgeLength - 1) / 2)
                {
                    x = -(stageEdgeLength - 1) / 2;
                    y--;
                }
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
        roomUiDic[CurrentRoom.Coordinate].GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0f / 255f, 200f / 255f, 255f / 255f); // Property\CurrentRoom
    }

    public IEnumerator MigrateRoom(Vector2 moveDirection, int spawnDirection)
    {
        fadePanel.SetActive(true);
        yield return ws02;

        roomUiDic[CurrentRoom.Coordinate].GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f); // Property\CurrentRoom

        CurrentRoom = roomDic[CurrentRoom.Coordinate + moveDirection];
        Wakgood.Instance.transform.position = CurrentRoom.Doors[spawnDirection].transform.position + (Vector3)moveDirection * 4;
        miniMapCamera.transform.position = new Vector3(CurrentRoom.Coordinate.x, CurrentRoom.Coordinate.y, -1) * 100;

        UpdateMap();

        if (CurrentRoom.IsVisited == false) mapPanel.SetActive(false);

        CurrentRoom.Enter();

        yield return ws02;
        fadePanelAnimator.SetTrigger("FadeIn");
        yield return ws02;
        fadePanel.SetActive(false);
    }

    private void MapDoor(bool bOpen)
    {
        if (GameManager.Instance.IsFighting)
        {
            if (canOpenText != null) StopCoroutine(canOpenText);
            StartCoroutine(canOpenText = CantOpenText());
        }
        else
        {
            if (canOpenText != null) StopCoroutine(canOpenText);

            scrollRectBackGround.localPosition = -CurrentRoom.Coordinate * (mapGridLayoutGroup.cellSize.x + mapGridLayoutGroup.spacing.x);
            mapPanel.SetActive(bOpen);
        }
    }

    private IEnumerator StageSpeedWagon()
    {
        stageSpeedWagon.SetActive(true);
        stageNumberText.text = $"1-{stageDataBuffer.Items[currentStageID].id}";
        stageNameCommentText.text = $"{stageDataBuffer.Items[currentStageID].name} : {stageDataBuffer.Items[currentStageID].comment}";
        yield return new WaitForSeconds(2f);
        stageSpeedWagon.SetActive(false);
    }

    private IEnumerator CantOpenText()
    {
        noticeText.text = "전투 중에는 열 수 없습니다.";
        noticeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        noticeText.gameObject.SetActive(false);
    }

    public void StopAllSpeedWagons()
    {
        StopCoroutine("StageSpeedWagon");
        stageSpeedWagon.SetActive(false);
        StopCoroutine("CantOpenText");
        noticeText.gameObject.SetActive(false);
    }

    public void CheckMonsterCount()
    {
        if (CurrentRoom is NormalRoom) (CurrentRoom as NormalRoom).CheckMonsterCount();
    }
}