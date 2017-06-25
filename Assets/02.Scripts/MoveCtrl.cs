using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCtrl : MonoBehaviour {

    private Transform tr;
    private CharacterController controller;

    private float h = 0.0f;
    private float v = 0.0f;


    public float movSpeed = 5.0f;
    public float rotSpeed = 50.0f;

    private Vector3 movDir = Vector3.zero;

	// Use this for initialization
	void Start () {

        this.enabled = GetComponent<NetworkView>().isMine;

        tr = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        // 마우스 좌우 이동 값으로 회전
        tr.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime);

        // 이동 방향을 벡터의 덧셈 연산을 이용해 미리 계산
        movDir = (tr.forward * v) + (tr.right * h);

        // 중력의 영향을 받아 밑으로 떨어지도록 y값 변경
        movDir.y -= 20f * Time.deltaTime;

        // 플레이어의 이동
        controller.Move(movDir * movSpeed * Time.deltaTime)
                  ;	}
}
