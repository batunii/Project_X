using TMPro;
using UnityEngine;
using XRMultiplayer;

public class GreetingBoardUI : MonoBehaviour
{
    [SerializeField] TMP_Text m_RoomNameText;
    [SerializeField] TMP_Text m_RoomCodeText;


    private void OnEnable()
    {
        if (XRINetworkGameManager.Instance == null)
            return;

        XRINetworkGameManager.Connected.Subscribe(ConnectedToGame);
        XRINetworkGameManager.ConnectedRoomName.Subscribe(UpdateRoomName);

    }

    private void OnDisable()
    {
        if (XRINetworkGameManager.Instance == null)
            return;

        XRINetworkGameManager.Connected.Unsubscribe(ConnectedToGame);
        XRINetworkGameManager.ConnectedRoomName.Unsubscribe(UpdateRoomName);

    }

    void ConnectedToGame(bool connected)
    {
        if (m_RoomNameText == null || m_RoomCodeText == null)
            return;

        if (connected)
        {
            m_RoomNameText.text = XRINetworkGameManager.ConnectedRoomName.Value;
            m_RoomCodeText.text = XRINetworkGameManager.ConnectedRoomCode;
        }
    }

    void UpdateRoomName(string roomName)
    {
        if (m_RoomCodeText == null)
            return;

        m_RoomNameText.text = roomName;
    }
}
