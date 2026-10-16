using UnityEngine;

public class vehicleLogic : MonoBehaviour
{
    [SerializeField] Transform[] springPoints;
    [SerializeField] Transform[] steerPoints;
    [SerializeField] float restLength;
    [SerializeField] float springStrength;
    [SerializeField] float damperStiffness;
    [SerializeField] float acceleration;
    [SerializeField] float maxSteerAngle;
    [SerializeField] float tireGripFactor;
    [SerializeField]Rigidbody vehicleRb;
    float forwardInput = 1f;
    float steerInput = 0.5f;

    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        Steering();
    }
    void FixedUpdate() 
    {
        Suspension();
        
    }
    void Steering() 
    {
        /*if (keyboard.current.dKey.isPressed)
        {
            foreach (Transform steerPoint in steerPoints) 
            {
                steerPoint.localRotation = Quaternion.Euler(0f, steerInput * maxSteerAngle, 0f);
            }
        }
        else if (Keyboard.current.aKey.isPressed) 
        {
            foreach (Transform steerPoint in steerPoints)
            {
                steerPoint.localRotation = Quaternion.Euler(0f, -steerInput * maxSteerAngle, 0f);
            }
        }*/
    }
    void Suspension() 
    {
        foreach (Transform springPoint in springPoints) 
        {
            RaycastHit hit;
            Vector3 forwardDir = springPoint.forward;
            Vector3 steerDir = springPoint.right;
            if (Physics.Raycast(springPoint.position,-transform.up,out hit, restLength)) 
            {
                
                float displacement = restLength - hit.distance;
                float springVelocity = Vector3.Dot(springPoint.up,vehicleRb.GetPointVelocity(springPoint.position));
                float netForce = (displacement * springStrength) - (springVelocity * damperStiffness);
                vehicleRb.AddForceAtPosition(netForce * springPoint.up, springPoint.position);


                vehicleRb.AddForceAtPosition(forwardDir * acceleration * forwardInput, springPoint.position);
                Vector3 tireVelocity = vehicleRb.GetPointVelocity(springPoint.position);
                
                float steerVal = Vector3.Dot(steerDir, tireVelocity);
                float intendedValChange = -steerVal * tireGripFactor;
                float finalChange = intendedValChange / Time.deltaTime;
                vehicleRb.AddForceAtPosition(steerDir * finalChange, springPoint.position);
            }

            
        }
    }
    
}
