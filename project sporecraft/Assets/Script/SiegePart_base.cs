using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegePart_base : MonoBehaviour
{
    public Renderer rendererToFindEdges;

    List<Vector3> directions = new List<Vector3>();
    List<Transform> Sockets = new List<Transform>();
    List<Transform> disabledSockets = new List <Transform>();
    Joint joint;

    [SerializeField]
    SelectDirections selectDirections; //joint 할수있는 위치를 정하는

    [System.Serializable]
    public class SelectDirections
    {
        public bool up;
        public bool down;
        public bool forward;
        public bool right;
        public bool left;
        public bool back;
    }

    public bool debugPos;
    public GameObject debugPrefab;


    void Start()
    {
        Transform trans = rendererToFindEdges.transform;

        if(GetComponent<Joint>())
            joint = GetComponent<Joint>();

        if (selectDirections.up)
        {
            Vector3 addDirection = trans.up + new Vector3(0, rendererToFindEdges.bounds.extents.y - 1, 0);
            Vector3 finalPos = trans.position + addDirection;

            directions.Add(finalPos);
        }
        if (selectDirections.down)
        {
            Vector3 addDirection = -trans.up + new Vector3(0, -rendererToFindEdges.bounds.extents.y + 1, 0);
            Vector3 finalPos = trans.position + addDirection;

            directions.Add(finalPos);
        }
        if (selectDirections.right)
        {
            Vector3 addDirection = trans.right + new Vector3(rendererToFindEdges.bounds.extents.x - 1,0, 0);
            Vector3 finalPos = trans.position + addDirection;

            directions.Add(finalPos);
        }
        if (selectDirections.left)
        {
            Vector3 addDirection = -trans.right + new Vector3(-rendererToFindEdges.bounds.extents.x + 1,0, 0);
            Vector3 finalPos = trans.position + addDirection;

            directions.Add(finalPos);
        }
        if (selectDirections.forward)
        {
            Vector3 addDirection = trans.forward + new Vector3(0,0, rendererToFindEdges.bounds.extents.z - 1);
            Vector3 finalPos = trans.position + addDirection;

            directions.Add(finalPos);
        }
        if (selectDirections.back)
        {
            Vector3 addDirection = -trans.forward + new Vector3(0,0,-rendererToFindEdges.bounds.extents.z + 1);
            Vector3 finalPos = trans.position + addDirection;

            directions.Add(finalPos);
        }

        for(int i = 0; i < directions.Count; i++)
        {
            GameObject obj;

            if (debugPos)
            {
                obj = (GameObject)Instantiate(debugPrefab, directions[i], Quaternion.identity);
            }
            else
            {
                obj = new GameObject();
                obj.transform.position = directions[i];
            }

            obj.transform.parent = transform;
            Sockets.Add(obj.transform);
        }
    }

    
    public Transform RetrunClosetDirection(Vector3 pos)
    {
        Transform retVal = null;

        if(directions.Count > 0)
        {
            Transform closetTrans = Sockets[0]; //가장 처음것부터
            Vector3 closetPos = Sockets[0].position;
            float distance = Vector3.Distance(pos, closetPos); //거리찾음

            for(int i = 0; i < directions.Count; i++)
            {
                float tempDist = Vector3.Distance(pos, Sockets[i].position);

                if(tempDist < distance)
                {
                    closetPos = Sockets[i].position;
                    closetTrans = Sockets[i];
                    distance = tempDist; 
                }//더 가까운 소켓이 있다면 가장가까운 소켓으로 배정
            }

            retVal = closetTrans;
        }

        return retVal;
    }

    public void DisableSocket(Transform socket)
    {
        if (socket)
        {
            socket.gameObject.SetActive(false);

            if(Sockets.Contains(socket))
                Sockets.Remove(socket);

            disabledSockets.Add(socket);
        }
    }

    public void AssignTargetToJoint(Transform target)
    {
        joint.connectedBody = target.GetComponent<Rigidbody>();
    }
}
