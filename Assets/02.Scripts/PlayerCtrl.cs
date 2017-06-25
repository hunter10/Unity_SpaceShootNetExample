using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.Utility;

public class PlayerCtrl : MonoBehaviour {

    private Transform tr;
    private NetworkView _networkView;

    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        _networkView = GetComponent<NetworkView>();

        if(_networkView.isMine){
            Camera.main.GetComponent<SmoothFollow>().target = tr;

        }
    }

    private void Update()
    {
        if(_networkView.isMine){
            
        }
        else // 원격 플레이어일 때 수
        {
            // 전송받아온 변경된 위치로 부드럽게 이동 
            tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
			// 전송받아온 변경된 각도로 부드럽게 회전 
			tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);
        }
    }

    // NetworkView 컴포넌트에서 호출해 주는 콜백 함수
    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        // 로컬 플레이어의 위치 및 회전 정보 송신
        if(stream.isWriting)
        {
            Vector3 pos = tr.position;
            Quaternion rot = tr.rotation;

            // 데이터 전송
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else //원격 플레이어의 위치 및 회전 정보 수신
        {
            Vector3 revPos = Vector3.zero;
            Quaternion revRot = Quaternion.identity;

			// 데이터 전송
			stream.Serialize(ref revPos);
			stream.Serialize(ref revRot);

            currPos = revPos;
            currRot = revRot;
        }
    }
}
