using UnityEngine;

public class SpawnChambers : MonoBehaviour
{
    public GameObject prefab;

    void Awake()
    {
        //StartChamber - ��������� ������� ���� � ��� ��������� � ������
        Chamber startChamber = SpawnStartChamber(prefab);

        //������
        Chamber roomLeftwardOfStartChamber = SpawnLeftward(startChamber, prefab);
    }

    /// <summary>
    /// ������� prefab ������� �����
    /// </summary>
    /// <param name="chamber"> �������, ����� ������� ������� ����� ������������� � ����� �������� </param>
    /// <param name="prefab_"> ������ ����� �������</param>
    /// <returns></returns>
    public Chamber SpawnLeftward(in Chamber chamber, in GameObject prefab_)
    {
        Chamber newRoom = Instantiate(prefab_, Vector3.zero, Quaternion.identity, transform).GetComponent<Chamber>();
        newRoom.SetLeftRotation();

        newRoom.transform.position = chamber.GetLeftContactPoint() - newRoom.GetRightContactPoint();

        return newRoom;
    }

    /// <summary>
    /// ������� prefab ������� ������
    /// </summary>
    /// <param name="chamber"> �������, ������ ������� ������� ����� ������������� � ����� �������� </param>
    /// <param name="prefab_"> ������ ����� �������</param>
    /// <returns></returns>
    public Chamber SpawnRightward(in Chamber chamber, in GameObject prefab_)
    {
        Chamber newRoom = Instantiate(prefab_, Vector3.zero, Quaternion.identity, transform).GetComponent<Chamber>();
        newRoom.SetRightRotation();

        newRoom.transform.position = chamber.GetRightContactPoint() - newRoom.GetLeftContactPoint();

        return newRoom;
    }

    /// <summary>
    /// ������� prefab ������� ������
    /// </summary>
    /// <param name="chamber"> �������, ������� ������� ������� ����� ������������� � ����� �������� </param>
    /// <param name="prefab_"> ������ ����� �������</param>
    /// <returns></returns>
    public Chamber SpawnUpward(in Chamber chamber, in GameObject prefab_)
    {
        Chamber newRoom = Instantiate(prefab_, Vector3.zero, Quaternion.identity, transform).GetComponent<Chamber>();
        newRoom.SetTopRotation();

        newRoom.transform.position = chamber.GetTopContactPoint() - newRoom.GetBottomContactPoint();

        return newRoom;
    }

    /// <summary>
    /// ������� prefab ������� �����
    /// </summary>
    /// <param name="chamber"> �������, ������ ������� ������� ����� ������������� � ����� �������� </param>
    /// <param name="prefab_
    /// <returns></returns>
    public Chamber SpawnDownward(in Chamber chamber, in GameObject prefab_)
    {
        Chamber newRoom = Instantiate(prefab_, Vector3.zero, Quaternion.identity, transform).GetComponent<Chamber>();
        newRoom.SetBottomRotation();

        newRoom.transform.position = chamber.GetBottomContactPoint() - newRoom.GetTopContactPoint();

        return newRoom;
    }

    /// <summary>
    /// ������� ��������� ������� � ������ ������
    /// </summary>
    /// <param name="prefab_"> ������ ��������� ������� </param>
    /// <returns></returns>
    public Chamber SpawnStartChamber(in GameObject prefab_)
    {
        Chamber newRoom = Instantiate(prefab_, Vector3.zero, Quaternion.identity, transform).GetComponent<Chamber>();
        newRoom.SetLeftRotation();

        return newRoom;
    }



/*    public void SpawnLevelChambers()
    {
        Chamber room1 = SpawnStartChamber(prefab);

        Chamber room2 = SpawnLeftward(room1, prefab);

        Chamber room3 = SpawnRightward(room1, prefab);

        Chamber room4 = SpawnUpward(room1, prefab);

        Chamber room5 = SpawnDownward(room1, prefab);

        Chamber room6 = SpawnDownward(room5, prefab);

        Chamber room7 = SpawnLeftward(room5, prefab);

        Chamber room8 = SpawnRightward(room5, prefab);
        *//*
                Chamber leftRoom = Instantiate(prefab, transform.position, Quaternion.identity, transform.parent).GetComponent<Chamber>();
                leftRoom.SetLeftRotation();

                leftRoom.transform.position = room1.GetLeftContactPoint() - leftRoom.GetRightContactPoint();

                Chamber leftRoom1 = Instantiate(prefab, transform.position, Quaternion.identity, transform.parent).GetComponent<Chamber>();
                leftRoom1.SetBottomRotation();

                leftRoom1.transform.position = leftRoom.GetLeftContactPoint() - leftRoom1.GetRightContactPoint();*/

        /*        �� ���� �������� �������� ������� � ������� doors � walls 
                ������� ������� ��� ��������� �������, ������� ������� ������ � ������ ������*//*
    }*/
}
