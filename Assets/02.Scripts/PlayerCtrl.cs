using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityStandardAssets.Utility;

public class PlayerCtrl : MonoBehaviour {

    public enum AnimState
    {
        idle = 0,
        runForward,
        runBackward,
        runRight,
        runLeft
    }

    public AnimState animState = AnimState.idle;
    // 사용할 애니메이션 클립 배열
    public AnimationClip[] animClips;

    private CharacterController controller;
    private Animation anim;

    private Transform tr;
    private NetworkView _networkView;

    // 위치 정보를 송수신할 때 사용할 변수 선언 및 초깃값 설정
    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;

    public GameObject bullet;
    public Transform firePos;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        _networkView = GetComponent<NetworkView>();
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animation>();

        if(_networkView.isMine){
            Camera.main.GetComponent<SmoothFollow>().target = tr;

        }
    }

    private void Update()
    {
        if(_networkView.isMine){
            if(Input.GetMouseButtonDown(0))
            {
                // 자신은 로컬 함수를 호출해 발사
                Fire();

                //  자신을 제외한 나머지 원격 사용자에게 Fire 함수를 원격 호출
                _networkView.RPC("Fire", RPCMode.Others);
            }

            // CharacterController의 속도 벡터를 로컬 벡터로 변환
            Vector3 localVelocity = tr.InverseTransformDirection(controller.velocity);

            Vector3 forwardDir = new Vector3(0f, 0f, localVelocity.z);

            Vector3 rightDir = new Vector3(localVelocity.x, 0f, 0f);

            if(forwardDir.z >= 0.1f)
            {
                animState = AnimState.runForward;
            }
            else if(forwardDir.z <= -1.0f)
            {
                animState = AnimState.runBackward;
            }
            else if(rightDir.x >= 0.1f)
            {
                animState = AnimState.runRight;
            }
            else if (rightDir.x <= -0.1f)
            {
                animState = AnimState.runLeft;
            }
            else
            {
                animState = AnimState.idle;
            }

            anim.CrossFade(animClips[(int)animState].name, 0.2f);


        }
        else // 원격 플레이어일 때 수
        {
            // 전송받아온 변경된 위치로 부드럽게 이동 
            tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);

			// 전송받아온 변경된 각도로 부드럽게 회전 
			tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);

            anim.CrossFade(animClips[(int)animState].name, 0.1f);
        }
    }

    [RPC]
    void Fire()
    {
        GameObject.Instantiate(bullet, firePos.position, firePos.rotation);
    }

    // NetworkView 컴포넌트에서 호출해 주는 콜백 함수
    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        // 로컬 플레이어의 위치 및 회전 정보 송신
        if(stream.isWriting)
        {
            Vector3 pos = tr.position;
            Quaternion rot = tr.rotation;
            int _animState = (int)animState;

            // 데이터 전송
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref _animState);
        }
        else //원격 플레이어의 위치 및 회전 정보 수신
        {
            Vector3 revPos = Vector3.zero;
            Quaternion revRot = Quaternion.identity;
            int _animState = 0;

			// 데이터 전송
			stream.Serialize(ref revPos);
			stream.Serialize(ref revRot);
            stream.Serialize(ref _animState);

            currPos = revPos;
            currRot = revRot;
            animState = (AnimState)_animState;
        }
    }
}
