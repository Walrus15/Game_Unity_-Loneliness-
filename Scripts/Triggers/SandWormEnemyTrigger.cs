using UnityEngine;

public class SandZoneTriggerHandler : MonoBehaviour
{
    [Header("��������� ������")]
    public string playerTag = "Player";

    [Header("������ �� �����-�����")]
    public SandWormEnemyBrain sandWormEnemyBrain;

    void Start()
    {
        if (sandWormEnemyBrain == null)
        {
            Debug.LogError("������: � ���� 'Sand Worm Enemy Brain' �� ������� '" + gameObject.name + "' �� �������� ������ SandWormEnemyBrain. " +
                           "���������� GameObject ����� � ��� ���� � ����������.");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("����� ����� � ���� �����. �������� �����.");
            sandWormEnemyBrain.PlayerEnteredSandZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("����� ����� �� ���� �����. �������� �����.");
            sandWormEnemyBrain.PlayerExitedSandZone();
        }
    }
}