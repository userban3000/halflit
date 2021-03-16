using System.Collections;
using System.Collections.Generic;
using static ThrowableOptions;
using UnityEngine;

public class Player : DamageableEntity {
    
    [Header("Player")]
    public GameObject playerHolder;
    public float playerSpeed;
    public float mouseSens;
    public float rotAngle;

    [Header("Camera & Mouse")]
    public Camera cam;
    public GameObject crosshair;
    public float cameraDist = 10;
    [Range(1,10)]
    public float camMovementWithCrosshair = 4;
    [Range(0,10)]
    public float camSmoothness = 4.5f;
    private Vector3 camTargetPos;
    private Vector3 currentTargetPos;

    [Header("Shooting")]
    public GameObject throwablePrefab;
    public float playerMovementInfluenceOnThrowing = 1f;
    public float throwForce = 14f;
    private Vector3 mouseWhenStartHolding;
    private bool aiming;
    public float aimingSens;
    //should be private, only public for debug reasons
    public float throwPower;
    private GameObject thrownObject;

    protected override void Start() {
        base.Start();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void Update() {
        float xMov = Input.GetAxisRaw("Horizontal");
        float yMov = Input.GetAxisRaw("Vertical");

        Vector3 playerMovement = new Vector3(xMov,yMov,0).normalized;
        Vector3 playerToCrosshair = crosshair.transform.position - playerHolder.transform.position;

        //PLAYER MOVEMENT
        playerHolder.transform.position = playerHolder.transform.position + playerMovement * Time.deltaTime * playerSpeed;

        //PLAYER ROTATION
        rotAngle = Vector3.SignedAngle(Vector3.up, playerToCrosshair, Vector3.forward);
        playerHolder.transform.rotation = Quaternion.Euler(0, 0, rotAngle);

        //CROSSHAIR MOVEMENT
        Vector3 mPos = Input.mousePosition * mouseSens;
        mPos.z = 10;
        if ( !aiming )
            crosshair.transform.position = cam.ScreenToWorldPoint(mPos);

        //SHOOTING
        if ( Input.GetMouseButtonDown(0) ) {
            //start tracking force
            mouseWhenStartHolding = Input.mousePosition;
            aiming = true;
            throwPower = 0.5f;

            thrownObject = Instantiate(throwablePrefab, playerHolder.transform.position, playerHolder.transform.rotation) as GameObject;
        }
        if ( Input.GetMouseButton(0) ) {
            Vector3 mouseCurrent = Input.mousePosition;
            throwPower = (Mathf.Clamp( (mouseWhenStartHolding - mouseCurrent).sqrMagnitude, -aimingSens, aimingSens) + aimingSens) / 2 * aimingSens ;
        }
        if ( Input.GetMouseButtonUp(0) ) {
            Throwable thrown = thrownObject.GetComponent<Throwable>();
            Vector3 throwDir = playerToCrosshair.normalized * 3f + playerMovement * playerMovementInfluenceOnThrowing;
            thrown.Throw(throwDir, throwForce * throwPower);
        }


        //CAMERA MOVEMENT
        camTargetPos = playerHolder.transform.position + playerToCrosshair / camMovementWithCrosshair + new Vector3 (0, 0, -cameraDist);
        StartCoroutine(SmoothCam());
        cam.transform.position = currentTargetPos;
    }

    IEnumerator SmoothCam() {
        float t = 0;
        float percent = 0;

        Vector3 originalPos = cam.gameObject.transform.position;

        while ( percent < 1 ) {
            t += Time.deltaTime;
            percent = t * camSmoothness;

            currentTargetPos = Vector3.Lerp(originalPos, camTargetPos, percent);

            yield return null;
        }
    }

}
